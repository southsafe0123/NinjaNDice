using UnityEngine;
using UnityEngine.UI;

public class RequestFriendPanel : MonoBehaviour
{
    public GameObject requestContent;
    public GameObject requestPrefab;
    public Button requestButton;
    private void OnEnable()
    {
        UI_Controller.Instance.requestContent = requestContent;
        UI_Controller.Instance.requestPrefab = requestPrefab;
        StartCoroutine(ApiHandle.Instance.GetAllRequestname(ApiHandle.Instance.user.request));
    }
    private void OnDisable()
    {
        StopAllCoroutines();
        if (requestContent.transform.childCount == 0)
        {
            requestButton.gameObject.SetActive(false);
        }
    }
}
