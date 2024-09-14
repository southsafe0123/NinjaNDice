using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using WebSocketSharp;

public class LoginPanel : MonoBehaviour
{
    public static LoginPanel instance;
    public TMP_InputField usernameLogin;
    public TMP_InputField passwordLogin;
    public TextMeshProUGUI textUsernameGoogle;
    public Button btnConfirm;
    public Button btnLoginGoogle;
    private void Awake()
    {
        instance = this;
        btnConfirm.onClick.AddListener(() =>
        {
            ApiHandle.Instance.LoginButton(usernameLogin.text, passwordLogin.text);
        });

        btnLoginGoogle.onClick.AddListener(() =>    
        {
            ApiHandle.Instance.GetComponent<LoginManager>().ButtonSignIn();
        });
    }
    private IEnumerator TurnOffLoginCoroutine()
    {
         Debug.Log(UserSessionManager.Instance._id.IsNullOrEmpty());
         yield return new WaitUntil(() => !UserSessionManager.Instance._id.IsNullOrEmpty());
         gameObject.SetActive(false);
    }
    private void OnEnable()
    {
        StartCoroutine(TurnOffLoginCoroutine());
    }
}
