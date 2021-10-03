using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public Button StartButton;
    public Button OptionButton;
    void Start(){
        StartButton.onClick.AddListener(PlayIsClicked);
        OptionButton.onClick.AddListener(OptionIsClicked);
    }
     
    // Update is called once per frame
    void Update () {
    
    }

    public void PlayIsClicked()
    {
        SceneManager.LoadScene("Main");
    }

    public void OptionIsClicked()
    {
        
    }

    public void ExitIsClicked()
    {
        
    }
}
