using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : NetworkBehaviour
{
    public static GameManager Singleton;
    public NetworkVariable<int> dice = new NetworkVariable<int>();
    public List<Player> playerList = new List<Player>();
    public int gameTurn;
    public int playerIndex;
    public Map map;
    public NetworkManagerUI networkManagerUI;
    public CamToPlayer camToPlayer;
    private void Awake()
    {
        Singleton = this;
        playerIndex = 0;
        gameTurn = 0;

        // playerList = GameObject.FindObjectsByType<Player>(sortMode: FindObjectsSortMode.None).ToList();
    }
    private void Start()
    {
        dice.OnValueChanged += OnDiceValueChanged;
        if (!IsHost) return;
        playerList = PlayerList.Instance.GetPlayerOrder();
        for (int i = 0; i < playerList.Count; i++)
        {
            playerList[i].gameObject.transform.position = map.movePos[playerList[i].currentPos.Value].position;
        }
        playerList[playerIndex].isPlayerTurn.Value = true;
        SetCamFollowPlayer_ClientRPC(playerList[playerIndex].ownerClientID.Value);
    }

    private void OnDiceValueChanged(int oldValue, int newValue)
    {
        UpdateDiceUI(newValue);
    }
    public void TeleportPlayer(Player clientPlayer, int index = 0) // nhat da sua ham nay
    {
        if (index == 0)
        {
            clientPlayer.gameObject.transform.position = map.movePos[0].position;
        }
        else
        {

            StartCoroutine(DoTeleportPlayerCoroutine(index, clientPlayer));
        }
    }

    private IEnumerator DoTeleportPlayerCoroutine(int index, Player clientPlayer)
    {
        WaitUntil waitUntil = new WaitUntil(() => clientPlayer.gameObject.transform.position == map.movePos[clientPlayer.currentPos.Value].position);
        WaitForSeconds waitForSeconds = new WaitForSeconds(0.15f);
        int posCount = 1;
        do
        {
            yield return null;
            int newPos = clientPlayer.currentPos.Value + 1;
            if (newPos >= map.movePos.Count)
            {
                newPos = map.movePos.Count;
            }
            clientPlayer.currentPos.Value = newPos;
            try
            {
                clientPlayer.gameObject.transform.DOJump(map.movePos[newPos].position, 0.5f, 1, 0.4f);
                posCount++;
            }
            catch
            {
                clientPlayer.gameObject.transform.position = map.movePos[map.movePos.Count - 1].position;
                break;
            };
            yield return waitUntil;
            yield return waitForSeconds;
        } while (posCount <= index);

        if (clientPlayer.gameObject.transform.position == map.movePos[map.movePos.Count - 1].position) CheckEndGame_ClientRPC(clientPlayer.ownerClientID.Value);

        playerIndex = playerIndex >= playerList.Count - 1 ? 0 : playerIndex + 1;
        var oldGameTurn = gameTurn;
        gameTurn = playerIndex == 0 ? gameTurn + 1 : gameTurn;
        OnGameTurnChange(oldGameTurn, gameTurn);
        StartCoroutine(SwitchCamCoroutine());
    }
    [ClientRpc]
    private void CheckEndGame_ClientRPC(ulong clientPlayerID)
    {
        if (PlayerList.Instance.playerOrders.Count > 2)
        {
            EndGamePanel.Instance.btnBack.interactable = true;
            if (IsHost)
            {
                EndGamePanel.Instance.btnLeave.gameObject.SetActive(false);
            }
            EndGamePanel.Instance.DisplayEndGamePanel(true);
            EndGamePanel.Instance.AddPlayerRankingList(clientPlayerID);
            //setplayerWin;
            PlayerList.Instance.GetPlayerDic_Value(clientPlayerID).isPlayerDoneGame.Value = true;
            //updatePlayerOrder
            PlayerList.Instance.ResetPlayerOrder();
        }
        else
        {
            EndGamePanel.Instance.btnBack.interactable = false;
            EndGamePanel.Instance.btnLeave.gameObject.SetActive(true);
            EndGamePanel.Instance.DisplayEndGamePanel(true);
            EndGamePanel.Instance.AddPlayerRankingList(clientPlayerID);
            EndGamePanel.Instance.AddPlayerRankingList(clientPlayerID);
        }
    }

    public void SwitchCam()
    {
        StartCoroutine(SwitchCamCoroutine());
    }

    private IEnumerator SwitchCamCoroutine()
    {
        yield return new WaitForSeconds(1f);
        var playerID = playerList[playerIndex].ownerClientID.Value;
        SetCamFollowPlayer_ClientRPC(PlayerList.Instance.GetPlayerDic_Value(playerID).ownerClientID.Value);
        yield return null;
        playerList[playerIndex].isPlayerTurn.Value = true;
    }

    [ClientRpc]
    private void SetCamFollowPlayer_ClientRPC(ulong playerID)
    {
        camToPlayer.playerToFollow = PlayerList.Instance.GetPlayerDic_Value(playerID);
        camToPlayer.playerInTurn = PlayerList.Instance.GetPlayerDic_Value(playerID);
    }

    public void OnGameTurnChange(int oldGameTurn, int newGameTurn)
    {
        if (oldGameTurn == newGameTurn) return;
        ChangeScene_ClientRPC();
        //Debug.LogError("isminigame now");
    }
    [ClientRpc]
    private void ChangeScene_ClientRPC()
    {
        StartCoroutine(ChangeSceneCoroutine());
    }

    private IEnumerator ChangeSceneCoroutine()
    {
        yield return new WaitForSeconds(1f);
        var randomvalue = 2;
        switch (randomvalue)
        {
            case 0:
                LoadScene.Instance.StartLoadSceneMultiplayer("minigameAU", IsHost);
                break;
            case 1:
                LoadScene.Instance.StartLoadSceneMultiplayer("minigameQuizz", IsHost);
                break;
            case 2:
                LoadScene.Instance.StartLoadSceneMultiplayer("MinigameRockPaperKunai", IsHost);
                break;
            default:
                Debug.LogError("isminigame now");
                break;
        }

    }

    [ServerRpc(RequireOwnership = false)]
    public void SendRollDiceTo_ServerRPC(ulong clientID)
    {
        var diceValue = UnityEngine.Random.Range(1, 7);
        ChangeDiceValue_ClientRPC(diceValue, clientID);
    }
    [ClientRpc]
    public void ChangeDiceValue_ClientRPC(int diceValue, ulong clientID)
    {
        StartCoroutine(RollDiceCoroutine(diceValue, clientID));

    }

    [ServerRpc(RequireOwnership = false)]
    public void SetPlayerTurn_ServerRPC(ulong clientID, bool isPlayerTurn)
    {
        SetPlayerTurn_ClientRPC(clientID, isPlayerTurn);
    }
    [ClientRpc]
    public void SetPlayerTurn_ClientRPC(ulong clientID, bool isPlayerTurn)
    {
        var player = PlayerList.Instance.GetPlayerDic_Value(clientID);
        if (IsHost)
        {
            player.isPlayerTurn.Value = isPlayerTurn;
        }
    }

    private IEnumerator RollDiceCoroutine(int diceValue, ulong clientID)
    {
        var player = PlayerList.Instance.GetPlayerDic_Value(clientID);
        SetPlayerTurn_ClientRPC(clientID, false);
        var playerDice = player.GetComponentInChildren<Animator>();
        playerDice.Play("Dice_Roll");
        playerDice.transform.localScale = new Vector2(0.5f, 0.5f);
        playerDice.transform.localPosition = Vector2.zero;
        playerDice.transform.DOScale(new Vector2(1, 1), 1).SetEase(Ease.OutBack);
        playerDice.transform.DOMove(playerDice.transform.position + Vector3.up * 1.5f, 1).SetEase(Ease.OutBack).OnComplete(() =>
        {
            if (IsHost) dice.Value = diceValue;
            playerDice.Play($"Dice_Result_{diceValue}");
        });
        yield return new WaitUntil(() => dice.Value == diceValue);
        yield return new WaitForSeconds(1.5f);
        playerDice.Play("Dice_Idle");
        if (!IsHost) yield break;
        TeleportPlayer(playerList[playerIndex], diceValue); // id cua nguoi roll dice
    }

    private void UpdateDiceUI(int value)
    {
        networkManagerUI.numDiceText.text = value.ToString();
    }

}
