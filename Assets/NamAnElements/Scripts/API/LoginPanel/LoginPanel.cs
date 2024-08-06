using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
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
}
