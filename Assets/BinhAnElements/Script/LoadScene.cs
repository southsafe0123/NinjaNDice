using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public static LoadScene Instance;
    public Animator trasition;
    public float transitionTime;
    public GameObject waitForPlayerPanel;
    public bool isMultiplayerScene;
    public bool isAllPlayerReady = false;
    private void Awake()
    {
        if (Instance == null)
        {
            Debug.Log("LoadScene_created");
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
                SetPlayerReadyInScene(true);
                StartCoroutine(WaitForPlayer());
            }
            else
            {
                StartCoroutine(PlayStartScene());
            }
        }
    }
    

    private IEnumerator WaitForPlayer()
    {
        waitForPlayerPanel.SetActive(true);
        WaitForSeconds waithalfsecond = new WaitForSeconds(0.5f);
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
        SetPlayerReadyInScene(false);
        trasition.Play("EndTransition");
        yield return new WaitForSeconds(transitionTime);
        if (!isHost) yield break;
        NetworkManager.Singleton.SceneManager.LoadScene(sceneName, 0);
    }

    private static void SetPlayerReadyInScene(bool isReady)
    {
        PlayerList.Instance.GetPlayerDic_Value(NetworkManager.Singleton.LocalClientId).SetPlayerReadySceneLoaded_ServerRPC(NetworkManager.Singleton.LocalClientId, isReady);
    }
}
