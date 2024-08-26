using UnityEngine;
using UnityEngine.UI;

public class RequestFriendPanel : MonoBehaviour
{
    public static RequestFriendPanel instance;
    public GameObject requestContent;
    public GameObject requestPrefab;
    public Button requestButton;
    private void OnEnable()
    {
        UI_Controller.Instance.requestContent = requestContent;
        UI_Controller.Instance.requestPrefab = requestPrefab;
        UI_Controller.Instance.UpdateRequest();
    }

    public void GetRequest()=> StartCoroutine(ApiHandle.Instance.GetAllRequestname(ApiHandle.Instance.user.request));
}
