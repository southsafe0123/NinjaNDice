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
        PlayerList.PlayerOrder playerNeedToRemove = PlayerList.Instance.playerOrders.First(playerOrder => playerOrder.player.Equals(PlayerList.Instance.GetPlayerDic_Value(clientID)));

        PlayerList.Instance.playerOrders.Remove(playerNeedToRemove);
        PlayerList.Instance.playerDic.Remove(clientID);

        PlayerList.Instance.ResetPlayerOrder();

        if (PlayerList.Instance.playerDic.Count < 2)
        {
            OnClickDisconnect();
        }
    }
    public void OnClickDisconnect()
    {

        if (!IsHost)
        {
            Disconnect();
        }
        else
        {
            OnClickDisconnect_ClientRPC();
            StartCoroutine(WaitAllPlayerLeft());
        }


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
}
