using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;




public class NetworkManagerOwn : NetworkManager
{
    [SerializeField] private int minPlayers = 2;
    [Scene][SerializeField] private string menuScene = string.Empty;
    public static event Action OnClientConnected;
    public static event Action OnClientDisconnected;

    public List<string> PlayerNames = new List<string>();

    public List<NetworkRoomPlayerLobby> RoomPlayers { get; } = new List<NetworkRoomPlayerLobby>();
    public List<NetworkGamePlayerLobby> GamePlayers { get; } = new List<NetworkGamePlayerLobby>();


    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {

        // Check if there's already a player for this connection
        if (conn.identity != null)
        {
            Debug.LogWarning("Trying to add player, but player object already exists for this connection.");
            return;
        }

        // Add the player for the connection
        GameObject player = Instantiate(playerPrefab);

        // Assign local player authority
        NetworkServer.AddPlayerForConnection(conn, player);
        player.GetComponent<NetworkIdentity>().AssignClientAuthority(conn);
    }

    
    public override void OnClientConnect()
    {
        
        base.OnClientConnect();
        Debug.Log("Connected to server");
        Debug.Log("Client ID: " + NetworkClient.connection.connectionId);
        Debug.Log(PlayerNames);
        OnClientConnected?.Invoke();
        
        
        SwitchToMapGenDemoScene();
        //SceneManager.LoadScene("NetworkTestConnect");
        
    }
    
    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();
        OnClientDisconnected?.Invoke();
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        base.OnServerSceneChanged(sceneName);
        var player = Instantiate(playerPrefab);
        NetworkServer.Spawn(player);

    }
    public void SwitchToMapGenDemoScene()
    {
        if (isNetworkActive)
        {
            ServerChangeScene("NetworkTestConnect");


            
        }
        else
        {
            Debug.LogError("Network is not active. Cannot change scene.");
        }
    }

    //public override void ServerChangeScene(string newSceneName)
    //{
    //    // From menu to game
    //    if (SceneManager.GetActiveScene().name == menuScene && newSceneName.StartsWith("Scene_Map"))
    //    {
    //        for (int i = RoomPlayers.Count - 1; i >= 0; i--)
    //        {
    //            var conn = RoomPlayers[i].connectionToClient;
    //            var gameplayerInstance = Instantiate(gamePlayerPrefab);
    //            gameplayerInstance.SetDisplayName(RoomPlayers[i].DisplayName);

    //            NetworkServer.Destroy(conn.identity.gameObject);

    //            NetworkServer.ReplacePlayerForConnection(conn, gameplayerInstance.gameObject);
    //        }
    //    }

    //    base.ServerChangeScene(newSceneName);
    //}

}      

 
