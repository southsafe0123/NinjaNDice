using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSkin : MonoBehaviour
{
    public string skinId;
    public string skinName;
    public Sprite avatar;

    private void OnEnable()
    {
        foreach (Transform child in ShopPanel.instance.shopContent.transform)
        {
            var skinInShop = child.GetComponent<ItemShop>();
            if (skinInShop.skinId == skinId)
            {
                skinName = skinInShop.skinName.text;
            }
        }
    }

}
