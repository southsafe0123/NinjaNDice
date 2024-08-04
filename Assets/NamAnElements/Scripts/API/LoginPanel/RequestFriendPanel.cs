using UnityEngine;

public class RequestFriendPanel : MonoBehaviour
{
    public GameObject requestContent;
    public GameObject requestPrefab;
    private void OnEnable()
    {
        UI_Controller.Instance.requestContent = requestContent;
        UI_Controller.Instance.requestPrefab = requestPrefab;
        StartCoroutine(ApiHandle.Instance.GetAllRequestname(ApiHandle.Instance.user.request));
    }
    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
