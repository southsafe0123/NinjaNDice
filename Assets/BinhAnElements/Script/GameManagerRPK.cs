using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.VisualScripting;
using Unity.Services.Lobbies.Models;
using TMPro;
using System.Linq;
using DG.Tweening;

public class GameManagerRPK : NetworkBehaviour
{
    public const string MAIN_GAMEPLAY_SCENE = "NamAn";
    public static GameManagerRPK instance;
    public Map map;
    public Map mapFight;
    public int index, indexInFight, topPlayer = 1;
    public GameObject gameInput, time;

    public List<Player> players = new List<Player>();
    public List<Player> playerRange = new List<Player>();
    public List<Player> playerRangeTemp = new List<Player>();
    HashSet<int> selectedIndices = new HashSet<int>();
    public Player playerInFight;
    public List<Player> checkPlayerInFight;
    private bool hostPlayerAdded = false;
    [SerializeField] TextMeshProUGUI timerText;

    private void Awake()
    {
        instance = this;
    }

    public void Start()
    {
        foreach (Player player in PlayerList.Instance.playerDic.Values)
        {
            player.AddComponent<PlayerHeath>().health = 2;
        }

        foreach (Player player in PlayerList.Instance.playerDic.Values)
        {
            player.gameObject.transform.position = map.movePos[index].position;
            CheckPlayerInFightMap_ClientRPC(player.ownerClientID.Value, 2);
            players.Add(player);
            index++;
        }

        //Player hostPlayer = null; 

        //foreach (Player player in players)
        //{
        //    if (player.ownerClientID.Value == NetworkManager.Singleton.LocalClientId)
        //    {
        //        hostPlayer = player;
        //    }
        //}

        //playerRange.Add(hostPlayer);
        //players.Remove(hostPlayer);
        //List<Player> list = new List<Player>(players);
        //while (playerRange.Count < 2)
        //{
        //    int randomIndex = Random.Range(0, players.Count);
        //    if (!selectedIndices.Contains(randomIndex))
        //    {
        //        playerRange.Add(players[randomIndex]);
        //        selectedIndices.Add(randomIndex);
        //        list.Remove(players[randomIndex]);
        //    }
        //}

        SetPlayersTurnToFalse();


        //foreach (Player player in list)
        //{
        //    //if(list.Count > 1)
        //    //{
        //    //    gameInput.SetActive(true);
        //    //    gameInput.GetComponent<RpkButton>().player = PlayerList.Instance.GetPlayerDic_Value(player.ownerClientID.Value);
        //    //    gameInput.GetComponent<RpkButton>().ConnetResult(2);
        //    //}
        //    //else
        //    //{
        //    //    gameInput.SetActive(true);
        //    //    gameInput.GetComponent<RpkButton>().player = PlayerList.Instance.GetPlayerDic_Value(player.ownerClientID.Value);
        //    //    gameInput.GetComponent<RpkButton>().ConnetResult(1);
        //    //}

        //    CheckPlayerInFightMap_ClientRPC(playerInFight.ownerClientID.Value, 2);
        //}

    }

    public void PickingPlayerList()
    {
        index = 0;
        players.Clear();
        playerRange.Clear();
        selectedIndices.Clear();
        checkPlayerInFight.Clear();
        foreach (Player player in PlayerList.Instance.playerDic.Values)
        {
            player.gameObject.transform.DOJump(map.movePos[index].position, 0.5f, 1, 0.4f);
            if (player.GetComponent<PlayerHeath>().health > 0)
            {
                players.Add(player);
            }
            else
            {
                players.Remove(player);
            }
            index++;
        }

        while (playerRange.Count < 2)
        {
            if (players.Count != 1)
            {
                int randomIndex = Random.Range(0, players.Count);
                if (!selectedIndices.Contains(randomIndex))
                {
                    playerRange.Add(players[randomIndex]);
                    selectedIndices.Add(randomIndex);
                }
            }

            if (players.Count == 1)
            {
                foreach (Player player in players)
                {
                    playerRangeTemp.Add(player);
                }
                break;
            }
        }
    }

    public void EndGame(List<Player> playerRangeTemp)
    {
        foreach (Player player in playerRangeTemp)
        {
            player.gameObject.transform.position = mapFight.movePos[2].position;
        }
        gameInput.SetActive(false);
    }

    public void TelePlayerToFightMap(List<Player> playerRange)
    {
        indexInFight = 0;
        foreach (Player playerInFight in playerRange)
        {
            if (indexInFight < mapFight.movePos.Count)
            {
                playerInFight.gameObject.transform.DOJump(mapFight.movePos[indexInFight].position, 0.5f, 1, 0.4f);
                CheckPlayerInFightMap_ClientRPC(playerInFight.ownerClientID.Value, indexInFight);
                checkPlayerInFight.Add(playerInFight);
                indexInFight++;
            }
        }
    }

    [ClientRpc]
    public void CheckPlayerInFightMap_ClientRPC(ulong playerID, int indexInFight)
    {
        if (NetworkManager.Singleton.LocalClientId == playerID)
        {
            gameInput.SetActive(true);
            gameInput.GetComponent<RpkButton>().player = PlayerList.Instance.GetPlayerDic_Value(playerID);
            gameInput.GetComponent<RpkButton>().ConnetResult(indexInFight);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void ShowResult_ServerRPC(int index, string resultName)
    {
        ShowResult_ClientRPC(index, resultName);
    }

    [ClientRpc]
    public void ShowResult_ClientRPC(int index, string resultName)
    {
        if (gameInput.GetComponent<RpkButton>().indexInFight == index) return;
        gameInput.GetComponent<RpkButton>().DisplayTheirResult(resultName);
    }

    [ServerRpc(RequireOwnership = false)]
    public void ShowHPPlayer_ServerRPC(ulong playerID)
    {

        ShowHPPlayer_ClientRPC(playerID);

    }

    [ClientRpc]
    public void ShowHPPlayer_ClientRPC(ulong playerID)
    {
        var player = PlayerList.Instance.GetPlayerDic_Value(playerID);
        player.GetComponent<PlayerHeath>().health--;
        player.DisplayCurrentHealth();
        //AudioManager.Instance.PlaySFXTakeHp();
        if (playerID == NetworkManager.Singleton.LocalClientId)
        {
            LifeRemainPanel.instance.UpdateHealth();
            Debug.Log("Myplayer health: " + NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerHeath>().health);
            if (player.GetComponent<PlayerHeath>().health == 0)
            {
                player.GetComponent<PlayerHeath>().isDead = true;
                Debug.Log("Player: " + playerID + " lose");
                CallThisPlayerIsDead_ServerRPC(playerID);
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void CallThisPlayerIsDead_ServerRPC(ulong playerID)
    {
        var player = PlayerList.Instance.GetPlayerDic_Value(playerID);
        MiniEndGamePanel.Instance.AddPlayerLose(player);

        if (MiniEndGamePanel.Instance.playerLose.Count >= PlayerList.Instance.playerOrders.Count - 1)
        {
            StartCoroutine(EndGame());
        }
    }

    private void SetPlayersTurnToFalse()
    {
        foreach (Player player in PlayerList.Instance.playerDic.Values)
        {
            player.isPlayerTurn.Value = false;
        }
    }

    private IEnumerator EndGame()
    {
        WaitForSeconds wait1f = new WaitForSeconds(1.15f);
        Player playerWin = PlayerList.Instance.playerDic.First(player => !MiniEndGamePanel.Instance.playerLose.Contains(player.Value)).Value != null ? PlayerList.Instance.playerDic.First(player => !MiniEndGamePanel.Instance.playerLose.Contains(player.Value)).Value : null;
        Debug.Log("PlayerWin:" + playerWin.ownerClientID.Value);
        if (playerWin != null)
        {
            MiniEndGamePanel.Instance.SetPlayerWin(playerWin);
            EndGameAnouncement_ClientRPC(playerWin.ownerClientID.Value, topPlayer);
            topPlayer++;
        }
        MiniEndGamePanel.Instance.playerLose.Reverse();
        yield return wait1f;
        foreach (Player player in MiniEndGamePanel.Instance.playerLose)
        {
            EndGameAnouncement_ClientRPC(player.ownerClientID.Value, topPlayer);
            topPlayer++;
            yield return wait1f;
        }

        RemovedComponent_ClientRPC();
        SetPlayersTurnToFalse();

        int i = 3;
        while (i > -1)
        {
            CallToLeave_ClientRPC(i);
            yield return wait1f;
            i--;
        }

        StartLoadScene_ClientRPC();
    }

    [ClientRpc]
    private void CallToLeave_ClientRPC(int i)
    {
        MiniEndGamePanel.Instance.SettextWaitToLeave(i.ToString());
    }
    [ClientRpc]
    private void StartLoadScene_ClientRPC()
    {
        LoadScene.Instance.StartLoadSceneMultiplayer(MAIN_GAMEPLAY_SCENE, IsHost);
    }

    [ClientRpc]
    private void EndGameAnouncement_ClientRPC(ulong playerID, int topPlayer)
    {
        MiniEndGamePanel.Instance.DisplayEndMinigame(true);
        MiniEndGamePanel.Instance.DisplayPlayer(PlayerList.Instance.GetPlayerDic_Value(playerID), topPlayer);
    }

    [ClientRpc]
    private void RemovedComponent_ClientRPC()
    {
        foreach (var player in PlayerList.Instance.playerDic)
        {
            Destroy(player.Value.GetComponent<PlayerHeath>());
        }
    }

    [ClientRpc]
    private void CallEndGame_ClientRPC(ulong playerID, bool isWin)
    {
        if (isWin) Debug.LogError("Player Win: " + playerID);
        if (!isWin) Debug.LogError("Player lose: " + playerID);
    }
}
