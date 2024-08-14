
using UnityEngine;
using TMPro;
using System.Security.Cryptography;
using UnityEngine.UI;

public class SearchFriendItem : MonoBehaviour
{
    [SerializeField] private TMP_Text username;
    public Button btnAddFriend;
    private string _id;
    public void SetData(string username, string _id)
    {
        this.username.text = username;
        this._id = _id;
    }
    public void AddFriend()
    {
        Debug.Log(_id);
        ApiHandle.Instance.AddFriendButton(_id);
    }
}
