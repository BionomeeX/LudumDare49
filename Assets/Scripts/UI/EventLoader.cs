using UnityEngine;
using UnityEngine.UI;

namespace Unstable.UI
{
    public class EventLoader : MonoBehaviour
    {
        [SerializeField]
        private RectTransform _choicesTransform; // where cards go !

        [SerializeField]
        private RectTransform _eventPanel;

        [SerializeField]
        private Text _title, _description;

        [SerializeField]
        private GameObject _choicePrefab; // card
        private RectTransform _choicePrefabTransform;

        [SerializeField]
        private int _leftRightMargin;

        [SerializeField]
        private int _BottomMargin;

        [SerializeField]
        private int _interChoiceMargin;

        public void Load(Model.Event e)
        {
            _choicePrefabTransform ??= (RectTransform)_choicePrefab.transform;

            _eventPanel.gameObject.SetActive(true);
            // Destroy all choices that were still here
            for (int i = 0; i < _choicesTransform.childCount; i++)
            {
                Destroy(_choicesTransform.GetChild(i).gameObject);
            }

            // Display current info
            _title.text = e.Name;
            _description.text = e.Description;

            var interChoiceSize = (_eventPanel.sizeDelta.x - 2 * _leftRightMargin - _interChoiceMargin * (e.Choices.Length - 1) - e.Choices.Length * _choicePrefabTransform.sizeDelta.x) / (e.Choices.Length + 1);

            for (int i = 0; i < e.Choices.Length; ++i)
            {
                var choiceObject = Instantiate(_choicePrefab, _choicesTransform);
                ((RectTransform)choiceObject.transform).anchoredPosition = new Vector2(
                    _leftRightMargin + interChoiceSize + i * (_choicePrefabTransform.sizeDelta.x + interChoiceSize + _interChoiceMargin),
                    _BottomMargin
                );
                choiceObject.GetComponent<EventChoiceLoader>().Init(e.Choices[i]);
            }
        }

        public void UnLoad()
        {
            _eventPanel.gameObject.SetActive(false);
        }
    }
}
