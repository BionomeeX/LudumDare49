using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Unstable.Model;

namespace Unstable.UI
{
    public class EventChoiceLoader : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IDropHandler
    {
        [SerializeField]
        private TMP_Text _title, _description;

        [SerializeField]
        private GameObject _requirementPanel;

        [SerializeField]
        private TMP_Text _requirementText;

        private Image _image;
        private Color _baseColor;

        private EventChoice _choiceData;

        private List<(string, int)> _requirements;

        public void Init(EventChoice choice)
        {
            _title.text = choice.TargetTrigram != null
                ? GameManager.Instance.GetLeaderFromTrigram(choice.TargetTrigram).DomainName
                : "";
            _description.text = choice.Description;
            _choiceData = choice;

            _requirements = new();

            if (choice.Requirements != null && choice.Requirements.Any())
            {
                _requirements = choice.Requirements.Select(r =>
                {
                    return (GameManager.Instance.GetEffect(r.Key), r.Value);
                }).ToList();
            }
            UpdateRequirementDisplay();
        }

        public void UpdateRequirementDisplay()
        {
            if (_image == null)
            {
                _image = GetComponent<Image>();
                _baseColor = _image.color;
            }

            if (_requirements.Any())
            {
                _requirementPanel.SetActive(true);
                _requirementText.text = string.Join("\n", _requirements.Select(r =>
                {
                    return r.Item1 + ": " + r.Item2;
                }));
                _image.color = new Color(_baseColor.r - .2f, _baseColor.g - .2f, _baseColor.b - .2f);
            }
            else
            {
                _requirementPanel.SetActive(false);
                _requirementText.text = "";
                _image.color = _baseColor;
            }

            if (_choiceData.Effects != null && _choiceData.Effects.Any())
            {
                if (!string.IsNullOrWhiteSpace(_requirementText.text))
                {
                    _requirementText.text += "\n\n";
                }

                _requirementText.text += string.Join("\n", _choiceData.Effects.Select(x => EventManager.ActionToString(x.MethodName, x.Argument)));
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _image.color = new Color(_baseColor.r - .2f, _baseColor.g - .2f, _baseColor.b - .2f);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!_requirements.Any())
            {
                _image.color = _baseColor;
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!_requirements.Any())
            {
                if (_choiceData.Effects != null)
                {
                    foreach (var effect in _choiceData.Effects)
                    {
                        EventManager.DoAction(effect.MethodName, effect.Argument);
                    }
                }
                GameManager.Instance.EndEvent();
            }
        }

        public void OnDrop(PointerEventData eventData) {
            Debug.Log("Dropped: " + eventData.pointerDrag);
        }
    }
}
