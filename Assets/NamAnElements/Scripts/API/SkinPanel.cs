using UnityEngine;

public class SkinPanel: MonoBehaviour
{
    public static SkinPanel instance;
    public GameObject skinContent;
    public GameObject skinPrefab;
    private void Awake()
    {
        instance = this;
    }
    private void OnEnable()
    {
        UI_Controller.Instance.skinContent = skinContent;
        UI_Controller.Instance.skinPrefab = skinPrefab;
        UI_Controller.Instance.UpdateSkin();

        if (skinContent.transform.childCount == 1)
        {
            PrefsData.SetData(PrefsData.PLAYER_SKIN_ID, "Default");
        }
    }
}
