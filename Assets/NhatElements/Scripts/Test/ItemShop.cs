using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Newtonsoft.Json;

public class ItemShop : MonoBehaviour
{

    [SerializeField] private TextAsset skinInfoJson;
    [SerializeField] private TMP_Text skinName;
    [SerializeField] private TMP_Text skinPrice;
    // Start is called before the first frame update
    // void Start()
    // {

    // }

    public void UpdateData(skin s1)
    {


        DisplayData(s1);

    }

    public void DisplayData(skin s1)
    {

        skinName.text = s1.name;
        skinPrice.text = s1.price.ToString();
    }




}
