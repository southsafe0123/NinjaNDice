using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayButton: MonoBehaviour
{
    public static PlayButton Instance;
    public Button btnPlay;
    public TextMeshProUGUI txtPlay;
    private void Awake()
    {
        Instance = this;
    }
    public IEnumerator HostCoroutine()
    {
        txtPlay.text = "W A I T ...";
        btnPlay.interactable = false;
        LobbyGameManager.Instance.playerSlots[0].gameObject.SetActive(false);
        yield return null;
        LobbyGameManager.Instance.ButtonClickHostServer();
        btnPlay.onClick.RemoveListener(Host);
        btnPlay.onClick.AddListener(Play);
    }
    public void Host()
    {
        StartCoroutine(HostCoroutine());
    }
    public void Play()
    {
        LobbyGameManager.Instance.ButtonClickStartGame();
    }
    private void Start()
    {
        txtPlay.text = "H O S T";
        btnPlay.onClick.AddListener(Host);
    }
}
