using System;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class MiniEndGamePanel : MonoBehaviour
{
    public static MiniEndGamePanel Instance;
    public GameObject endMinigamePanelGroup;
    public List<PlayerMiniEndGameItem> playerEndList = new List<PlayerMiniEndGameItem>();
    public List<Player> playerLose = new List<Player>();
    public Player playerWin;
    public TextMeshProUGUI txtWaitToLeave;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        foreach (PlayerMiniEndGameItem playerItem in playerEndList)
        {
            playerItem.gameObject.SetActive(false);
        }
    }
    public void DisplayEndMinigame(bool isDisplay)
    {
        if (!AudioManager.Instance.sfxSource.isPlaying)
        {
            AudioManager.Instance.PlaySFXEndGamePanel();
        }
        endMinigamePanelGroup.SetActive(isDisplay);
    }
    public void SettextWaitToLeave(string text)
    {
        txtWaitToLeave.text = "LEAVE IN " + text + "...";
    }
    public void AddPlayerLose(Player player)
    {
        if (playerLose.Contains(player)) return;
        playerLose.Add(player);
    }
    public void SetPlayerWin(Player player)
    {
        playerWin = player;
    }
    public void DisplayPlayer(Player player,int topPlayer)
    {
        var playerCanGetItem = (int)Math.Ceiling((double)PlayerList.Instance.playerOrders.Count / 2);
        Debug.Log("playerCanGetItem In minigame: " + playerCanGetItem);
        foreach (PlayerMiniEndGameItem playerItem in playerEndList)
        {
            if (!playerItem.gameObject.activeInHierarchy)
            {
                playerItem.gameObject.SetActive(true);
                playerItem.player = player;
                playerItem.playerTop = "Top " + topPlayer;
                if (NetworkManager.Singleton.IsHost)
                {
                    if (topPlayer <= playerCanGetItem)
                    {
                        Debug.Log("This player can get item: " + player.ownerClientID.Value);
                        ItemPool.Instance.GivePlayerRandomItem_ClientRPC(player.ownerClientID.Value);
                    }
                    PlayerList.Instance.SetPlayerOrder(topPlayer, player);
                }
                Debug.Log("top: " + topPlayer);
                break;
            }
        }
    }
    
}
