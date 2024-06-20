using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : NetworkBehaviour
{
    public static GameManager Singleton;
    public NetworkVariable<int> dice = new NetworkVariable<int>();
    //public Dictionary<ulong, Player> players = new Dictionary<ulong, Player>();
    public List<Player> playerList = new List<Player>();
    public int playerIndex;
    public Map map;
    public NetworkManagerUI networkManagerUI;
    private void Awake()
    {
        Singleton = this;
        playerIndex = 0;

       playerList = GameObject.FindObjectsByType<Player>(sortMode: FindObjectsSortMode.None).ToList();
    }
    private void Start()
    {
        dice.OnValueChanged += OnDiceValueChanged;
       
        //NetworkManager.Singleton.OnClientConnectedCallback += OnPlayerConnect;
        //NetworkManager.Singleton.OnClientDisconnectCallback += OnPlayerDisconnect;
        if (!IsHost) return;
        for (int i = 0; i < playerList.Count; i++)
        {
            playerList[i].gameObject.transform.position = map.movePos[0].position;
        }
        playerList[playerIndex].isPlayerTurn.Value = true;
    }
    
    private void OnDiceValueChanged(int oldValue, int newValue)
    {
        UpdateDiceUI(newValue);
    }
    //private void OnPlayerConnect(ulong clientID)
    //{
    //    if (!IsServer) return;
    //    Debug.LogError("player " + clientID + " joined");
    //    var playerInNetwork = NetworkManager.Singleton.ConnectedClients[clientID].PlayerObject.gameObject.GetComponent<Player>();
    //    players.Add(clientID, playerInNetwork);
    //    TeleportPlayer(clientID);
    //}
    void TeleportPlayer(Player clientPlayer, int index = 0) // nhat da sua ham nay
    {
        if (index == 0)
        {
            clientPlayer.gameObject.transform.position = map.movePos[0].position;
        }
        else
        {

            int newPos = clientPlayer.currentPos.Value + index;
            if (newPos >= map.movePos.Count)
            {
                newPos = map.movePos.Count;
            }
            clientPlayer.currentPos.Value = newPos;
            clientPlayer.gameObject.transform.position = map.movePos[newPos].position;

            clientPlayer.isPlayerTurn.Value = false;
            playerIndex = playerIndex >= playerList.Count-1 ? 0 : playerIndex + 1;
            playerList[playerIndex].isPlayerTurn.Value = true;
        }
    }

    //private void OnPlayerDisconnect(ulong clientID)
    //{
    //    if (!IsServer) return;
    //    Debug.LogError("player " + clientID + " joined");
    //}

    [ServerRpc(RequireOwnership = false)]
    public void SendRollDiceTo_ServerRPC()
    {
        var diceValue = UnityEngine.Random.Range(1, 7);
        SendRollDiceTo_ClientRPC(diceValue);
    }

    [ClientRpc]
    public void SendRollDiceTo_ClientRPC(int diceValue)
    {
        if (!IsHost) return;
        dice.Value = diceValue;
        TeleportPlayer(playerList[playerIndex], diceValue); // id cua nguoi roll dice


    }
    private void UpdateDiceUI(int value)
    {
        networkManagerUI.numDiceText.text = value.ToString();
    }
   

    //public void ClickStartGame()
    //{
    //    if (!IsServer) return;
    //    StartCoroutine(StartGame());
    //}

    //private IEnumerator StartGame()
    //{
    //    players[playerIndex].SetPlayerTurn(true);
    //    Debug.LogError("call from host" + OwnerClientId);
    //    yield return null;
    //}

    //public void test()
    //{

    //}

}