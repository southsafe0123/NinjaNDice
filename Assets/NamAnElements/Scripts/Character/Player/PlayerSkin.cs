using UnityEngine;

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
        try
        {
            spriteRenderer.sprite = SkinPool.instance.GetSkin(PrefsData.GetData(PrefsData.PLAYER_SKIN_ID)).skinData==null? SkinPool.instance.GetSkin("Default").skinData: SkinPool.instance.GetSkin(PrefsData.GetData(PrefsData.PLAYER_SKIN_ID)).skinData;
        }
        catch (System.Exception)
        {

            Debug.LogError("something wrong here? or not :))");
        }
        
    }
}