using UnityEngine;

public class FriendPanel : MonoBehaviour
{
    public GameObject friendContent;
    public GameObject friendPrefab;

    private void OnEnable()
    {
        UI_Controller.Instance.friendContent = friendContent;
        UI_Controller.Instance.friendPrefab = friendPrefab;
        UI_Controller.Instance.UpdateFriend();
    }

}
