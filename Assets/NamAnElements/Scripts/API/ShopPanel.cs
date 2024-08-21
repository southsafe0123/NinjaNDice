using System.Collections;
using UnityEngine;

public class ShopPanel : MonoBehaviour
{
    public static ShopPanel instance;
    public GameObject shopContent;
    public GameObject shopPanelGroup;
    public UpdateSkinsHandle updateSkinsHandle;
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        SkinPool.instance.shopContent = shopContent;
    }
}
