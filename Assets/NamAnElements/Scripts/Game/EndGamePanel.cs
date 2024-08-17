using System.Collections.Generic;
using Unity.Multiplayer.Samples.Utilities;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EndGamePanel : MonoBehaviour
{
    public static EndGamePanel Instance;
    public Button btnBack;
    public Button btnLeave;
    public GameObject endGamePanel;
    public List<GameObject> playerRankingList = new List<GameObject>();
    public int top;
    private void Awake()
    {
        if(Instance == null)
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
        DisplayEndGamePanel(false);
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
        foreach (GameObject playerRankingGameobject in playerRankingList)
        {
            if (!playerRankingGameobject.activeInHierarchy)
            {
                playerRankingGameobject.SetActive(true);
                Player player = PlayerList.Instance.GetPlayerDic_Value(clientID);
                playerRankingGameobject.GetComponent<PlayerEndGameItem>().SetPlayerEndGameItem(player.GetComponent<PlayerData>().playerName.Value.ToString(), 400 / top, top);
                top++;
                break;
            }
        }
    }
}