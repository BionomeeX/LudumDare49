using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Unstable.Model;

namespace Unstable.UI
{
    public class EventChoiceLoader : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField]
        private Text _title, _description;

        private Image _image;
        private Color _baseColor;

        private void Start()
        {
            _image = GetComponent<Image>();
            _baseColor = _image.color;
        }

        public void Init(EventChoice choice)
        {
            _title.text = GameManager.Instance.GetLeaderFromTrigram(choice.TargetTrigram).DomainName;
            _description.text = choice.Description;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _image.color = new Color(_baseColor.r - .2f, _baseColor.g - .2f, _baseColor.b - .2f);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _image.color = _baseColor;
        }
    }
}
