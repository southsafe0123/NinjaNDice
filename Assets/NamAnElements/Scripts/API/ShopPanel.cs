using UnityEngine;

public class ShopPanel : MonoBehaviour
{
    public static ShopPanel instance;
    public GameObject shopContent;

    private void Awake()
    {
        instance = this;
    }
}
