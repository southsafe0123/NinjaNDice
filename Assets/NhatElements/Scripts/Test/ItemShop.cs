using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Newtonsoft.Json;
using UnityEngine.UI;

public class ItemShop : MonoBehaviour
{
    public string skinId;
    public string skinName;
    public Sprite skinData;
    public Sprite skinAvatar;
    public TMP_Text txtSkinName;
    [SerializeField] private TMP_Text txtSkinPrice;
    // Start is called before the first frame update
    // void Start()
    // {

    // }

    public void UpdateData(skin s1,string skinId)
    {
        this.skinId = skinId;
        DisplayData(s1);
    }

    public void DisplayData(skin s1)
    {
        skinName = s1.name;
        txtSkinName.text = s1.name;
        txtSkinPrice.text = s1.price.ToString()+"G";
    }

    public void BuySkin()
    {
        CheckBuySkinPanel.instance.DisplayCheck(true);
        CheckBuySkinPanel.instance.SetDataSkinConfirmBuy(ApiHandle.Instance.user._id, skinId);
    }

}
