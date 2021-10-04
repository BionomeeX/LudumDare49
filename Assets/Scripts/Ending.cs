using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unstable.Model;

namespace Unstable
{
    public class Ending : MonoBehaviour
    {
        [SerializeField]
        private GameObject _panel;

        [SerializeField]
        private Image _image;

        [SerializeField]
        private TMP_Text _text;

        public void LoadEnding(Leader leader, Sprite ending)
        {
            _panel.SetActive(true);

            _image.sprite = ending;
            _text.text = string.Join("\n", leader.Endings);
        }
    }
}
