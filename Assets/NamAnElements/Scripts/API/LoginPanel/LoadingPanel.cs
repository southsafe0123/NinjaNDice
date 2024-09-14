using UnityEngine;

public class LoadingPanel : MonoBehaviour
{
    public static LoadingPanel Instance;
    private void Awake()
    {
        Instance = this;
    }
    public void SetDisplayLoading(bool isDisplay)
    {
        foreach (Transform child in gameObject.transform)
        {
            child.gameObject.SetActive(isDisplay);
        }
    }
    public bool IsDisplaying()
    {
        return gameObject.transform.GetChild(0).gameObject.activeSelf;
    }
}