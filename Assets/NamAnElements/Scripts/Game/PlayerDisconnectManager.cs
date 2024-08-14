using System.Collections;
using System.Collections.Generic;
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
        PlayerList.Instance.playerDic.Remove(clientID);

        Player player = PlayerList.Instance.GetPlayerDic_Value(clientID);
        PlayerList.Instance.playerOrders.Remove(PlayerList.Instance.playerOrders.First(playerOrder => playerOrder.player == player));

        //if (!sv_dicPlayer.ContainsKey(clientID)) return;
        //sv_dicPlayer[clientID]?.SetActive(false);

        //foreach (var player in sv_dicPlayer)
        //{
        //    if (player.Key.Equals(clientID))
        //    {
        //        int slotIndex = playerSlots.IndexOf(sv_dicPlayer[clientID]);
        //        SetActiveInClient_ClientRPC(slotIndex, false);
        //    };
        //}

        //sv_dicPlayer.Remove(clientID);
        ////PlayerList.Instance.UpdatePlayerList();
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
        yield return new WaitUntil(() => PlayerList.Instance.playerDic.Count == 1);
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
        NetworkManager.Singleton.Shutdown();
        if (AuthenticationService.Instance.IsSignedIn)
        {
            AuthenticationService.Instance.SignOut();
        }
        Destroy(GameObject.Find("PlayerList"));
        NetworkManager.Destroy(NetworkManager.gameObject);
        //Destroy(PlayerList.Instance.gameObject);

        StartCoroutine(WaitForShutdownAndLoadScene());
    }
    private IEnumerator WaitForShutdownAndLoadScene()
    {
        // Chờ cho NetworkManager shutdown hoàn toàn
        yield return new WaitUntil(() => NetworkManager == null);

        LoadScene.Instance.StartLoadScene("MenuScene");
    }
}
