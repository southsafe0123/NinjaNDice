using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : NetworkBehaviour
{
    public static GameManager Singleton;
    public NetworkVariable<int> dice = new NetworkVariable<int>();
    public Dictionary<ulong, Player> players = new Dictionary<ulong, Player>();
    public ulong playerIndex;
    public Map map;
    public NetworkManagerUI networkManagerUI;
    private void Awake()
    {
        Singleton = this;
    }
    private void Start()
    {
        dice.OnValueChanged += OnDiceValueChanged;
        NetworkManager.Singleton.OnClientConnectedCallback += OnPlayerConnect;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnPlayerDisconnect;
    }
    private void OnDiceValueChanged(int oldValue, int newValue)
    {
        UpdateDiceUI(newValue);
    }
    private void OnPlayerConnect(ulong clientID)
    {
        if (!IsServer) return;
        Debug.LogError("player " + clientID + " joined");
        var playerInNetwork = NetworkManager.Singleton.ConnectedClients[clientID].PlayerObject.gameObject.GetComponent<Player>();
        players.Add(clientID, playerInNetwork);
        TeleportPlayer(clientID);
    }
    void TeleportPlayer(ulong clientID, int index = 0) // nhat da sua ham nay
    {
        if (index == 0)
        {
            players[clientID].gameObject.transform.position = map.movePos[0].position;
        }
        else
        {

            int newPos = players[clientID].currentPos.Value + index;
            if (newPos >= map.movePos.Count)
            {
                newPos = map.movePos.Count;
            }
            players[clientID].currentPos.Value = newPos;
            players[clientID].gameObject.transform.position = map.movePos[newPos].position;

        }
    }

    private void OnPlayerDisconnect(ulong clientID)
    {
        if (!IsServer) return;
        Debug.LogError("player " + clientID + " joined");
    }

    [ServerRpc(RequireOwnership = false)]
    public void SendRollDiceTo_ServerRPC()
    {
        var diceValue = UnityEngine.Random.Range(1, 7);
        SendRollDiceTo_ClientRPC(diceValue);
    }

    [ClientRpc]
    public void SendRollDiceTo_ClientRPC(int diceValue)
    {
        if (!IsServer) return;
        dice.Value = diceValue;
        TeleportPlayer(OwnerClientId, diceValue); // id cua nguoi roll dice


    }
    private void UpdateDiceUI(int value)
    {
        networkManagerUI.numDiceText.text = value.ToString();
    }

    public void ClickStartGame()
    {
        if (!IsServer) return;
        StartCoroutine(StartGame());
    }

    private IEnumerator StartGame()
    {
        players[playerIndex].SetPlayerTurn(true);
        Debug.LogError("call from host" + OwnerClientId);
        yield return null;
    }

    public void test()
    {

    }

}