using Unity.VisualScripting;
using UnityEngine;
using WebSocketSharp;

public class PlayerSkin : MonoBehaviour
{
    public static PlayerSkin instance;
    public SpriteRenderer spriteRenderer;
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        UpdateSkin();
    }
    public void UpdateSkin()
    {
        if (ApiHandle.Instance == null) return;
        if (ApiHandle.Instance.user.avatar.IsNullOrEmpty())
        {
            spriteRenderer.sprite = SkinPool.instance.GetSkin(0).skinData;
        }
        else
        {
            spriteRenderer.sprite = SkinPool.instance.GetSkin(int.Parse(ApiHandle.Instance.user.avatar)).skinData;
        }



    }
}