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
        private Image _checkboxAudio, _checkboxTutorial;

        [SerializeField]
        private TMP_Text _bestScore;

        public void Play()
        {
            SceneManager.LoadScene("Main");
        }

        private void Start()
        {
            foreach (var c in _checkboxs)
            {
                c.CountInfo.text = "Event count: " + GlobalData.Instance.GetCardsCount(c.Name);
            }

            foreach (var l in _meetingRoom.LeadersImages)
            {
                l.Sprite.gameObject.SetActive(GlobalData.Instance.EndingsData.Contains(l.Trigram));
            }

            foreach (var l in GlobalData.Instance.DecksAllowed)
            {
                var c = _checkboxs.FirstOrDefault(x => x.Name.ToUpperInvariant() == l.ToUpperInvariant());
                c.Checkbox.sprite = _checked;
            }

            if (GlobalData.Instance.SkipTutorial)
            {
                _checkboxTutorial.sprite = _checked;
            }

            if (GlobalData.Instance.MuteAudio)
            {
                _checkboxAudio.sprite = _checked;
            }

            _bestScore.text = "Best Score: " + GlobalData.Instance.BestScore.ToString();
            UpdateCardsCount();
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

        public void MuteAudio()
        {
            GlobalData.Instance.MuteAudio = !GlobalData.Instance.MuteAudio;
            _checkboxAudio.sprite = GlobalData.Instance.MuteAudio ? _checked : _notChecked;
            GlobalData.Instance.Save();
        }

        public void SkipTutorial()
        {
            GlobalData.Instance.SkipTutorial = !GlobalData.Instance.SkipTutorial;
            _checkboxTutorial.sprite = GlobalData.Instance.SkipTutorial ? _checked : _notChecked;
            GlobalData.Instance.Save();
        }
    }
}
