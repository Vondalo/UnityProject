using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitSettings : MonoBehaviour
{
    
    void Start()
    {
        if(PlayerPrefs.HasKey("Volume"))
        AudioListener.volume = PlayerPrefs.GetFloat("Volume")/100;


    }

    
}
