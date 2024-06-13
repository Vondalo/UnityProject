using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour
{


    
    public void TeleportPlayerToSpawn(GameObject player)
    {
        Debug.Log("teleported pplaya");
        GameObject spawn = GameObject.FindGameObjectWithTag("Spawn");
        GameObject cam = Camera.main.gameObject;
        
        Vector3 newSpawnPos = new Vector3(spawn.transform.position.x, spawn.transform.position.y,player.gameObject.transform.position.z);
        Vector3 newSpawnPosCam = new Vector3(spawn.transform.position.x, spawn.transform.position.y, cam.transform.position.z);
        player.transform.position = newSpawnPos;
        cam.transform.position = newSpawnPosCam;
    }

   
}
