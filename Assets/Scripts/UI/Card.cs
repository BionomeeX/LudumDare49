using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Unstable.UI
{
    // Movements from https://github.com/Xwilarg/HadipoRun/blob/master/Assets/Scripts/Controller/DragDropController.cs
    public class Card : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        [SerializeField]
        private Text _title;

        private Vector3 _target;

        public void Init(Model.Card card)
        {
            _title.text = card.Name;
        }

        // Drag and drop
        private Vector2 pointerOffset;
        private RectTransform canvasRectTransform;
        private RectTransform panelRectTransform;

        private bool _isHold;

        private void Start()
        {
            Canvas canvas = GetComponentInParent<Canvas>();
            if (canvas != null)
            {
                canvasRectTransform = canvas.transform as RectTransform;
                panelRectTransform = transform as RectTransform;
            }

            _target = transform.position;
        }

        private void FixedUpdate()
        {
            if (!_isHold)
            {
                transform.position = Vector3.Slerp(transform.position, _target, .1f);
            }
        }

        public void OnPointerDown(PointerEventData data)
        {
            panelRectTransform.SetAsLastSibling();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(panelRectTransform, data.position, data.pressEventCamera, out pointerOffset);
        }

        public void OnDrag(PointerEventData data)
        {
            _isHold = true;

            Vector2 pointerPosition = data.position;

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRectTransform, pointerPosition, data.pressEventCamera, out Vector2 localPointerPosition
                ))
            {
                panelRectTransform.localPosition = localPointerPosition - pointerOffset;
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _isHold = false;
        }
    }
}
