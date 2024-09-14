using System;
using System.Collections;
using System.Linq;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;

public class PlayerDisconnectManager : NetworkBehaviour
{
    public static PlayerDisconnectManager Instance;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        NetworkManager.Singleton.OnClientDisconnectCallback += OnDisconnectedClient;
        StartCoroutine(ServerShutDownCheck());
    }

    private IEnumerator ServerShutDownCheck()
    {
        if (IsHost) yield break ;
        yield return new WaitUntil(() => !NetworkManager.Singleton.IsConnectedClient);
        Disconnect();
    }

    private void OnDisconnectedClient(ulong clientID)
    {
        if (!IsHost) return;
        UnloadPlayer(clientID);
        Debug.LogError("userout: " + clientID);
    }

    private void UnloadPlayer(ulong clientID)
    {
        if (!PlayerList.Instance.playerDic.ContainsKey(clientID)) return;
        PlayerList.Instance.playerDic.Remove(clientID);

        PlayerList.Instance.ResetPlayerOrder();

        if (PlayerList.Instance.playerDic.Count < 2)
        {
            OnClickDisconnect();
        }
    }
    public void OnClickDisconnect()
    {
        CallDisconnect_ServerRPC();
    }
    [ServerRpc(RequireOwnership = false)]
    private void CallDisconnect_ServerRPC()
    {
        OnClickDisconnect_ClientRPC();
        StartCoroutine(WaitAllPlayerLeft());
    }

    private IEnumerator WaitAllPlayerLeft()
    {
        yield return new WaitUntil(() => PlayerList.Instance.playerDic.Count < 2);
        Disconnect();
    }

    [ClientRpc]
    void OnClickDisconnect_ClientRPC()
    {
        if (IsHost) return;
        Disconnect();
    }

    private void Disconnect()
    {
        try
        {
            NetworkManager.Singleton.Shutdown();
            if (AuthenticationService.Instance.IsSignedIn)
            {
                AuthenticationService.Instance.SignOut();
            }
            Destroy(GameObject.Find("PlayerList"));
            Destroy(EndGamePanel.Instance.gameObject);
            NetworkManager.Destroy(NetworkManager.gameObject);
            //Destroy(PlayerList.Instance.gameObject);

            StartCoroutine(WaitForShutdownAndLoadScene());
        }
        catch (System.Exception e)
        {
            Debug.LogError("PlayerDisconnectManager_SomethingWrong: " + e);
        }

    }
    private IEnumerator WaitForShutdownAndLoadScene()
    {
        // Chờ cho NetworkManager shutdown hoàn toàn
        yield return new WaitUntil(() => NetworkManager == null);

        LoadScene.Instance.StartLoadScene("MenuScene");
    }

    private void OnApplicationQuit()
    {
        OnClickDisconnect();
    }
}
