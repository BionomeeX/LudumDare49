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

        private bool _magnified = false;
        private bool _in = false;

        public Dictionary<string, int> Effects { get; private set; }

        public static int RefId = 0;
        public int Id { private set; get; }

        private void Start()
        {
            _canvas = (RectTransform)GetComponentInParent<Canvas>().transform;
        }

        public string Trigram { private set; get; }

        public void Init(Model.Card card, FactionInfo _faction)
        {
            Trigram = _faction.Trigram;

            _title.text = card.Name;
            _description.text = card.Effects == null ? "" : string.Join("\n", card.Effects.Select(e => GameManager.Instance.GetEffect(e.Key) + ": " + e.Value));
            name = "Card " + card.Name;

            Effects = card.Effects;

            _background.sprite = _faction.CardBackground;

            Id = RefId++;

            foreach (var el in this.GetComponentsInChildren<TMP_Text>())
            {
                el.raycastTarget = false;
            }
        }

        public void SetTarget(Vector2 pos)
        {
            _target = pos;
        }


        private void FixedUpdate()
        {
            if (!_isHold && !_magnified && transform.localPosition != _target)
            {
                if ((transform.localPosition - _target).magnitude < 5.0f)
                {
                    transform.localPosition = _target;
                }
                else
                {
                    transform.localPosition = Vector3.Slerp(transform.localPosition, _target, .1f);
                }
            } else if (_in && !_isHold && transform.localPosition == _target) {
                _magnified = true;
                transform.localPosition = transform.localPosition + new Vector3(0.0f, ((RectTransform)transform).sizeDelta.y * 0.5f, 0.0f);
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _isHold = true;
            Vector2 localPosition = _canvas.transform.InverseTransformPoint(Camera.main.ScreenToWorldPoint(eventData.position));
            _offset = (Vector2)transform.localPosition - localPosition;
            this.GetComponent<Image>().raycastTarget = false;

        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector2 localPosition = _canvas.transform.InverseTransformPoint(Camera.main.ScreenToWorldPoint(eventData.position));
            transform.localPosition = localPosition + _offset;
            _magnified = false;
        }
        public void OnEndDrag(PointerEventData eventData)
        {
            _isHold = false;
            this.GetComponent<Image>().raycastTarget = true;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            transform.SetAsLastSibling();
            _in = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            foreach (var elem in transform.parent.GetComponentsInChildren<RectTransform>().OrderBy(x => x.transform.position.x))
            {
                elem.SetAsLastSibling();
            }
            _magnified = false;
            _in = false;
        }


        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object other)
        {
            if (other is null)
                return false;
            if (other is Card c)
            {
                return c == this;
            }
            return false;
        }

        public static bool operator ==(UI.Card a, UI.Card b)
        {
            if (a is null)
            {
                if (b is null)
                {
                    return true;
                }
                return false;
            }
            return a.Id == b.Id;
        }

        public static bool operator !=(UI.Card a, UI.Card b)
        {
            return !(a.Id == b.Id);
        }
    }
}
