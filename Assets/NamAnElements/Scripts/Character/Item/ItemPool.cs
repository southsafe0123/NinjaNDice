using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using static ItemPool;

public class ItemPool : NetworkBehaviour
{
    public static ItemPool Instance;

    public List<GameObject> fullItemList = new List<GameObject>();
    public List<DataItemPlayerOwn> playerItemList = new List<DataItemPlayerOwn>();
    public int itemIndex;
    public Player playerOwner;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

    }

    private void Start()
    {
        playerOwner = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Player>();
        playerItemList = playerOwner.GetComponent<PlayerItem>().playerItemList;
    }

    [ContextMenu("UpdateFullItemList")]
    public void UpdateItemList()
    {
        fullItemList.Clear();
        foreach (Transform child in gameObject.transform)
        {
            fullItemList.Add(child.gameObject);
        }
    }

    public void AddPlayerItemToList(string itemName, int amount)
    {
        Debug.Log("===> give item to player");
        bool playerHaveThisItem = playerItemList.FirstOrDefault(item => item.itemName == itemName) != null;
        if (playerHaveThisItem)
        {
            DataItemPlayerOwn thisItem = playerItemList.First(item => item.itemName == itemName);
            thisItem.amount = thisItem.amount + amount;
        }
        else
        {
            playerItemList.Add(new DataItemPlayerOwn(itemName, amount));
        }
    }

    public void SetPlayerItemAmountInList(string itemName, int amount)
    {
        DataItemPlayerOwn item = playerItemList.First(item => item.itemName == itemName);
        item.amount = amount;
        if (item.amount == 0) playerItemList.Remove(item);
    }
    public void UseItem(Player targetPlayer)
    {
        GetCurrentPlayerItem().targetPlayer = targetPlayer;
        GetCurrentPlayerItem().Effect();
        DataItemPlayerOwn item = playerItemList.First(item => item.itemName == GetCurrentPlayerItem().itemName);
        SetPlayerItemAmountInList(item.itemName, item.amount - 1);
        GameManager.Singleton.SetPlayerTurn_ServerRPC(NetworkManager.LocalClientId, false);
    }
    public void OnClickNextItem()
    {
        itemIndex++;
        if (itemIndex > playerItemList.Count - 1) itemIndex = 0;
    }
    public void OnClickPrevItem()
    {
        itemIndex--;
        if (itemIndex < 0) itemIndex = playerItemList.Count - 1;
    }
    public ItemBase GetCurrentPlayerItem()
    {
        try
        {
            var playerItem = fullItemList.First(gameObject => gameObject.GetComponent<ItemBase>().itemName == playerItemList[itemIndex].itemName).GetComponent<ItemBase>();
            return playerItem;
        }
        catch (System.Exception)
        {
            Debug.LogError("cant display item");
            return null;
        }
    }
    public ItemBase GetItemInFullItem(string itemName)
    {
        try
        {
            var playerItem = fullItemList.First(gameObject => gameObject.GetComponent<ItemBase>().itemName == itemName).GetComponent<ItemBase>();
            return playerItem;
        }
        catch (System.Exception)
        {
            Debug.LogError("cant display item");
            return null;
        }
    }
    public int GetCurrentPlayerItemAmount()
    {
        try
        {
            return playerItemList[itemIndex].amount;
        }
        catch (System.Exception)
        {
            Debug.LogError("cant display item amount");
            return 0;
        }
    }
    [ClientRpc]
    public void GivePlayerRandomItem_ClientRPC(ulong clientID)
    {
        if (clientID == NetworkManager.Singleton.LocalClientId)
        {
            int randomItemIndex = UnityEngine.Random.Range(0, fullItemList.Count);
            int amount = 1;
            ItemBase item = fullItemList[randomItemIndex].GetComponent<ItemBase>();

            SendInfoItemToMiniEndGame_ServerRPC(NetworkManager.Singleton.LocalClientId,item.itemName); 
            AddPlayerItemToList(item.itemName, amount);
        }
    }
    [ServerRpc(RequireOwnership =false)]
    private void SendInfoItemToMiniEndGame_ServerRPC(ulong clientID, string itemName)
    {
        if (MiniEndGamePanel.Instance == null) return;
        UpdatePlayerItem_ClientRPC(clientID,itemName);
    }

    [ClientRpc]
    public void UpdatePlayerItem_ClientRPC(ulong playerID, string itemName)
    {
        foreach (PlayerMiniEndGameItem playerItem in MiniEndGamePanel.Instance.playerEndList)
        {
            if (playerItem.player != null && playerItem.player.ownerClientID.Value == playerID)
            {
                ItemBase item = GetItemInFullItem(itemName);
                Debug.Log("player: " + playerID + "/item: " + item.itemName);
                playerItem.itemPlayerGet = item;
            }
        }
    }

    public void SavePlayerItemBeforeChangeScene()
    {
        try
        {
            playerOwner.GetComponent<PlayerItem>().playerItemList = playerItemList;

        }
        catch (System.Exception)
        {
            Debug.LogError("Cant Save Item Data");
        }
    }
    public override void OnDestroy()
    {
        SavePlayerItemBeforeChangeScene();
    }
}
