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

        public void SetTarget(Vector3 pos)
        {
            _target = pos;
        }

        // Drag and drop
        private bool _isHold;
        private Vector2 _offset;

        private void FixedUpdate()
        {
            if (!_isHold)
            {
                transform.position = Vector3.Slerp(transform.position, _target, .1f);
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

        public void OnPointerUp(PointerEventData eventData)
        {
            _isHold = false;
        }
    }
}
