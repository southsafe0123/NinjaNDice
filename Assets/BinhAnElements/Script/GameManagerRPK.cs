using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class GameManagerRPK : NetworkBehaviour
{
    public Map map;
    public Map mapFight;
    public int index;
    public GameObject myResult;
    public GameObject theirResult;
    Result theirResultTemp;
    Result myResultTemp;
    // Start is called before the first frame update
    void Start()
    {
        index = 0;
        foreach(Player player in PlayerList.Instance.playerDic.Values)
        {
            player.gameObject.transform.position = map.movePos[index].position;
            index++;
        }

        if (!IsHost) return;
        Player player1 = PlayerList.Instance.playerOrders[Random.Range(0, PlayerList.Instance.playerOrders.Count - 1)].player;
        Player player2 = PlayerList.Instance.playerOrders[Random.Range(0, PlayerList.Instance.playerOrders.Count - 1)].player==player1 ? PlayerList.Instance.playerOrders[Random.Range(0, PlayerList.Instance.playerOrders.Count - 1)].player : PlayerList.Instance.playerOrders[Random.Range(0, PlayerList.Instance.playerOrders.Count - 1)].player;

        player1.gameObject.transform.position = mapFight.movePos[0].position;
        player2.gameObject.transform.position = mapFight.movePos[1].position;

        CheckPlayerInFightMap_ClientRPC(player1.ownerClientID.Value,player2.ownerClientID.Value);

    }
    [ClientRpc]
    public void CheckPlayerInFightMap_ClientRPC(ulong player1ID, ulong player2ID)
    {
        if(NetworkManager.Singleton.LocalClientId == player1ID || NetworkManager.Singleton.LocalClientId == player2ID)
        {
            Debug.Log("ok");
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
