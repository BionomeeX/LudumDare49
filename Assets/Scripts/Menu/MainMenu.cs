using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Unstable.Data;

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

        [SerializeField]
        private MeetingRoom _meetingRoom;

        [SerializeField]
        private Image _checkboxSanity, _checkboxTutorial;

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

            foreach (var l in _meetingRoom.LeadersImages)
            {
                l.Sprite.gameObject.SetActive(GlobalData.Instance.EndingsData.Contains(l.Trigram));
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
            GlobalData.Instance.Save();
        }

        public void DisplaySanity()
        {
            GlobalData.Instance.DisplaySanity = !GlobalData.Instance.DisplaySanity;
            _checkboxSanity.sprite = GlobalData.Instance.DisplaySanity  ? _checked : _notChecked;
        }

        public void SkipTutorial()
        {
            GlobalData.Instance.SkipTutorial = !GlobalData.Instance.SkipTutorial;
            _checkboxTutorial.sprite = GlobalData.Instance.SkipTutorial ? _checked : _notChecked;
        }
    }
}
