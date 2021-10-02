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
