using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

public class SettingPanel : MonoBehaviour
{
    public TextMeshProUGUI txtPlayerName;
    public GameObject btnLogin;
    public GameObject btnLogout;
    public GameObject btnConfirmLogout;
    public Button btnChangeNameConfirm;
    public TextMeshProUGUI guestName;
    public GameObject changeNamePanel;
    private void Start()
    {
        btnConfirmLogout.GetComponent<Button>().onClick.AddListener(() =>
        {
            StartCoroutine(LogoutCoroutine());
        });

        btnChangeNameConfirm.onClick.AddListener(() =>
        {
            if (guestName.text.IsNullOrEmpty()) return;

            if (UserSessionManager.Instance.username.IsNullOrEmpty())
            {
                PlayerPrefs.SetString("GuestName",guestName.text);
                changeNamePanel.SetActive(false);
            }
            else
            {
                ApiHandle.Instance.ChangeNameButton(guestName.text);
                LoadingPanel.Instance.SetDisplayLoading(true);
                changeNamePanel.SetActive(false);
            }
        });
    }
    private void Update()
    {
        if (UserSessionManager.Instance._id.IsNullOrEmpty())
        {
            btnLogin.SetActive(true);
            btnLogout.SetActive(false);
        }
        else
        {
            btnLogin.SetActive(false);
            btnLogout.SetActive(true);
        }
        txtPlayerName.text = UserSessionManager.Instance.username.IsNullOrEmpty() ? PlayerPrefs.GetString("GuestName") : ApiHandle.Instance.user.nameingame.ToString();
    }

    private IEnumerator LogoutCoroutine()
    {
        LoadingPanel.Instance.SetDisplayLoading(true);
        yield return null;
        Destroy(ApiHandle.Instance.GetComponent<WS_Client>());
        UserSessionManager.Instance.ClearSession();
        ApiHandle.Instance.user = null;
        Destroy(ApiHandle.Instance.GetComponent<LoginManager>());
        PlayerPrefs.DeleteKey("username");
        PlayerPrefs.DeleteKey("unityId");
        PlayerPrefs.DeleteKey("password");
        yield return new WaitForSeconds(UnityEngine.Random.Range(0.3f, 1.3f));
        ApiHandle.Instance.AddComponent<LoginManager>();
        yield return new WaitUntil(() => UserSessionManager.Instance.username.IsNullOrEmpty());
        LoadingPanel.Instance.SetDisplayLoading(false);
    }
}
