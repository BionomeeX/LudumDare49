using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Unstable.Menu
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _deckData;

        [SerializeField]
        private Sprite _notChecked, _checked;

        [SerializeField]
        private DeckCheckbox[] _checkboxs;

        public void Play()
        {
            SceneManager.LoadScene("Main");
        }

        private void Start()
        {
            UpdateCardsCount();
            foreach (var c in _checkboxs)
            {
                c.CountInfo.text = "Cards count: " + GlobalData.Instance.GetCardsCount(c.Name);
            }
        }

        public void UpdateCardsCount()
        {
            _deckData.text = GlobalData.Instance.GetCardsCount();
        }

        public void ToggleDeck(string deck)
        {
            var c = _checkboxs.FirstOrDefault(x => x.Name.ToUpperInvariant() == deck.ToUpperInvariant());
            c.Checkbox.sprite = GlobalData.Instance.ToggleDeck(c.Name) ? _checked : _notChecked;
        }
    }
}
