// using Unity.Tutorials.Core.Editor;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

public class LoginChecker : MonoBehaviour
{
    private GameObject loginPanel;
    public void CheckLogin()
    {
        loginPanel = FindObjectOfType<LoginPanel>(true).gameObject;
        if (UserSessionManager.Instance._id.IsNullOrEmpty())
        {
            loginPanel.SetActive(true);

        }

    }
}