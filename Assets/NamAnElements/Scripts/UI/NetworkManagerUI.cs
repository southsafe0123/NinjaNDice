using Mono.CSharp;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    // Start is called before the first frame update
    public Button btnServer;
    public Button btnHost;
    public Button btnClient;
    private void Awake()
    {
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
    }
}
