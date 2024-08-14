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
        playerItemList.Add(new DataItemPlayerOwn(itemName, amount));
    }
    public void SetPlayerItemAmountInList(string itemName, int amount)
    {
        DataItemPlayerOwn item = playerItemList.First(item => item.itemName == itemName);
        item.amount = amount;
        if(item.amount == 0) playerItemList.Remove(item);
    }
    public void UseItem(Player targetPlayer)
    {
        GetCurrentPlayerItem().targetPlayer = targetPlayer;
        GetCurrentPlayerItem().Effect();
        DataItemPlayerOwn item = playerItemList.First(item => item.itemName == GetCurrentPlayerItem().itemName);
        SetPlayerItemAmountInList(item.itemName,item.amount - 1);
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
    
    public void GivePlayerRandomItem()
    {
        int randomItemIndex = UnityEngine.Random.Range(0, fullItemList.Count);
        int amount = 1;
        ItemBase item = fullItemList[randomItemIndex].GetComponent<ItemBase>();
        AddPlayerItemToList(item.itemName, amount);
    }

    public void SavePlayerItemBeforeChangeScene()
    {
        try
        {
            playerOwner.GetComponent<PlayerItem>().playerItemList= playerItemList;

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
