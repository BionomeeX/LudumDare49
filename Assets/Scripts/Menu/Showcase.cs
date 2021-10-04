using UnityEngine;
using UnityEngine.UI;

namespace Unstable.Menu
{
    public class Showcase : MonoBehaviour
    {
        [SerializeField]
        private Sprite[] Endings;
        [SerializeField]
        private Sprite[] ConceptArts;

        [SerializeField]
        private Image _renderer;

        private int _index;

        private int _target;
        public void Show(int target)
        {
            _target = target;
            _index = 0;
            Show();
        }

        public Sprite[] GetCurrents()
        {
            if (_target == 0) return Endings;
            else return ConceptArts;
        }

        public void Next()
        {
            _index++;
            if (_index >= GetCurrents().Length)
            {
                _index = 0;
            }
            Show();
        }

        public void Previous()
        {
            _index--;
            if (_index <= 0)
            {
                _index = GetCurrents().Length - 1;
            }
            Show();
        }

        private void Show()
        {
            _renderer.sprite = GetCurrents()[_index];
        }
    }
}
