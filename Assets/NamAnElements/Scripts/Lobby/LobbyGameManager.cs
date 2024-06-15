using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LobbyGameManager : NetworkBehaviour
{
    //this is client ID
    public ulong ownerClientID;

    //this is server data (because clientID is Online and only server have)
    public Dictionary<ulong, GameObject> sv_dicPlayer = new Dictionary<ulong, GameObject>();
    //this is all userdata (because it offline)
    public List<GameObject> playerSlots = new List<GameObject>();
    public GameObject disconnectButton;
    public GameObject startGameButton;
    

    private void Awake()
    {
        startGameButton.SetActive(false);
        disconnectButton.SetActive(false);
    }
    private void Start()
    {
        OnConnectedClient();
        OnDisconnectedClient();
    }

    private void OnConnectedClient()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += clientID =>
        {
            if (!IsHost) return;
            LoadPlayer(clientID);
            SetActiveButton_ClientRPC(true);
            Debug.LogError("userjoin: " + clientID);
        };
    }

    private void OnDisconnectedClient()
    {
        NetworkManager.Singleton.OnClientDisconnectCallback += clientID =>
        {
            if (!IsHost) return;
            UnloadPlayer(clientID);
            Debug.LogError("userout: " + clientID);
        };
    }


    //below is detail of all function mean

    private void LoadPlayer(ulong clientID)
    {
        GameObject emptyPlayerSlot = GetEmptyPlayerSlot();
        if (!emptyPlayerSlot)
        {
            NetworkManager.Singleton.DisconnectClient(clientID);
            return;
        }
        sv_dicPlayer[clientID] = emptyPlayerSlot;
        SetOwnerClientID_ClientRPC(clientID);
        sv_dicPlayer[clientID].SetActive(true);

        foreach (var player in sv_dicPlayer)
        {
            if(player.Value.activeSelf)
            {
                int slotIndex = playerSlots.IndexOf(player.Value);
                SetActiveInClient_ClientRPC(slotIndex, true);
            }
        }
        
    }


    [ClientRpc]
    public void SetActiveInClient_ClientRPC(int slotIndex, bool isActive)
    {
        playerSlots[slotIndex].SetActive(isActive);
    }

    [ClientRpc]
    private void SetOwnerClientID_ClientRPC(ulong clientID)
    {
        if (NetworkManager.Singleton.LocalClientId != clientID) return;
        ownerClientID = clientID;
    }

    private GameObject GetEmptyPlayerSlot()
    {
        foreach (var playerSlot in playerSlots)
        {
            if (!playerSlot.activeSelf) return playerSlot;
        }

        return null;
    }

    [ClientRpc]
    private void SetActiveButton_ClientRPC(bool isActive)
    {

        startGameButton.SetActive(IsHost && sv_dicPlayer.Count > 1);
        disconnectButton.SetActive(isActive);
    }
    private void UnloadPlayer(ulong clientID)
    {
        if (!sv_dicPlayer.ContainsKey(clientID)) return;
        sv_dicPlayer[clientID].SetActive(false);

        foreach (var player in sv_dicPlayer)
        {
            if (player.Key.Equals(clientID))
            {
                int slotIndex = playerSlots.IndexOf(sv_dicPlayer[clientID]);
                SetActiveInClient_ClientRPC(slotIndex, false);
            };
        }
        
        sv_dicPlayer.Remove(clientID);

    }
    [ServerRpc(RequireOwnership = false)]
    void Disconnect_ServerRPC(ulong ownerID)
    {
        UnloadPlayer(ownerID);
        Disconnect_ClientRPC(ownerID);
    }
    [ClientRpc]
    void Disconnect_ClientRPC(ulong ownerID)
    {
        if (NetworkManager.Singleton.LocalClientId != ownerID) return;
        foreach (var playerSlot in playerSlots) playerSlot.SetActive(false);
        NetworkManager.Singleton.Shutdown();
        disconnectButton.SetActive(false);
    }
    /// <summary>
    /// BELOW IS FOR BUTTON CLICK IN CANVAS PANEL COMMAND BUTTON
    /// </summary>
    public void ButtonClickHostServer()
    {
        NetworkLobby.Instance.CreateRelay();
    }
    public void ButtonClickJoinServer(TMP_InputField joinCode)
    {
        NetworkLobby.Instance.JoinRelay(joinCode.text);
    }
    public void ButtonClickDisconnectServer()
    {
        Disconnect_ServerRPC(ownerClientID);
    }
    public void ButtonClickStartGame()
    {
        NetworkManager.Singleton.SceneManager.LoadScene("NamAn", 0);
    }
}
