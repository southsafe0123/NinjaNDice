using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.VisualScripting;
using Unity.Services.Lobbies.Models;
public class GameManagerRPK : NetworkBehaviour
{
    public static GameManagerRPK instance;
    public Map map;
    public Map mapFight;
    public int index, indexInFight;
    public GameObject gameInput;
    
    List<Player> players = new List<Player>();
    public List<Player> playerRange = new List<Player>();
    HashSet<int> selectedIndices = new HashSet<int>();
    public Player playerInFight;
    public List<Player> checkPlayerInFight;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        foreach (Player player in PlayerList.Instance.playerDic.Values)
        {
            player.AddComponent<PlayerHeath>().health = 3;
        }
        PickingPlayerList();
        TelePlayerToFightMap(playerRange);
    }

    IEnumerator waitGameStart()
    {
        yield return new WaitForSeconds(3);
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

            player.gameObject.transform.position = map.movePos[index].position;
            players.Add(player);
            index++;
        }

        while (playerRange.Count < 2)
        {
            int randomIndex = Random.Range(0, players.Count);
            if (!selectedIndices.Contains(randomIndex))
            {
                playerRange.Add(players[randomIndex]);
                selectedIndices.Add(randomIndex);
            }
        }

    }

    public void TelePlayerToFightMap(List<Player> playerRange)
    {

        indexInFight = 0;
        foreach (Player playerInFight in playerRange)
        {
            if (indexInFight < mapFight.movePos.Count)
            {
                if (playerInFight.GetComponent<PlayerHeath>().health > 0)
                {
                    playerInFight.gameObject.transform.position = mapFight.movePos[indexInFight].position;
                    CheckPlayerInFightMap_ClientRPC(playerInFight.ownerClientID.Value, indexInFight);
                    checkPlayerInFight.Add(playerInFight);
                    indexInFight++;
                }
                else
                {
                    checkPlayerInFight.Remove(playerInFight);
                }
            }
            else
            {
                Debug.LogWarning("Not enough positions in mapFight.movePos for all players.");
            }
        }

    }

    [ClientRpc]
    public void CheckPlayerInFightMap_ClientRPC(ulong playerID, int indexInFight)
    {
        if(NetworkManager.Singleton.LocalClientId == playerID)
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
    public void ShowHPPlayer_ServerRPC(int health, ulong playerID)
    {
        ShowHPPlayer_ClientRPC(health, playerID);
    }

    [ClientRpc]
    public void ShowHPPlayer_ClientRPC(int health, ulong playerID)
    {
        PlayerList.Instance.GetPlayerDic_Value(playerID).GetComponent<PlayerHeath>().health = health;
    }
}
