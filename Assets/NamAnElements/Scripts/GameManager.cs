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
        playerList = GameObject.FindObjectsByType<Player>(sortMode: FindObjectsSortMode.None).ToList();
        foreach (Player player in playerList)
        {
            SortPlayerListByServer_ClientRPC(player.ownerClientID.Value);
        }
        for (int i = 0; i < playerList.Count; i++)
        {
            playerList[i].gameObject.transform.position = map.movePos[playerList[i].currentPos.Value].position;
        }
        playerList[playerIndex].isPlayerTurn.Value = true;
        SetCamFollowPlayer_ClientRPC(playerIndex);
    }
    [ClientRpc]
    public void SortPlayerListByServer_ClientRPC(ulong clientID)
    {
        if (IsHost) return;
        var temp = GameObject.FindObjectsByType<Player>(sortMode: FindObjectsSortMode.None).ToList();
        foreach (var item in temp)
        {
            if(item.ownerClientID.Value == clientID)
            {
                playerList.Add(item);
                break;
            }
        }
    }
    private void OnDiceValueChanged(int oldValue, int newValue)
    {
        UpdateDiceUI(newValue);
    }
    void TeleportPlayer(Player clientPlayer, int index = 0) // nhat da sua ham nay
    {
        if (index == 0)
        {
            clientPlayer.gameObject.transform.position = map.movePos[0].position;
        }
        else
        {

            int newPos = clientPlayer.currentPos.Value + index;
            if (newPos >= map.movePos.Count)
            {
                newPos = map.movePos.Count;
            }
            clientPlayer.currentPos.Value = newPos;
            try
            {
                clientPlayer.gameObject.transform.position = map.movePos[newPos].position;
            }
            catch
            {
                clientPlayer.gameObject.transform.position = map.movePos[map.movePos.Count - 1].position;
            };
               

            if (clientPlayer.gameObject.transform.position == map.movePos[map.movePos.Count - 1].position) EndGame(clientPlayer);

            clientPlayer.isPlayerTurn.Value = false;
            playerIndex = playerIndex >= playerList.Count - 1 ? 0 : playerIndex + 1;

            var oldGameTurn = gameTurn;
            gameTurn = playerIndex == 0 ? gameTurn + 1 : gameTurn;
            OnGameTurnChange(oldGameTurn, gameTurn);
            playerList[playerIndex].isPlayerTurn.Value = true;
            StartCoroutine(SwitchCamCoroutine());
        }
    }

    private void EndGame(Player clientPlayer)
    {
        Debug.LogError("Player: " + clientPlayer.ownerClientID.Value + "Win");
    }



    private IEnumerator SwitchCamCoroutine()
    {
        yield return new WaitForSeconds(1f);
        SetCamFollowPlayer_ClientRPC(playerIndex);
    }

    [ClientRpc]
    private void SetCamFollowPlayer_ClientRPC(int playerIndex)
    {
        camToPlayer.playerToFollow = playerList[playerIndex];
    }

    private void OnGameTurnChange(int oldGameTurn, int newGameTurn)
    {
        if (oldGameTurn == newGameTurn) return;
        ToMinigame("minigameAU");
        //Debug.LogError("isminigame now");
    }

    [ServerRpc(RequireOwnership = false)]
    public void SendRollDiceTo_ServerRPC()
    {
        var diceValue = UnityEngine.Random.Range(1, 7);
        ChangeDiceValue(diceValue);
    }

    public void ChangeDiceValue(int diceValue)
    {
        if (!IsHost) return;
        dice.Value = diceValue;
        TeleportPlayer(playerList[playerIndex], diceValue); // id cua nguoi roll dice
    }
    private void UpdateDiceUI(int value)
    {
        networkManagerUI.numDiceText.text = value.ToString();
    }

    public void ToMinigame(string sceneName)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }
    }

    public void ModuleMinigame()
    {

    }

    public void BackToMainGame()
    {
        // chuyen scene ve main game
        // dich chuyen player ve vi tri cu da dung truoc khi vao minigame
        // chuyen turn ve nguoi choi cu


    }

    public void WinMinigame(/* nhan vao item thuong*/ )
    {
        // nhan thuong, luu item vao inventory cuar nguoi choi
        // BackToMainGame();


    }

}