
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

public class LoginChecker : MonoBehaviour
{
    public static LoginChecker instance;
    public List<GameObject> objectCheckLogins = new List<GameObject>();
    private void OnEnable()
    {
        CheckLogin();
    }
    private void Awake()
    {
        instance = this;
    }

    public void CheckLogin()
    {
        foreach (var obj in objectCheckLogins)
        {
            if (UserSessionManager.Instance._id.IsNullOrEmpty())
            {
                obj.SetActive(false);
            }
            else
            {
                obj.SetActive(true);
            }
        }
    }
}