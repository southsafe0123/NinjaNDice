using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEditor.VersionControl;
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

public class RegisterPanel: MonoBehaviour
{
    public TMP_InputField usernameRegister;
    public TMP_InputField EmailRegister;
    public TMP_InputField passwordRegister;
    public TMP_InputField RepasswordRegister;
    public Button btnConfirm;
    private void Awake()
    {
        btnConfirm.onClick.AddListener(() =>
        {
            ApiHandle.Instance.RegisterButton(usernameRegister, EmailRegister, passwordRegister, RepasswordRegister);
        });
    }

}
