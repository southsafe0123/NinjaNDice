using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : NetworkBehaviour
{
    public static LoadScene Instance;
    public Animator trasition;
    public float transitionTime;
    public GameObject waitForPlayerPanel;
    public bool isMultiplayerScene;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        StartCoroutine(PlayStartScene());
        SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
    }

    private void SceneManager_activeSceneChanged(Scene arg0, Scene arg1)
    {
        if (arg1.isLoaded)
        {
            if (isMultiplayerScene)
            {
                SetPlayerReadySceneLoaded_ServerRPC(NetworkManager.LocalClientId,true);
                StartCoroutine(WaitForPlayer());
            }
            else
            {
                StartCoroutine(PlayStartScene());
            }
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadySceneLoaded_ServerRPC(ulong clientID,bool isReadySceneLoaded)
    {
        PlayerList.Instance.GetPlayerDic_Value(clientID).isReadySceneLoaded.Value = isReadySceneLoaded;
    }

    private IEnumerator WaitForPlayer()
    {
        waitForPlayerPanel.SetActive(true);
        WaitForSeconds waithalfsecond = new WaitForSeconds(0.5f);
        bool isAllPlayerReady = false;
        while (true)
        {
            yield return waithalfsecond;
            foreach (Player player in PlayerList.Instance.playerDic.Values)
            {
                if (player.isReadySceneLoaded.Value == false)
                {
                    isAllPlayerReady = false;
                    break;
                }
                else
                {
                    isAllPlayerReady = true;
                }
                
            }

            if(isAllPlayerReady == true)
            {
                isMultiplayerScene = false;

                waitForPlayerPanel.SetActive(false);
                StartCoroutine(PlayStartScene());

                yield break;
            }
            
        }
        
    }

    public void StartScene()
    {
        StartCoroutine(PlayStartScene());
    }
    public void StartLoadScene(string sceneName)
    {
        StartCoroutine(PlayLoadScene(sceneName));
    }

    IEnumerator PlayLoadScene(string sceneName)
    {
        trasition.Play("EndTransition");
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(sceneName);
    }
    IEnumerator PlayStartScene()
    {
        yield return new WaitForEndOfFrame();
        trasition.Play("StartTransition");
    }

    public void StartLoadSceneMultiplayer(string sceneName, bool isHost)
    {
        StartCoroutine(PlayLoadSceneMultiplayer(sceneName, isHost));
    }

    private IEnumerator PlayLoadSceneMultiplayer(string sceneName, bool isHost)
    {
        isMultiplayerScene = true;
        SetPlayerReadySceneLoaded_ServerRPC(NetworkManager.LocalClientId, false);

        trasition.Play("EndTransition");
        yield return new WaitForSeconds(transitionTime);
        if (!isHost) yield break;
        NetworkManager.Singleton.SceneManager.LoadScene(sceneName, 0);
    }
}
