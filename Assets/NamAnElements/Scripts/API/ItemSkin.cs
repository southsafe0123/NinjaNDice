using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

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
    private void OnEnable()
    {
        CheckDisplay();
    }
    private void Start()
    {
        CheckDisplay();
    }

    public void CheckDisplay()
    {

        if (ApiHandle.Instance == null) return;
        if (ApiHandle.Instance.user.avatar.IsNullOrEmpty())
        {
            if(skinId == "Default")
            {
                btnEquip.interactable = false;
                btnEquip.GetComponentInChildren<TextMeshProUGUI>().text = "Equiped";
            }
        }
        else
        {
            if (skinId == SkinPool.instance.GetSkin_Id(int.Parse(ApiHandle.Instance.user.avatar)))
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
        
    }

    public void OnClickEquipSkin()
    {
        StartCoroutine(EquipCoroutine());

    }

    private IEnumerator EquipCoroutine()
    {
        ApiHandle.Instance.SetSkinButton(SkinPool.instance.GetSkin_Slot(skinId).ToString());
        yield return new WaitUntil(() => !LoadingPanel.Instance.IsDisplaying());
        PlayerSkin.instance.UpdateSkin();
        GameObject.Find("SkinPanelGroup").SetActive(false);
        
    }
}
