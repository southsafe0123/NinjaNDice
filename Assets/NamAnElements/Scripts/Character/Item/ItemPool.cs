using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class ItemPool : NetworkBehaviour
{
    public static ItemPool Instance;

    public List<GameObject> fullItemList = new List<GameObject>();
    public List<DataItemPlayerOwn> playerItemList = new List<DataItemPlayerOwn>();
    [System.Serializable]
    public class DataItemPlayerOwn
    {
        public ItemBase item;
        public int amount;

        public DataItemPlayerOwn(ItemBase item, int amount)
        {
            this.item = item;
            this.amount = amount;
        }
    }
    public Player playerOwner;
    public int itemIndex;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        playerOwner = NetworkManager.LocalClient.PlayerObject.GetComponent<Player>();
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

    public void AddPlayerItemToList(ItemBase item, int amount)
    {
        playerItemList.Add(new DataItemPlayerOwn(item, amount));
    }
    public void SetPlayerItemAmountInList(string itemName, int amount)
    {
        DataItemPlayerOwn item = playerItemList.First(item => item.item.itemName == itemName);
        item.amount = amount;
        if(item.amount == 0) playerItemList.Remove(item);
    }
    public DataItemPlayerOwn GetPlayerItem(string itemName)
    {
        try
        {
            return playerItemList.First(item => item.item.itemName == itemName);
        }
        catch (System.Exception)
        {
            Debug.LogError("dont see any dataItem");
            return null;
        } 
    }
    public void UseItem(Player targetPlayer)
    {
        GetCurrentPlayerItem().item.targetPlayer = targetPlayer;
        GetCurrentPlayerItem().item.Effect();
        SetPlayerItemAmountInList(GetCurrentPlayerItem().item.itemName, GetCurrentPlayerItem().amount - 1);
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
    public DataItemPlayerOwn GetCurrentPlayerItem()
    {
        try
        {
            return playerItemList[itemIndex];
        }
        catch (System.Exception)
        {
            Debug.LogError("cant display item");
            return null;
        }
    }
    
    public void GivePlayerRandomItem()
    {
        int randomItemIndex = UnityEngine.Random.Range(0, fullItemList.Count);
        int amount = 1;
        ItemBase item = fullItemList[randomItemIndex].GetComponent<ItemBase>();
        AddPlayerItemToList(item, amount);
    }

}