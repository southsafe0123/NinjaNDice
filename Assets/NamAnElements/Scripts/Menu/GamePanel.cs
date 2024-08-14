using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Tutorials.Core.Editor;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GamePanel : MonoBehaviour
{
    public static GamePanel Instance;
    private void Awake()
    {
        Instance = this;
    }
    public Button btnPlay;
    public GameObject request;



    private void Start()
    {
        btnPlay.onClick.AddListener(OnClickPlay);
    }

 

    private void OnClickPlay()
    {
        LoadScene.Instance.StartLoadScene("LobbyScene");
    }


}
