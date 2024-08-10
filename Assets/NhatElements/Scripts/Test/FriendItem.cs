
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

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
                    btnInteract.interactable = false;
                }
                else
                {
                    btnInteract.interactable = true;
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
        Debug.Log("Clicked Invite");
        if (NetworkLobby.txtRoomCode.text != "")
        {
            Debug.Log("sending roomcode" + NetworkLobby.txtRoomCode.text);
            ApiHandle.Instance.GetComponent<WS_Client>()?.SendButton(_id, NetworkLobby.txtRoomCode.text);
        }
        else
        {
            StartCoroutine(HostServer());
            Debug.Log("sending roomcode" + NetworkLobby.txtRoomCode.text);
            
            Debug.Log("Host first, invite after");
           
        }
    }

    private IEnumerator HostServer()
    {
        yield return StartCoroutine(PlayButton.Instance.HostCoroutine());
        yield return new WaitUntil(() => NetworkLobby.txtRoomCode.text != "");
        ApiHandle.Instance.GetComponent<WS_Client>()?.SendButton(_id, NetworkLobby.txtRoomCode.text);
    }
}
