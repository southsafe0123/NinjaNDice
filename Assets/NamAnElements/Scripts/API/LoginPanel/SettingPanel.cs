using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

public class SettingPanel : MonoBehaviour
{
    public static SettingPanel instance;
    public TextMeshProUGUI txtPlayerName;
    public GameObject btnLogin;
    public GameObject btnLogout;
    public GameObject btnConfirmLogout;
    public Button btnChangeNameConfirm;
    public TMP_InputField guestName;
    public GameObject changeNamePanel;
    public Sprite defaultImage;
    public Image avatarImage;
    private void Awake()
    {
        instance = this;
    }
    private void OnEnable()
    {
        SetAvatar();
    }
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
                PrefsData.SetData(PrefsData.PLAYER_INGAME_NAME_NOLOGIN, guestName.text);
                changeNamePanel.SetActive(false);
            }
            else
            {
                ApiHandle.Instance.ChangeNameButton(guestName.text.ToString());
                LoadingPanel.Instance.SetDisplayLoading(true);
                changeNamePanel.SetActive(false);
            }
        });
    }

    public void SetAvatar()
    {

        if (ApiHandle.Instance == null) return;

        avatarImage.sprite = ApiHandle.Instance.user.avatar.IsNullOrEmpty() ? defaultImage : SkinPool.instance.GetSkin(int.Parse(ApiHandle.Instance.user.avatar)).skinAvatar;


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
        txtPlayerName.text = UserSessionManager.Instance.username.IsNullOrEmpty() ? PrefsData.GetData(PrefsData.PLAYER_INGAME_NAME_NOLOGIN) : ApiHandle.Instance.user.nameingame.ToString();

    }

    private IEnumerator LogoutCoroutine()
    {
        LoadingPanel.Instance.SetDisplayLoading(true);
        yield return null;
        ApiHandle.Instance.GetComponent<WS_Client>().DisconnectWS();
        yield return new WaitUntil(() => !ApiHandle.Instance.GetComponent<WS_Client>().isConnect);
        Destroy(ApiHandle.Instance.GetComponent<WS_Client>());
        UserSessionManager.Instance.ClearSession();
        ApiHandle.Instance.user = null;
        Destroy(ApiHandle.Instance.GetComponent<LoginManager>());
        PrefsData.DeleteData(PrefsData.PLAYER_ID_UNITY_LOGIN);
        PrefsData.DeleteData(PrefsData.PLAYER_USERNAME_LOGIN);
        PrefsData.DeleteData(PrefsData.PLAYER_PASSWORD_LOGIN);
        if (SkinPanel.instance != null)
        {
            foreach (Transform itemSKin in SkinPanel.instance.skinContent.transform)
            {
                itemSKin.GetComponent<ItemSkin>().CheckDisplay();
                if (itemSKin.GetComponent<ItemSkin>().isSkinEquiped == true)
                {
                    break;
                }
            }
        }

        yield return new WaitForSeconds(UnityEngine.Random.Range(0.3f, 1.3f));
        ApiHandle.Instance.AddComponent<LoginManager>();
        PlayerSkin.instance.UpdateSkin();
        UI_Controller.Instance.UpdateMoney();
        avatarImage.sprite = defaultImage;
        yield return new WaitUntil(() => UserSessionManager.Instance.username.IsNullOrEmpty());
        LoadingPanel.Instance.SetDisplayLoading(false);
    }
}
