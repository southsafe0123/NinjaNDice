using System.Collections.Generic;
using UnityEngine;

public class SkinPool : MonoBehaviour
{
    public static SkinPool instance;
    public List<GameObject> skinList = new List<GameObject>();
    public GameObject skinContent;
    public GameObject shopContent;
    public GameObject defaultSkin;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }

    public void CallUpdateSkinPool()
    {
        skinList.Clear();
        foreach (Transform skinItem in skinContent.transform)
        {
            Destroy(skinItem.gameObject);
        }
        GameObject clone = Instantiate(defaultSkin, skinContent.transform);
        skinList.Add(clone);
        foreach (Transform shopItem in shopContent.transform)
        {
           GameObject clone1 = Instantiate(shopItem.gameObject, skinContent.transform);
           skinList.Add(clone1);
        }

        PlayerSkin.instance.UpdateSkin();
    }

    public ItemShop GetSkin(string skinID)
    {
        foreach (Transform skinItem in skinContent.transform)
        {
            if (skinItem.GetComponent<ItemShop>().skinId == skinID)
            {
                return skinItem.GetComponent<ItemShop>();
            }
        }
        return GetSkin(0);
    }
    public ItemShop GetSkin(int skinSlot)
    {
        return skinList[skinSlot].GetComponent<ItemShop>() == null? skinList[0].GetComponent<ItemShop>() : skinList[skinSlot].GetComponent<ItemShop>();
    }
    public string GetSkin_Id(int skinSlot)
    {
        return skinList[skinSlot].GetComponent<ItemShop>()== null? skinList[0].GetComponent<ItemShop>().skinId : skinList[skinSlot].GetComponent<ItemShop>().skinId;
    }

    public int GetSkin_Slot(string skinId)
    {
        for (int i = 0; i < skinList.Count; i++)
        {
            if (skinList[i].GetComponent<ItemShop>().skinId == skinId)
            {
                return i;
            }
        }
        return 0;
    }
}