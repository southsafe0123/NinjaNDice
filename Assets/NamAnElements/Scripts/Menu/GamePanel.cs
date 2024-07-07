using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GamePanel : MonoBehaviour
{
    public Button btnPlay;
    private void Start()
    {
        btnPlay.onClick.AddListener(OnClickPlay);
    }

    private void OnClickPlay()
    {
        SceneManager.LoadScene("LobbyScene");
    }
}
