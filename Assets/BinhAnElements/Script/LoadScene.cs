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
    private void Awake()
    {
        if(Instance == null)
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
            StartCoroutine(PlayStartScene());
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

    public void StartLoadSceneMultiplayer(string sceneName,bool isHost)
    {
        StartCoroutine(PlayLoadSceneMultiplayer(sceneName, isHost));
    }

    private IEnumerator PlayLoadSceneMultiplayer(string sceneName,bool isHost)
    {
        trasition.Play("EndTransition");
        yield return new WaitForSeconds(transitionTime);
        if (!isHost) yield break;
        NetworkManager.Singleton.SceneManager.LoadScene(sceneName, 0);
    }
}
