using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using Unity.Tutorials.Core.Editor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LoginPanel : MonoBehaviour
{
    public TMP_InputField usernameLogin;
    public TMP_InputField passwordLogin;
    public Button btnConfirm;
    private void Awake()
    {
        btnConfirm.onClick.AddListener(() =>
        {
            ApiHandle.Instance.LoginButton(usernameLogin, passwordLogin);
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
