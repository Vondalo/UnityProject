using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine.SceneManagement;
using System.Net;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using Telepathy;
using System;
using Mirror;
using NetworkManager = Unity.Netcode.NetworkManager;
using UnityEditor.Build;

public class JoinMenu : MonoBehaviour
{
    [SerializeField] private GameObject mapGenPrefab = null;
    private Coroutine connectionTimeoutCoroutine;
    [SerializeField] private GameObject ConnectionFailed = null;
    [SerializeField] private GameObject InvalidNamePopup = null;
    [SerializeField] private GameObject InvalidIPPopup = null;
    [SerializeField] private DefaultNetworkManager networkManager = null;
    [SerializeField] private GameObject PagePanel = null;
    [SerializeField] private TMP_Text client_name = null;
    [SerializeField] private TMP_Text ip_adress = null;
    [SerializeField] private CharacterButtonHandler characterButtonHandler = null;
    [Scene] [SerializeField] private string sceneToLoad = string.Empty;
            

    private int PrefabIndex = 0;
    public void HostLobby()
    {
        string ip = System.Text.RegularExpressions.Regex.Replace(ip_adress.text, "[^0-9.]", "");   // IN Text Mesh Pro ist ein unsichtbares Zeichen dabei, ich habe eine Psychose bekommen
        string name = client_name.text.Trim();

        if (!IsValidIP(ip))  // Überprüft ob die IP gültig ist
        {
            InvalidIPPopup.SetActive(true);
            return;
        }
        if (!CheckNameValidity(name)) return;



        PlayerPrefs.SetInt("prefabIndex", PrefabIndex);   // PlayerPrefs damit ich kein NetworkVariable brauche
        PlayerPrefs.SetString("username", name); 
        var unityTransport = networkManager.GetComponent<UnityTransport>();
        unityTransport.SetConnectionData(ip, 7777);

        networkManager.StartHost();
            
        connectionTimeoutCoroutine = StartCoroutine(ConnectionTimeout(5, "Host"));
      

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private bool IsValidIP(string ip)
    {
        if (string.IsNullOrWhiteSpace(ip)) return false;
        var splitValues = ip.Split('.');
        if (splitValues.Length != 4) return false;
        return splitValues.All(r => byte.TryParse(r, out _));

    }
    // HostLobby und ConnectLobby sind fast identisch, nur dass hier der Server gestartet wird, unnötig aber joa, dirty coding oder so
    public void ConnectLobby()
    {
        string ip = System.Text.RegularExpressions.Regex.Replace(ip_adress.text, "[^0-9.]", "");
        string name = client_name.text.Trim();

        if (!IsValidIP(ip))
        {
            InvalidIPPopup.SetActive(true);
            return;
        }
        if (!CheckNameValidity(name)) return;
        PlayerPrefs.SetInt("prefabIndex", PrefabIndex);
        PlayerPrefs.SetString("username", name); // set name
        var unityTransport = networkManager.GetComponent<UnityTransport>();
        unityTransport.SetConnectionData(ip, 7777);

      

        networkManager.StartClient();
        connectionTimeoutCoroutine = StartCoroutine(ConnectionTimeout(5, "Client"));
        

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

   

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
       
        if (scene.path.ToLower().Normalize() == sceneToLoad.ToLower().Trim().Normalize())
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;

            if (NetworkManager.Singleton.IsServer)
            {
                OnClientConnected(NetworkManager.Singleton.LocalClientId);
                NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            }
        }
    }

    private void OnClientConnected(ulong clientId)
    {
       networkManager.OnClientDisconnectCallback += HandleDisconnect;
        if (NetworkManager.Singleton.IsServer)
        {
            if (networkManager.IsHost && GameObject.Find("MapGenerator(Clone)") == null)
            {
                Debug.Log("Is Host --------------------------------------------------");
                GameObject mapGenInstance = Instantiate(mapGenPrefab);
                mapGenInstance.GetComponent<NetworkObject>().Spawn();

            }


            GameObject player = Instantiate(characterButtonHandler.Prefabs[PlayerPrefs.GetInt("prefabIndex")].characterPrefab); // ich bring mich um
           
            NetworkObject networkObject = player.GetComponent<NetworkObject>();
            networkObject.SpawnAsPlayerObject(clientId);

            var displayPlayerName = player.GetComponent<DisplayPlayerName>();
            displayPlayerName.SetPlayerName(PlayerPrefs.GetString("username"));


            Debug.Log("Before Teleport");
            TeleportPlayerToSpawnServerRpc(new ServerRpcParams(),player);
            //MapGen MapGenerator = GameObject.Find("MapGenerator(Clone)").GetComponent<MapGen>();
            //if (MapGenerator.FinishedGenerationNetworkVariable.Value)
            //{
            //    player.GetComponent<Spawn>().TeleportPlayerToSpawn(player);
            //}
            //else
            //{ 
            //    MapGenerator.FinishedGenerationEvent += () => player.GetComponent<Spawn>().TeleportPlayerToSpawn(player);
            //}
            
        }
        
    }
    [ServerRpc (RequireOwnership =false)]
    public void TeleportPlayerToSpawnServerRpc(ServerRpcParams rpcParams, GameObject player)
    {
        Debug.Log("teleported pplaya");
        GameObject spawn = GameObject.FindGameObjectWithTag("Spawn");
        GameObject cam = Camera.main.gameObject;

        Vector3 newSpawnPos = new Vector3(spawn.transform.position.x, spawn.transform.position.y, player.gameObject.transform.position.z);
        Vector3 newSpawnPosCam = new Vector3(spawn.transform.position.x, spawn.transform.position.y, cam.transform.position.z);
        player.transform.position = newSpawnPos;
        cam.transform.position = newSpawnPosCam;
    }
   
    private void HandleDisconnect(ulong clientId)
    {
        networkManager.Shutdown();
        networkManager.OnClientDisconnectCallback -= HandleDisconnect;
        SceneManager.LoadScene(sceneToLoad);
    }

   

    private bool CheckNameValidity(string name)
    {
        if (name.Length < 3 || name.Length > 10)
        {
            InvalidNamePopup.SetActive(true);
            return false;
        }
        return true;
    }
    
    public void SetPlayerGameObject(int index)
    {

        
        PrefabIndex = index;

    }



    private IEnumerator ConnectionTimeout(float timeout, string mode)
    {
        yield return new WaitForSeconds(timeout);

        if (NetworkManager.Singleton.IsConnectedClient)
        {
            Debug.Log($"{mode} connected successfully.");
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.Log($"{mode} failed to connect within {timeout} seconds.");
            networkManager.Shutdown();

           
            SceneManager.sceneLoaded -= OnSceneLoaded;
            ConnectionFailed.SetActive(true);

        }

        connectionTimeoutCoroutine = null;
    }
}

