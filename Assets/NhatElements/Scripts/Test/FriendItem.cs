
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FriendItem : MonoBehaviour
{
    [SerializeField] private TMP_Text username;
    [SerializeField] private TMP_Text status;
    [SerializeField] private RawImage avatar;
    public Sprite mail;
    public Sprite delete;
    public Color mailColor;
    public Color deleteColor;
    public Button btnInteract;
    public Image imageInteract;
    public string _id;

    private void Start()
    {
        switch (SceneManager.GetActiveScene().name)
        {
            case "MenuScene":
                imageInteract.sprite = delete;
                imageInteract.color = deleteColor;
                break;
            case "LobbyScene":
                imageInteract.sprite = mail;
                imageInteract.color = mailColor;

                if (status.text.Contains("offline"))
                {
                    imageInteract.enabled = false;
                }
                else
                {
                    imageInteract.enabled = true;
                }

                break;
        }
        btnInteract.onClick.AddListener(() =>
        {
            switch (SceneManager.GetActiveScene().name)
            {
                case "MenuScene":
                    RemoveFriend();
                    break;
                case "LobbyScene":
                    InviteFriend();
                    break;
            }
        });
    }
    public void SetData(string username, string status, string _id)
    {
        this.username.text = username;
        this.status.text = status;
        this._id = _id;
        // this.avatar.texture = avatar;
    }
    public void RemoveFriend()
    {
        ApiHandle.Instance.DeleteFriendButton(_id);
    }

    public void InviteFriend()
    {
        if (NetworkLobby.txtRoomCode.text != "")
        {
            Debug.Log("sending roomcode");
            ApiHandle.Instance.GetComponent<WS_Client>()?.SendButton(_id, NetworkLobby.txtRoomCode.text);
        }
    }

}

public class InviteItem : MonoBehaviour
{
    public TMP_Text username;
    public RawImage avatar;

    public void SetData(string username)
    {
        this.username.text = username;
        // this.avatar.texture = avatar;

    }
    public void Accept()
    {
       
    }

    public void Decline()
    {
        
    }
}
