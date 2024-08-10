using UnityEngine;

public class InvitePanel : MonoBehaviour
{
    public GameObject inviteContent;
    public GameObject invitePrefab;
    private void OnEnable()
    {
        UI_Controller.Instance.inviteContent = inviteContent;
        UI_Controller.Instance.invitePrefab = invitePrefab;
    }
}