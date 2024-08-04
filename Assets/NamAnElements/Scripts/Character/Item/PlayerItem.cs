using System.Collections.Generic;
using UnityEngine;

public class PlayerItem : MonoBehaviour
{
    public class PlayerItemHave
    {
        public int amount;
        public ItemBase item;
    }

    public List<PlayerItemHave> itemsPlayerHave = new List<PlayerItemHave>();
}
