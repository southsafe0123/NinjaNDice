using Mono.CSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using WebSocketSharp;

public class LobbyGameManager : NetworkBehaviour
{
    public static LobbyGameManager Instance;
    //this is server data (because clientID is Online and only server have)
    public Dictionary<ulong, GameObject> sv_dicPlayer = new Dictionary<ulong, GameObject>();
    //this is all userdata (because it offline)
    public List<GameObject> playerSlots = new List<GameObject>();
    public Button disconnectButton;
    public GameObject startGameButton;
    //use to set playerOrder that login into this server
    private int playerOrder = 0;

    public List<Player> playerListTest = new List<Player>();
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        playerSlots[0].SetActive(true);
        if(ApiHandle.Instance.user.avatar.IsNullOrEmpty())
        {
            playerSlots[0].transform.Find("Image").GetComponent<Image>().sprite = SkinPool.instance.GetSkin(0).skinAvatar;
        }
        else
        {
            playerSlots[0].transform.Find("Image").GetComponent<Image>().sprite = SkinPool.instance.GetSkin(int.Parse(ApiHandle.Instance.user.avatar)).skinAvatar;
        }
         
        RegisterDisconnectButton();
        NetworkManager.Singleton.OnClientConnectedCallback += OnConnectedClient;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnDisconnectedClient;
    }
    private void OnDisable()
    {
        try
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnConnectedClient;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnDisconnectedClient;
        }
        catch (Exception)
        {
            Debug.LogError("Lobbygamemanager_ Cant remove call back");
        }

    }

    private void RegisterDisconnectButton()
    {
        disconnectButton.onClick.AddListener(() =>
        {
            OnClickDisconnect();
        });
    }

    private void OnConnectedClient(ulong clientID)
    {
        if (!IsHost) return;
        LoadPlayer(clientID);
        Debug.LogError("userjoin: " + clientID);
    }
    private void LoadPlayerOrder(ulong clientID)
    {
        var plInstance = PlayerList.Instance;
        plInstance.AddPlayerOrder(playerOrder, plInstance.GetPlayerDic_Value(clientID));
        playerOrder++;
    }

    private void LoadListPlayerDic(ulong clientID)
    {
        var plInstance = PlayerList.Instance;
        var playerList = GameObject.FindObjectsByType<Player>(sortMode: FindObjectsSortMode.None).ToList();
        var player = playerList.First(player => player.ownerClientID.Value == clientID);
        plInstance.SetPlayerDic(clientID, player);
        plInstance.SetPlayerDic_ClientRPC();
    }

    private void OnDisconnectedClient(ulong clientID)
    {
        if (!IsHost) return;
        UnloadPlayer(clientID);
        SetActiveButton_ClientRPC(true);
        UpdateLobby_ClientRPC();
        Debug.LogError("userout: " + clientID);
    }


    //below is detail of all function mean
    [ClientRpc]
    public void UpdateLobby_ClientRPC()
    {
        StartCoroutine(WaitUntilHaveData());
    }

    private IEnumerator WaitUntilHaveData()
    {
        yield return new WaitForSeconds(0.15f);
        Debug.Log("Cllear=>?>>>>>>");
        playerListTest.Clear();
        foreach (var item in playerSlots)
        {
            item.SetActive(false);
        }
        yield return new WaitForSeconds(0.15f);
        Debug.Log("Add=>?>>>>>>");
        foreach (Player player in PlayerList.Instance.playerDic.Values)
        {
            Debug.Log("=>>>>>P: " + player.ownerClientID.Value);
            playerListTest.Add(player);
        }
        yield return new WaitForSeconds(0.15f);
        Debug.Log("Done=>?>>>>>>");
        for (int i = 0; i < playerListTest.Count; i++)
        {
            yield return new WaitUntil(() => !playerListTest[i].GetComponent<PlayerData>().playerSkin.Value.ToString().IsNullOrEmpty());
            playerSlots[i].SetActive(true);
            var playerSkinSlot = playerListTest[i].GetComponent<PlayerData>().playerSkin.Value.ToString();
            Debug.Log("AvatarSlot: "+playerSkinSlot);
            var playerSprite = SkinPool.instance.GetSkin(int.Parse(playerSkinSlot)).skinAvatar;
            playerSlots[i].transform.Find("Image").GetComponent<Image>().sprite = playerSprite;
        }
    }

    private void LoadPlayer(ulong clientID)
    {
        //PlayerList.Instance.UpdatePlayerList();
        GameObject emptyPlayerSlot = GetEmptyPlayerSlot();
        if (!emptyPlayerSlot)
        {
            NetworkManager.Singleton.DisconnectClient(clientID);
            return;
        }
        sv_dicPlayer[clientID] = emptyPlayerSlot;

        AddOwnerClientID(clientID);
        LoadListPlayerDic(clientID);
        LoadPlayerOrder(clientID);
        UpdateLobby_ClientRPC();
        SetActiveButton_ClientRPC(true);
       

    }
    private IEnumerator WaitUntilHaveData(int slotIndex, ulong clientID)
    {
        yield return new WaitUntil(() => PlayerList.Instance.GetPlayerDic_Value(clientID));
        SetActiveInClient_ClientRPC(slotIndex, true, clientID);
    }

    private void AddOwnerClientID(ulong clientID)
    {
        var playerInClient = NetworkManager.Singleton.ConnectedClients[clientID].PlayerObject.GetComponent<Player>();
        playerInClient.ownerClientID.Value = clientID;
    }

    [ClientRpc] 
    public void SetActiveInClient_ClientRPC(int slotIndex, bool isActive, ulong clientID)
    {
        playerSlots[slotIndex].SetActive(isActive);
    }

    private GameObject GetEmptyPlayerSlot()
    {
        foreach (var playerSlot in playerSlots)
        {
            if (!playerSlot.activeSelf) return playerSlot;
        }

        return null;
    }

    [ClientRpc]
    private void SetActiveButton_ClientRPC(bool isActive)
    {
        startGameButton.GetComponentInChildren<TextMeshProUGUI>().text = "S T A R T";
        startGameButton.GetComponent<Button>().interactable = IsHost && sv_dicPlayer.Count > 1;
        //disconnectButton.SetActive(isActive);
    }
    private void UnloadPlayer(ulong clientID)
    {
        if (!sv_dicPlayer.ContainsKey(clientID)) return;
        sv_dicPlayer[clientID]?.SetActive(false);

        foreach (var player in sv_dicPlayer)
        {
            if (player.Key.Equals(clientID))
            {
                int slotIndex = playerSlots.IndexOf(sv_dicPlayer[clientID]);
                SetActiveInClient_ClientRPC(slotIndex, false,clientID);
            };
        }

        sv_dicPlayer.Remove(clientID);
        //PlayerList.Instance.UpdatePlayerList();
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
        yield return new WaitUntil(() => playerSlots.Where(player => player.activeInHierarchy).Count() == 1);
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

    /// <summary>
    /// BELOW IS FOR BUTTON CLICK IN CANVAS PANEL COMMAND BUTTON
    /// </summary>
    public void ButtonClickHostServer()
    {
        NetworkLobby.Instance.CreateRelay();
    }
    public void ButtonClickJoinServer(TMP_InputField joinCode)
    {
        NetworkLobby.Instance.JoinRelay(joinCode.text);
    }
    public void ButtonClickJoinServer(string joinCode)
    {
        NetworkLobby.Instance.JoinRelay(joinCode);
    }
    public void ButtonClickStartGame()
    {
        StartGame_ClientRPC();

    }
    [ClientRpc]
    private void StartGame_ClientRPC()
    {
        LoadScene.Instance.StartLoadSceneMultiplayer("NamAn", IsHost);
    }
}
