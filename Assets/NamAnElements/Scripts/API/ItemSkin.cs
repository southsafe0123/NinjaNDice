using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSkin : MonoBehaviour
{
    public string skinId;
    public string skinName;
    public Sprite avatar;
    public Sprite skinData;
    public TextMeshProUGUI txtName;
    public Image imgAvatar;
    public bool isSkinEquiped;
    public Button btnEquip;
    public void UpdateInfoSkin()
    {
        foreach (Transform child in ShopPanel.instance.shopContent.transform)
        {
            var skinInShop = child.GetComponent<ItemShop>();
            if (skinInShop.skinId == skinId)
            {
                skinName = skinInShop.skinName;
                avatar = skinInShop.skinAvatar;
                skinData = skinInShop.skinData;

                txtName.text = skinName;
                imgAvatar.sprite = avatar;
            }
        }
    }

    private void Start()
    {
        CheckDisplay();
    }

    public void CheckDisplay()
    {
        if (skinId == PrefsData.GetData(PrefsData.PLAYER_SKIN_ID)) isSkinEquiped = true;
        if (isSkinEquiped)
        {
            btnEquip.interactable = false;
            btnEquip.GetComponentInChildren<TextMeshProUGUI>().text = "Equiped";
        }
        else
        {
            btnEquip.interactable = true;
            btnEquip.GetComponentInChildren<TextMeshProUGUI>().text = "Equip";
        }
    }

    public void OnClickEquipSkin()
    {
        isSkinEquiped = true;
        PrefsData.SetData(PrefsData.PLAYER_SKIN_ID, skinId);
        CheckDisplay();
        PlayerSkin.instance.UpdateSkin();
        if (PrefsData.HaveData(PrefsData.PLAYER_SKIN_ID))
        {
            foreach (Transform skinItem in SkinPanel.instance.skinContent.transform)
            {
                if (skinItem.GetComponent<ItemSkin>().skinId != PrefsData.GetData(PrefsData.PLAYER_SKIN_ID))
                {
                    skinItem.GetComponent<ItemSkin>().isSkinEquiped = false;
                    skinItem.GetComponent<ItemSkin>().CheckDisplay();
                }
            }
        }
    }
}
