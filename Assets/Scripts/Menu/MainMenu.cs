using UnityEngine;
using UnityEngine.SceneManagement;

namespace Unstable.Menu
{
    public class MainMenu : MonoBehaviour
    {
        public void Play()
        {
            SceneManager.LoadScene("Main");
        }
    }
}
