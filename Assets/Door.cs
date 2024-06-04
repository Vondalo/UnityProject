using DialogueSystem;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Door : NPC_Controller
{
    
    [SerializeField] bool isLocked;
    [SerializeField] GameObject? dialogueDoor;

   void Start()
    {
        
       
        GameObject uiLayer = GameObject.Find("Dialogues");
        GameObject dialogueSpawn =Instantiate(dialogueDoor, uiLayer.transform, false);
        
        dialogue = dialogueSpawn;
        dialogue.SetActive(false);

        
    }
    public  override void ActivateDialogue()
    {
        if(isLocked)
        {
            dialogue.SetActive(true);
        }
        else
        {
            Debug.Log("Teleport");
        }


    }


}
