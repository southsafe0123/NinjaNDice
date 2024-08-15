using System;
using System.Collections;
using TMPro;
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
    private void Start()
    {
        StartCoroutine(SlowUpdateNameCoroutine());
        btnConfirmLogout.GetComponent<Button>().onClick.AddListener(() =>
        {
            StartCoroutine(LogoutCoroutine());
        });

        btnChangeNameConfirm.onClick.AddListener(() =>
        {
            if (guestName.text.IsNullOrEmpty()) return;
            PlayerPrefs.SetString("GuestName",guestName.text);
        });
    }
    private IEnumerator SlowUpdateNameCoroutine()
    {
        WaitForSeconds wait015fsecond = new WaitForSeconds(0.15f); 
        while (true)
        {
            yield return null;
            yield return wait015fsecond;
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
            txtPlayerName.text = UserSessionManager.Instance.username.IsNullOrEmpty() ? PlayerPrefs.GetString("GuestName") : UserSessionManager.Instance.username.ToString();
        }
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
}
