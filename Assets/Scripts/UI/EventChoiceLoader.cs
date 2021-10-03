using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Unstable.Model;

namespace Unstable.UI
{
    public class EventChoiceLoader : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
    {
        [SerializeField]
        private Text _title, _description;

        [SerializeField]
        private GameObject _requirementPanel;

        [SerializeField]
        private Text _requirementText;

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

            if (choice.Requirements.Any())
            {
                _requirementPanel.SetActive(false);
                _requirementText.text = string.Join("\n", choice.Requirements.Select(r =>
                {
                    var s = r.Key.Split(' ');
                    return GameManager.Instance.GetCard(s[0], s[1]).Name + ": " + r.Value;
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
