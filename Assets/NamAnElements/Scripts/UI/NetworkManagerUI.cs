using Mono.CSharp;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    public static NetworkManagerUI Singleton;
    public Button btnServer;
    public Button btnHost;
    public Button btnClient;
    public Button btnRollDice;
    public Button btnStartGame;

    public Button test;
    public TextMeshProUGUI numDiceText;
    private void Awake()
    {
        Singleton = this;

        btnServer.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartServer();
        });
        btnHost.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();
        });
        btnClient.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient();
        });
        btnRollDice.onClick.AddListener(() =>
        {
            GameManager.Singleton.SendRollDiceTo_ServerRPC();
        });
        btnStartGame.onClick.AddListener(() =>
        {
            GameManager.Singleton.ClickStartGame();
        });
        // test.onClick.AddListener(() =>
        // {
        //     GameManager.Singleton.ClickStartGame();
        // });
    }
}
