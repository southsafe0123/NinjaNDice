using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndGamePanel : MonoBehaviour
{
    public static EndGamePanel Instance;
    public Button btnBack;
    public Button btnLeave;
    public GameObject endGamePanel;
    public List<GameObject> playerRankingList = new List<GameObject>();
    private void Awake()
    {
        Instance = this;
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
    public void AddPlayerRankingList(ulong clientPlayerID)
    {
        foreach (GameObject playerRanking in playerRankingList)
        {
            if (!playerRanking.activeInHierarchy)
            {
                playerRanking.SetActive(true);
                break;
            }
        }
    }
}