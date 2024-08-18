using UnityEngine;

public class SkinPanel: MonoBehaviour
{
    public GameObject skinContent;
    public GameObject skinPrefab;

    private void OnEnable()
    {
        UI_Controller.Instance.skinContent = skinContent;
        UI_Controller.Instance.skinPrefab = skinPrefab;
        UI_Controller.Instance.UpdateSkin();
    }
}
