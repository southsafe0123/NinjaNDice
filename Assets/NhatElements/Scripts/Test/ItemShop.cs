using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Newtonsoft.Json;

public class ItemShop : MonoBehaviour
{
    public string skinId;
    [SerializeField] private TextAsset skinInfoJson;
    public TMP_Text skinName;
    [SerializeField] private TMP_Text skinPrice;
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
        skinName.text = s1.name;
        skinPrice.text = s1.price.ToString();
    }

    public void BuySkin()
    {
        CheckBuySkinPanel.instance.DisplayCheck(true);
        CheckBuySkinPanel.instance.SetDataSkinConfirmBuy(ApiHandle.Instance.user._id, skinId);
    }




}
