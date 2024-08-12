using System;
using System.Collections;
using TMPro;
using Unity.Tutorials.Core.Editor;
using UnityEngine;
using UnityEngine.UI;

public class SettingPanel: MonoBehaviour
{
    public TextMeshProUGUI txtPlayerName;
    public GameObject btnLogin;
    public GameObject btnLogout;
    private void Start()
    {
        btnLogout.GetComponent<Button>().onClick.AddListener(() =>
        {
            StartCoroutine(LogoutCoroutine());
        });
    }

    private IEnumerator LogoutCoroutine()
    {
        LoadingPanel.Instance.SetDisplayLoading(true);
        yield return null;
        Destroy(ApiHandle.Instance.GetComponent<WS_Client>());
        UserSessionManager.Instance.ClearSession();
        ApiHandle.Instance.user = null;
        yield return new WaitForSeconds(UnityEngine.Random.Range(0.3f, 1.3f));
        yield return new WaitUntil(() => UserSessionManager.Instance.username.IsNullOrEmpty());
        LoadingPanel.Instance.SetDisplayLoading(false);
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

        txtPlayerName.text = UserSessionManager.Instance.username.IsNullOrEmpty()? "Guest" : UserSessionManager.Instance.username.ToString();
    }
}
