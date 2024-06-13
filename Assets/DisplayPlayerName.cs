using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking;
using Unity.Netcode;
using TMPro;
using UnityEditor;
using Unity.Netcode.Transports.UTP;
using Unity.Collections;
using System;

public class DisplayPlayerName : Unity.Netcode.NetworkBehaviour
{
    public NetworkVariable<NetPlayerName> PlayerName = new NetworkVariable<NetPlayerName>(new NetPlayerName() { username = "" });

    public override void OnNetworkSpawn()
    {
        Debug.Log("Spawned");
        base.OnNetworkSpawn();

        if (IsServer)
        {
            PlayerName.OnValueChanged += OnPlayerNameChanged;
            string username = PlayerPrefs.GetString("username");
            PlayerName.Value = new NetPlayerName() { username = username };
        }
        UpdatePlayerNameUI(PlayerName.Value.username.ToString());
    }

    private void OnPlayerNameChanged(NetPlayerName previousValue, NetPlayerName newValue)
    {
        Debug.Log($"Player {OwnerClientId} changed name from {previousValue.username} to {newValue.username}");
        UpdatePlayerNameUI(newValue.username.ToString());
    }

    public void SetPlayerName(string name)
    {
        if (IsServer)
        {
            PlayerName.Value = new NetPlayerName() { username = name };
        }
    }

    private void UpdatePlayerNameUI(string name)
    {
        GetComponentInChildren<TMP_Text>().text = name;
    }
}

public struct NetPlayerName : INetworkSerializable, System.IEquatable<NetPlayerName>
{
    public FixedString64Bytes username;

    public bool Equals(NetPlayerName other)
    {
        return username.Equals(other.username);
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref username);
    }
}
