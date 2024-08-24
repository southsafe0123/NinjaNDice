
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InviteItem : MonoBehaviour
{
    public TMP_Text username;
    public string inviteCode;
    public RawImage avatar;

    public void SetData(string username, string inviteCode)
    {
        this.username.text = username;
        this.inviteCode = inviteCode;
        // this.avatar.texture = avatar;

    }
    public void Accept()
    {
        Debug.Log("inviteCode: " + inviteCode);
        LobbyGameManager.Instance.ButtonClickJoinServer(inviteCode);
        TMP_InputField joinCode = GameObject.Find("JoinCode").GetComponent<TMP_InputField>();
        joinCode.text = inviteCode;
        Destroy(gameObject);
    }

    public void Decline()
    {
        Destroy(gameObject);
    }
}
