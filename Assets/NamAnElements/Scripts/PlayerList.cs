using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class PlayerList : NetworkBehaviour
{
    [System.Serializable]
    public class PlayerOrder
    {
        public int order;
        public Player player;

        public PlayerOrder(int order, Player player)
        {
            this.order = order;
            this.player = player;
        }
    }
    /// /////////////////////////
    public static PlayerList Instance;
    public Dictionary<ulong, Player> playerDic = new Dictionary<ulong, Player>();
    public List<PlayerOrder> playerOrders = new List<PlayerOrder>();
    private Coroutine coroutine;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void SetPlayerDic(ulong clientID, Player player)
    {
        playerDic[clientID] = player;
    }
    public Player GetPlayerDic_Value(ulong clientID)
    {
        if (playerDic.ContainsKey(clientID)) return playerDic[clientID];
        return null;
    }

    public ulong? GetPlayerDic_Key(Player player)
    {
        foreach (var playerInDic in playerDic)
        {
            if (player == playerInDic.Value)
            {
                return playerInDic.Key;
            }
        }
        return null;
    }

    public void ResetPlayerDic()
    {
        playerDic.Clear();
    }

    public void AddPlayerOrder(int order, Player player)
    {
        playerOrders.Add(new PlayerOrder(order, player));
    }

    public void SetPlayerOrder(int newOrder, Player player)
    {
       var query = playerOrders.First(playerOrder => playerOrder.player.ownerClientID.Value == player.ownerClientID.Value);
       query.order = newOrder;
    }

    public List<Player> GetPlayerOrder()
    {
        List<Player> playerOrderList = playerOrders
            .OrderBy(playerOrder => playerOrder.order)
            .Select(playerOrder => playerOrder.player)
            .ToList();
        return playerOrderList;
    }
    public void ResetPlayerOrder()
    {
        int i = 1;
       
        var playerOrdersTemp = playerOrders.Where(item => item.player.gameObject != null|| item.player.isPlayerDoneGame.Value == false).ToList();
        foreach (PlayerOrder playerOrder in playerOrdersTemp)
        {
            playerOrder.order= i;
            Debug.Log("ResetOrder: Player: "+playerOrder.order);
            i++;
        }
        playerOrders = playerOrdersTemp;
    }

    [ClientRpc]
    public void SetPlayerDic_ClientRPC()
    {
        if (IsHost) return;
        coroutine = StartCoroutine(SetPlayerDicCoroutine());
    }

    private IEnumerator SetPlayerDicCoroutine()
    {
        var playerList = GameObject.FindObjectsByType<Player>(sortMode: FindObjectsSortMode.None).ToList();
        Debug.Log("Number of players found: " + playerList.Count);

        yield return new WaitForSeconds(0.3f);
        ResetPlayerDic();
        foreach (Player player in playerList)
        {
            Debug.LogError(player.ownerClientID.Value);
            SetPlayerDic(player.ownerClientID.Value, player);
        }

        coroutine = null;
    }

    [ClientRpc]
    public void ResetPlayerDic_ClientRPC()
    {
        if (IsHost) return;
        ResetPlayerDic();
    }

    [ClientRpc]
    public void ResetPlayerOrder_ClientRPC()
    {
        if (IsHost) return;
        ResetPlayerOrder();
    }


    // chua code truong` hop player disconnect luc dang choi
}
