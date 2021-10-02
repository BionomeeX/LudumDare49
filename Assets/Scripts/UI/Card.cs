using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Unstable.UI
{
    // Movements from https://github.com/Xwilarg/HadipoRun/blob/master/Assets/Scripts/Controller/DragDropController.cs
    public class Card : MonoBehaviour, IPointerDownHandler, IDragHandler
    {
        [SerializeField]
        private Text _title;

        public void Init(Model.Card card)
        {
            _title.text = card.Name;
        }

        // Drag and drop
        private Vector2 pointerOffset;
        private RectTransform canvasRectTransform;
        private RectTransform panelRectTransform;

		private void Start()
		{
			Canvas canvas = GetComponentInParent<Canvas>();
			if (canvas != null)
			{
				canvasRectTransform = canvas.transform as RectTransform;
				panelRectTransform = transform as RectTransform;
			}
		}

		public void OnPointerDown(PointerEventData data)
		{
			panelRectTransform.SetAsLastSibling();
			RectTransformUtility.ScreenPointToLocalPointInRectangle(panelRectTransform, data.position, data.pressEventCamera, out pointerOffset);
		}

		public void OnDrag(PointerEventData data)
		{
			if (panelRectTransform == null)
				return;

			Vector2 pointerPosition = data.position;

			Vector2 localPointerPosition;
			if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
				canvasRectTransform, pointerPosition, data.pressEventCamera, out localPointerPosition
				))
			{
				panelRectTransform.localPosition = localPointerPosition - pointerOffset;
			}
		}
	}
}
