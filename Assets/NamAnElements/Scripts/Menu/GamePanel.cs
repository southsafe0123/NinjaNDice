using System;
using System.Collections;
using System.Collections.Generic;

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


    public void OnClickPlay()
    {
        LoadScene.Instance.StartLoadScene("LobbyScene");
    }


}
