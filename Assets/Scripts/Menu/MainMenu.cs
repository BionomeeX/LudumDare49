using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Unstable.Menu
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _deckData;

        public void Play()
        {
            SceneManager.LoadScene("Main");
        }

        private void Start()
        {
            _deckData.text = GlobalData.Instance.GetCardsCount();
        }
    }
}
