using UnityEngine;
using UnityEngine.UI;

namespace Unstable.UI
{
    public class EventLoader : MonoBehaviour
    {
        [SerializeField]
        private RectTransform _choicesTransform;

        [SerializeField]
        private GameObject _eventPanel;

        [SerializeField]
        private Text _title, _description;

        [SerializeField]
        private GameObject _choicePrefab;

        [SerializeField]
        private int _leftRightMargin;

        [SerializeField]
        private int _BottomMargin;

        [SerializeField]
        private int _interChoiceMargin;

        public void Load(Model.Event e)
        {
            _eventPanel.SetActive(true);
            // Destroy all choices that were still here
            for (int i = 0; i < _choicesTransform.childCount; i++)
            {
                Destroy(_choicesTransform.GetChild(i));
            }

            // Display current info
            _title.text = e.Name;
            _description.text = e.Description;

            int choiceSizeX = (_eventPanel.transform.Width - 2 * _leftRightMargin - _interChoiceMargin * (e.Choices.Length - 1)) / e.Choices.Length;

            //foreach (var choice in e.Choices)
            for (int i = 0; i < e.Choices.Length; ++i)
            {
                var choiceObject = Instantiate(_choicePrefab, _choicesTransform);
                // TODO: Set position
                choiceObject.transform.position = new Vector3(_leftRightMargin + i * (choiceSizeX + _interChoiceMargin), _eventPanel.transform.Height - _BottomMargin - _choicePrefab.transform.Height, 0);
            }
        }

        public void UnLoad()
        {
            _eventPanel.SetActive(false);
        }
    }
}
