using UnityEngine;

public class SkinPool : MonoBehaviour
{
    public static SkinPool instance;
    public GameObject skinContent;
    public GameObject shopContent;
    public GameObject defaultSkin;
    private void Awake()
    {if(instance == null)
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
        foreach (Transform skinItem in skinContent.transform)
        {
            Destroy(skinItem.gameObject);
        }
        Instantiate(defaultSkin, skinContent.transform);
        foreach (Transform shopItem in shopContent.transform)
        {
            Instantiate(shopItem, skinContent.transform);
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
        return null;
    }
}