
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public enum dir { up, down, right, left }

public class Door : NPC_Controller
{
    [Header("Door Settings")]
    [SerializeField] private int Offset;
    [SerializeField] private bool isLocked;
    [SerializeField] private GameObject? dialogueDoor;
    public GameObject connectedDoor;  // Reference to the connected door
    public GameObject connectedRoom;  // Reference to the connected room
    public static List<(Vector3 doorPosition, Vector3 teleportPosition)> teleportationDetails = new List<(Vector3 doorPosition, Vector3 teleportPosition)>();
    public dir direction;
    private FadeScript fadeScript;

    void Start()
    {
        fadeScript = FindObjectOfType<FadeScript>();

        GameObject uiLayer = GameObject.Find("Dialogues");
        GameObject dialogueSpawn = Instantiate(dialogueDoor, uiLayer.transform, false);

        dialogue = dialogueSpawn;
        dialogue.SetActive(false);

        if (connectedDoor == null)
        {
            Destroy(this.gameObject);
        }
    }

   
   

    void PassThrough(GameObject player)
    {
        Debug.Log("PassThrough invoked");
        StartCoroutine(fadeScript.FadeOutAndIn(() =>
        {
            Debug.Log("FadeOutAndIn complete");
            if (connectedDoor != null && player != null)
            {
                GameObject cam = GameObject.FindGameObjectWithTag("MainCamera");
                Vector3 newPosition = Vector3.zero;
                Vector3 doorPos = new Vector3(connectedDoor.transform.position.x, connectedDoor.transform.position.y, player.transform.position.z);
                switch (connectedDoor.GetComponent<Door>().direction)
                {
                    case dir.up:
                        newPosition = doorPos + new Vector3(0, -Offset, 0);
                        break;
                    case dir.down:
                        newPosition = doorPos + new Vector3(0, Offset, 0);
                        break;
                    case dir.right:
                        newPosition = doorPos + new Vector3(-Offset, 0, 0);
                        break;
                    case dir.left:
                        newPosition = doorPos + new Vector3(Offset, 0, 0);
                        break;
                }

               
                cam.transform.position = newPosition;
                player.transform.position = newPosition;
                    
                   
                
            }
            else
            {
                Debug.LogWarning("Connected door or player is null");
            }
        }));
    }

    public override void ActivateDialogue(GameObject player)
    {
        Debug.Log("ActivateDialogue called");
        if (isLocked)
        {
            dialogue.SetActive(true);
        }
        else
        {
            Debug.Log("Teleport");
            PassThrough( player);
        }
    }
}
