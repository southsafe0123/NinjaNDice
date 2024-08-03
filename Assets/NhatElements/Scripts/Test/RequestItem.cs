
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEditor.PackageManager.Requests;

public class RequestItem : MonoBehaviour
{

    [SerializeField] private TMP_Text username;
    [SerializeField] private RawImage avatar;

    [SerializeField] private request rq;

    public void SetData(string username)
    {
        this.username.text = username;
        // this.avatar.texture = avatar;

    }

    public void setRequest(request rq)
    {
        this.rq = rq;
    }

    public void Accept()
    {
        if (rq == null)
        {
            Debug.LogError("Request is null");
            return;
        }
        //send request to server
        StartCoroutine(ApiHandle.Instance.AcceptFriend(rq));
        Destroy(gameObject, 0.1f);
    }

    public void Decline()
    {
        if (rq == null)
        {
            Debug.LogError("Request is null");
            return;
        }
        //send request to server
        StartCoroutine(ApiHandle.Instance.DeclineFriend(rq));
        Destroy(gameObject, 0.1f);
    }
}
