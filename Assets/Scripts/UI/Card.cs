using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Unstable.SO;
using System.Collections.Generic;

namespace Unstable.UI
{
    public class Card : MonoBehaviour, IDragHandler, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IEndDragHandler
    {
        [SerializeField]
        private TMP_Text _title, _description;

        [SerializeField]
        private Image _background;
        private Vector3 _target;
        private RectTransform _canvas;

        // Drag and drop
        private bool _isHold;
        private Vector2 _offset;

        public Dictionary<string, int> Effects {get; private set;}

        private void Start()
        {
            _canvas = (RectTransform)GetComponentInParent<Canvas>().transform;
        }

        public void Init(Model.Card card, FactionInfo _faction)
        {
            _title.text = card.Name;
            _description.text = card.Effects == null ? "" : string.Join("\n", card.Effects.Select(e => GameManager.Instance.GetEffect(e.Key) + ": " + e.Value));
            name = "Card " + card.Name;

            Effects = card.Effects;

            _background.sprite = _faction.CardBackground;
        }

        public void SetTarget(Vector2 pos)
        {
            _target = pos;
        }


        private void FixedUpdate()
        {
            if (!_isHold && transform.localPosition != _target)
            {
                transform.localPosition = Vector3.Slerp(transform.localPosition, _target, .1f);
            }
        }

        public void OnBeginDrag(PointerEventData eventData){
            _isHold = true;
            Vector2 localPosition = _canvas.transform.InverseTransformPoint(Camera.main.ScreenToWorldPoint(eventData.position));
            _offset = (Vector2)transform.localPosition - localPosition;
            this.GetComponent<Image>().raycastTarget = false;
            foreach(var el in this.GetComponentsInChildren<TMP_Text>()){
                el.raycastTarget = false;
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector2 localPosition = _canvas.transform.InverseTransformPoint(Camera.main.ScreenToWorldPoint(eventData.position));
            transform.localPosition = localPosition + _offset;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            transform.SetAsLastSibling();
            if (!_isHold)
            {
                transform.localPosition = transform.localPosition + new Vector3(0.0f, 40.0f, 0.0f);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            foreach (var elem in transform.parent.GetComponentsInChildren<RectTransform>().OrderBy(x => x.transform.position.x))
            {
                elem.SetAsLastSibling();
            }
            if (!_isHold)
            {
                transform.localPosition = transform.localPosition - new Vector3(0.0f, 40.0f, 0.0f);
            }
        }

        public void OnEndDrag(PointerEventData eventData){
            _isHold = false;
            this.GetComponent<Image>().raycastTarget = true;
            foreach(var el in this.GetComponentsInChildren<TMP_Text>()){
                el.raycastTarget = true;
            }
        }

    }
}
