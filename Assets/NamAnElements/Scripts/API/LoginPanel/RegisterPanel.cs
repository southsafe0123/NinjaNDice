﻿using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RegisterPanel: MonoBehaviour
{
    public static RegisterPanel instance;
    public TMP_InputField usernameRegister;
    public TMP_InputField EmailRegister;
    public TMP_InputField passwordRegister;
    public TMP_InputField RepasswordRegister;
    public Button btnConfirm;
    private void Awake()
    {
        instance = this;
        btnConfirm.onClick.AddListener(() =>
        {
            ApiHandle.Instance.RegisterButton(usernameRegister, EmailRegister, passwordRegister, RepasswordRegister);
        });
    }

}
