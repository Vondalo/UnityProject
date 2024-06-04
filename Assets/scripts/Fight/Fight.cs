using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fight : FightBaseClass
{

    [Header("Text")]
    [SerializeField] private string Name;
    [SerializeField] private string Description;

    [Header("Health")]
    [SerializeField] private int HP;

    [Header("Music")]
    [SerializeField] private AudioClip sound;
    private void Awake()
    {
        attacks = transform.GetComponentsInChildren<Attack>();

    }
    private void OnEnable()
    {
        SoundManager.instance.PlaySound(sound);


    }
}
