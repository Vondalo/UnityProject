using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawnhandling : MonoBehaviour
{
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

   
    void OnTriggerEnter(Collider other)
    {
        PlayCollisionSound();
    }

   

    void PlayCollisionSound()
    {
        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.Play();
        }
    }
}
