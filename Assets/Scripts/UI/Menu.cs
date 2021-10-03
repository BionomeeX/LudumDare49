using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class Menu : MonoBehaviour
{
    public Button StartButton;
    public Button OptionButton;
    public Button ReturnButton;
    public GameObject MenuManager;
    public GameObject OptionManager;
    public Slider MusicSlider;
    public Slider SoundEffectSlider;
    public AudioMixer mixer;
    void Start(){
        StartButton.onClick.AddListener(PlayIsClicked);
        OptionButton.onClick.AddListener(OptionIsClicked);
        ReturnButton.onClick.AddListener(ReturnIsClicked);
        MusicSlider.onValueChanged.AddListener(SetMusicLevel);
        MenuManager.SetActive(true);
        OptionManager.SetActive(false);
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
        MenuManager.SetActive(false);
        OptionManager.SetActive(true);
        
    }

    public void SetMusicLevel (float sliderValue)
    {
        // mixer.SetFloat("MusicVol", Mathf.Log10(sliderValue) * 20);
        Debug.Log(sliderValue);
    }

    public void SetSoundEffectLevel (float sliderValue)
    {
        // mixer.SetFloat("SoundEffectVol", Mathf.Log10(sliderValue) * 20);
        Debug.Log(sliderValue);
    }

    public void ReturnIsClicked()
    {
        MenuManager.SetActive(true);
        OptionManager.SetActive(false);
    }    
}
