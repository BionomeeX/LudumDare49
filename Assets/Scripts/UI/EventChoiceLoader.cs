using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Unstable.Model;

namespace Unstable.UI
{
    public class EventChoiceLoader : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
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

        private void Start()
        {
            _image = GetComponent<Image>();
            _baseColor = _image.color;
        }

        public void Init(EventChoice choice)
        {
            _title.text = GameManager.Instance.GetLeaderFromTrigram(choice.TargetTrigram).DomainName;
            _description.text = choice.Description;
            _choiceData = choice;

            Debug.Log(choice.Requirements.Count);
            if (choice.Requirements?.Any() ?? false)
            {
                _requirementPanel.SetActive(false);
                _requirementText.text = string.Join("\n", choice.Requirements.Select(r =>
                {
                    return GameManager.Instance.GetEffect(r.Key) + ": " + r.Value;
                }));
            }
            else
            {
                _requirementPanel.SetActive(false);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _image.color = new Color(_baseColor.r - .2f, _baseColor.g - .2f, _baseColor.b - .2f);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _image.color = _baseColor;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            foreach (var effect in _choiceData.Effects)
            {
                EventManager.DoAction(effect.MethodName, effect.Argument);
            }
            GameManager.Instance.EndEvent();
        }
    }
}
