using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Controller : MonoBehaviour
{

    [SerializeField] public GameObject dialogue;

    public virtual void ActivateDialogue()
    {
        dialogue.SetActive(true);
    }

    public bool dialogueActive()
    {
        return dialogue.activeSelf;
    }
}
