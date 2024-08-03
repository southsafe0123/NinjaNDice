
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FriendItem : MonoBehaviour
{
    [SerializeField] private TMP_Text username;
    [SerializeField] private TMP_Text status;
    [SerializeField] private RawImage avatar;

    public void SetData(string username, string status)
    {
        this.username.text = username;
        this.status.text = status;
        // this.avatar.texture = avatar;
    }

}
