using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class Item : MonoBehaviour
{
    
}
public class PlayerItem : MonoBehaviour
{
    public class PlayerItemHave
    {
        public int amount;
        public ItemBase item;
    }

    public List<PlayerItemHave> itemsPlayerHave = new List<PlayerItemHave>();
}

public abstract class ItemBase: MonoBehaviour
{
    public int id;
    public string itemName;
    public TextField description;
    public string icon;

    public abstract void Effect();
}

public class ItemAttack: ItemBase
{
    public float indexPushBack;
    public Player targetPlayer;
    public Player castedPlayer;

    public override void Effect()
    {
        targetPlayer.currentPos.Value--;
        GameManager.Singleton.TeleportPlayer(targetPlayer, -1);
        castedPlayer.isPlayerTurn.Value = false;
    }
}