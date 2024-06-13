using Mirror;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;


public class DefaultNetworkManager : Unity.Netcode.NetworkManager
{
    
   public NetworkLobbyHandler data;
    public void Start()
    {
        data = this.GetComponent<NetworkLobbyHandler>();
       
    }
  
    
   
    
    

   
    


    



}
