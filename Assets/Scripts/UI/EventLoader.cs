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

            foreach (var choice in e.Choices)
            {
                var choiceObject = Instantiate(_choicePrefab, _choicesTransform);
                // TODO: Set position
            }
        }

        public void UnLoad()
        {
            _eventPanel.SetActive(false);
        }
    }
}
