using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ItemShop : MonoBehaviour
{

    [SerializeField] private TextAsset skinInfoJson;
    [SerializeField] private TMP_Text skinName;
    [SerializeField] private TMP_Text skinPrice;
    // Start is called before the first frame update
    void Start()
    {
        SkinInfo skinInfo = JsonUtility.FromJson<SkinInfo>(skinInfoJson.text);
        skinName.text = skinInfo.name;
        skinPrice.text = skinInfo.price.ToString();
    }

}
