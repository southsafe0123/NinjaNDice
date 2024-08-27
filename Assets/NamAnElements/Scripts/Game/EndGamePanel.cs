using System.Collections.Generic;
using Unity.Multiplayer.Samples.Utilities;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

public class EndGamePanel : MonoBehaviour
{
    public static EndGamePanel Instance;
    public Player player;
    public Button btnBack;
    public Button btnLeave;
    public Button btnLeaderBoard;
    public GameObject endGamePanel;
    public List<GameObject> playerRankingList = new List<GameObject>();
    public int top;
    public bool isDoneGame = false;
    private List<Player> tempPlayerInList = new List<Player>();
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            top = 1;
        }
        else
        {
            Destroy(gameObject);
        }

        foreach (GameObject playerRanking in playerRankingList)
        {
            playerRanking.SetActive(false);
        }
        player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Player>();
        DisplayEndGamePanel(false);
    }
    private void Update()
    {
        btnLeaderBoard.gameObject.SetActive(isDoneGame);
    }
    private void Start()
    {
        btnLeave.onClick.AddListener(() =>
        {
            PlayerDisconnectManager.Instance.OnClickDisconnect();
        });
    }
    public void DisplayEndGamePanel(bool isDisplay)
    {
        endGamePanel.SetActive(isDisplay);
    }
    public void UpdateRankingList(ulong clientID)
    {
        foreach (var item in tempPlayerInList)
        {
            if (item.ownerClientID.Value == clientID) return;
        }

        foreach (GameObject playerRankingGameobject in playerRankingList)
        {
            if (!playerRankingGameobject.activeInHierarchy)
            {
                playerRankingGameobject.SetActive(true);
                Player player = PlayerList.Instance.GetPlayerDic_Value(clientID);
                if (NetworkManager.Singleton.LocalClientId == clientID)
                {
                    isDoneGame = true;
                    if (!UserSessionManager.Instance._id.IsNullOrEmpty())
                    {
                        ApiHandle.Instance.AddMoney(top.ToString());
                    }
                }
                tempPlayerInList.Add(PlayerList.Instance.GetPlayerDic_Value(clientID));
                int money = 300;
                int calcMoney = 0;
                switch (top)
                {
                    case 1: calcMoney = money; break;
                    case 2: calcMoney = money * 75 / 100; break;
                    case 3: calcMoney = money * 50 / 100; break;
                    case 4: calcMoney = money * 25 / 100; break;
                }
                playerRankingGameobject.GetComponent<PlayerEndGameItem>().SetPlayerEndGameItem(player.GetComponent<PlayerData>().playerName.Value.ToString(), calcMoney, top);
                top++;
                break;
            }
        }

        PlayerList.Instance.playerDic.Remove(clientID);

        if (NetworkManager.Singleton.IsHost)
        {
            NetworkManager.Singleton.ConnectedClients[clientID].PlayerObject.Despawn();
            PlayerList.Instance.ResetPlayerOrder();
        }

    }
}