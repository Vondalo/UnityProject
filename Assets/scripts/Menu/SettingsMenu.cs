using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    void Start()
    {
        LoadVolume();
    }
    [SerializeField] private Slider volumeSlider;
    public void SetVolume()
    {
        AudioListener.volume = volumeSlider.value/100;
        PlayerPrefs.SetFloat("Volume", volumeSlider.value);
    }
    public void LoadVolume()
    {
        volumeSlider.value = PlayerPrefs.GetFloat("Volume");
    }
}
