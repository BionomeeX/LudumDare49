using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Unstable.UI
{
    public class Card : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField]
        private TMP_Text _title;

        private Vector2 _target;
        private RectTransform _canvas;

        private void Start()
        {
            _canvas = (RectTransform)GetComponentInParent<Canvas>().transform;
        }

        public void Init(Model.Card card)
        {
            _title.text = card.Name;
        }

        public void SetTarget(Vector2 pos)
        {
            _target = pos;
        }

        // Drag and drop
        private bool _isHold;
        private bool _hasMoved;
        private Vector2 _offset;

        private void FixedUpdate()
        {
            if ((!_isHold & !_hasMoved)| !_hasMoved)
            {
                transform.position = Vector3.Slerp(transform.position, new Vector2(_canvas.sizeDelta.x / 2f, 0) + _target, .1f);
            }
        }

        public void OnPointerDown(PointerEventData data)
        {
            _isHold = true;
            _offset = (Vector2)transform.position - data.position;
        }

        public void OnDrag(PointerEventData data)
        {
            transform.position = data.position + _offset;
        }

        public void OnPointerEnter(PointerEventData pointerEventData)
        {
            if (!_isHold)
            {
                _hasMoved = true;
                transform.position = transform.position + new Vector3(0.0f, 40.0f, 0.0f);
            }
        }

        public void OnPointerExit(PointerEventData pointerEventData)
        {
            if (!_isHold)
            {
                transform.position = transform.position - new Vector3(0.0f, 40.0f, 0.0f);
                _hasMoved = false;
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _isHold = false;
            _hasMoved = false;
        }
    }
}
