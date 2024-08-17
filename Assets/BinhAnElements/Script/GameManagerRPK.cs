using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class GameManagerRPK : NetworkBehaviour
{
    public Map map;
    public Map mapFight;
    public int index, indexInFight;
    public GameObject myResult;
    public GameObject theirResult;
    public GameObject gameInput;
    Result theirResultTemp;
    Result myResultTemp;
    List<Player> players = new List<Player>();
    // Start is called before the first frame update
    void Start()
    {
        index = 0;
        foreach (Player player in PlayerList.Instance.playerDic.Values)
        {
            player.gameObject.transform.position = map.movePos[index].position;
            players.Add(player);
            index++;
        }
        Debug.Log("so player " + players.Count);

        List<Player> playerRange = new List<Player>();
        HashSet<int> selectedIndices = new HashSet<int>();

        while (playerRange.Count < 2)
        {
            int randomIndex = Random.Range(0, players.Count);
            if (!selectedIndices.Contains(randomIndex))
            {
                playerRange.Add(players[randomIndex]);
                selectedIndices.Add(randomIndex);
            }
        }

        Debug.Log("so playerRange " + playerRange.Count);
        TelePlayerToFightMap(playerRange);

    }

    public void TelePlayerToFightMap(List<Player> playerRange)
    {

        indexInFight = 0;
        foreach (Player playerInFight in playerRange)
        {
            if (indexInFight < mapFight.movePos.Count)
            {
                playerInFight.gameObject.transform.position = mapFight.movePos[indexInFight].position;
                indexInFight++;
                CheckPlayerInFightMap_ClientRPC(playerInFight.ownerClientID.Value);
            }
            else
            {
                Debug.LogWarning("Not enough positions in mapFight.movePos for all players.");
            }
        }

    }

    [ClientRpc]
    public void CheckPlayerInFightMap_ClientRPC(ulong playerID)
    {
        if(NetworkManager.Singleton.LocalClientId == playerID)
        {
            Debug.Log("ok");
            gameInput.SetActive(true);
        }
    }

    public enum Result
    {
        Rock, Paper, Kunai 
    }

    public void OnClickCheckResult()
    {
        foreach(Transform child in theirResult.transform)
        {
            if (child.gameObject.activeInHierarchy)
            {
                switch (child.gameObject.name)
                {
                    case "Scroll": theirResultTemp = Result.Paper; break;
                    case "Rock": theirResultTemp = Result.Rock; break;
                    case "Kunai": theirResultTemp = Result.Kunai; break;
                }
                break;
            }
        }

        foreach (Transform child in myResult.transform)
        {
            if (child.gameObject.activeInHierarchy)
            {
                switch (child.gameObject.name)
                {
                    case "Scroll": myResultTemp = Result.Paper; break;
                    case "Rock": myResultTemp = Result.Rock; break;
                    case "Kunai": myResultTemp = Result.Kunai; break;
                }
                break;
            }
        }

        switch (myResultTemp)
        {
            case Result.Rock:
                switch (theirResultTemp)
                {
                    case Result.Rock:
                        Debug.Log("Draw !!");
                        break;
                    case Result.Paper:
                        Debug.Log("Lose !!");
                        break;
                    case Result.Kunai:
                        Debug.Log("Win !!");
                        break;
                    default:
                        break;
                }
                break;
            case Result.Paper:
                switch (theirResultTemp)
                {
                    case Result.Rock:
                        Debug.Log("Win !!");
                        break;
                    case Result.Paper:
                        Debug.Log("Draw !!");
                        break;
                    case Result.Kunai:
                        Debug.Log("Lose !!");
                        break;
                    default:
                        break;
                }
                break;
            case Result.Kunai:
                switch (theirResultTemp)
                {
                    case Result.Rock:
                        Debug.Log("Lose !!");
                        break;
                    case Result.Paper:
                        Debug.Log("Win !!");
                        break;
                    case Result.Kunai:
                        Debug.Log("Draw !!");
                        break;
                    default:
                        break;
                }
                break;
            default:
                break;
        }

        foreach (Transform child in theirResult.transform)
        {
            child.gameObject.SetActive(false);
        }

        foreach (Transform child in myResult.transform)
        {
            child.gameObject.SetActive(false);
        }
    }

}
