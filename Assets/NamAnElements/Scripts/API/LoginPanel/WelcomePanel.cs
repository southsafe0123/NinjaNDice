using QFSW.QC;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

public class WelcomePanel : MonoBehaviour
{
    public static WelcomePanel instance;
    public GameObject welcomePanel;
    public TextMeshProUGUI txtNewName;
    public Button btnConfirm;
    private void Awake()
    {
        instance = this;

        btnConfirm.onClick.AddListener(() =>
        {
            if (txtNewName.text.IsNullOrEmpty()) return;
            LoginManager loginManager = ApiHandle.Instance.GetComponent<LoginManager>();
            Debug.Log("id " + loginManager.PlayerProfile.playerInfo.Id + " name " + txtNewName.text);
            ApiHandle.Instance.Loginid(loginManager.PlayerProfile.playerInfo.Id, txtNewName.text, "Unity");
        });
    }
    public void DisplayPanel(bool isDisplay)
    {
        welcomePanel.SetActive(isDisplay);
    }
}
