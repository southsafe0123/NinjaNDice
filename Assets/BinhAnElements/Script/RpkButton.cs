using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Unity.Mathematics;

public class RpkButton : MonoBehaviour
{
    public static RpkButton instance;

    public GameObject resultLeft;
    public GameObject resultRight;
    public Button rockButton;
    public Button paperButton;
    public Button kunaiButton;
    public GameObject time;
    public GameObject win;
    public GameObject lose;
    public GameObject draw;
    public GameObject isDead;
    public GameObject notInBattle;
    public GameObject winner;
    public GameObject start;
    public Map fightMap;
    public Player player;
    public int indexInFight;
    public GameObject myResult;
    public GameObject theirResult;
    Result theirResultTemp;
    Result myResultTemp;
    //Transform scroll;
    //Transform kunai;
    //Transform rock;
    string resultName;

    public TextMeshProUGUI timerText;
    float remainingTime;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        StartCoroutine(WaitForPlayerChoose());
    }

    public IEnumerator WaitForPlayerChoose()
    {
        int i = 5;
        WaitForSeconds waitOneSecond = new WaitForSeconds(1);
        while (GameManagerRPK.instance.players.Count != 1)
        {
            if (player.GetComponent<PlayerHeath>().health > 0)
            {
                yield return waitOneSecond;
                time.SetActive(true);
                remainingTime = i;
                timerText.text = remainingTime.ToString();
                Debug.Log(i);
                i--;
            }
            else
            {
                yield return waitOneSecond;
                time.SetActive(false);
                remainingTime = i;
                timerText.text = remainingTime.ToString();
                Debug.Log(i);
                i--;
            }

            if (i < 0)
            {
                i = 5;
                yield return new WaitForSeconds(1);
                time.SetActive(false);
                rockButton.gameObject.SetActive(false);
                paperButton.gameObject.SetActive(false);
                kunaiButton.gameObject.SetActive(false);
                showTheirButton(resultName);
                yield return new WaitForSeconds(1);

                foreach (Transform child in theirResult.transform)
                {
                    child.gameObject.SetActive(false);
                }

                foreach (Transform child in myResult.transform)
                {
                    child.gameObject.SetActive(false);
                }

                OnClickCheckResult();

                yield return new WaitForSeconds(1);

                foreach (Transform child in myResult.transform)
                {
                    myResultTemp = Result.Clear;
                }

                start.gameObject.SetActive(false);
                lose.gameObject.SetActive(false);
                win.gameObject.SetActive(false);
                draw.gameObject.SetActive(false);
                notInBattle.gameObject.SetActive(false);

                if (player.GetComponent<PlayerHeath>().health == 0)
                {
                    isDead.SetActive(true);
                }


                yield return new WaitForSeconds(1);
                GameManagerRPK.instance.PickingPlayerList();

                if (GameManagerRPK.instance.players.Count > 1)
                {
                    GameManagerRPK.instance.TelePlayerToFightMap(GameManagerRPK.instance.playerRange);
                }
                else
                {
                    rockButton.gameObject.SetActive(false);
                    paperButton.gameObject.SetActive(false);
                    kunaiButton.gameObject.SetActive(false);
                    winner.gameObject.SetActive(true);
                    isDead.gameObject.SetActive(false);
                    GameManagerRPK.instance.EndGame(GameManagerRPK.instance.playerRangeTemp);
                    break;
                }
                yield return new WaitForSeconds(1);
            }
        }

    }


    public void ConnetResult(int indexInFight)
    {
        rockButton.gameObject.SetActive(false);
        paperButton.gameObject.SetActive(false);
        kunaiButton.gameObject.SetActive(false);
        rockButton.onClick.RemoveAllListeners();
        paperButton.onClick.RemoveAllListeners();
        kunaiButton.onClick.RemoveAllListeners();
        this.indexInFight = indexInFight;
        if (indexInFight == 0)
        {
            rockButton.gameObject.SetActive(true);
            paperButton.gameObject.SetActive(true);
            kunaiButton.gameObject.SetActive(true);
            unButtonClick(resultLeft);
            rockButton.onClick.AddListener(() => OnRockButtonClick(resultLeft));
            paperButton.onClick.AddListener(() => OnPaperButtonClick(resultLeft));
            kunaiButton.onClick.AddListener(() => OnKunaiButtonClick(resultLeft));
            myResult = resultLeft;
            theirResult = resultRight;
        }

        if (indexInFight == 1)
        {
            rockButton.gameObject.SetActive(true);
            paperButton.gameObject.SetActive(true);
            kunaiButton.gameObject.SetActive(true);
            unButtonClick(resultRight);
            rockButton.onClick.AddListener(() => OnRockButtonClick(resultRight));
            paperButton.onClick.AddListener(() => OnPaperButtonClick(resultRight));
            kunaiButton.onClick.AddListener(() => OnKunaiButtonClick(resultRight));
            myResult = resultRight;
            theirResult = resultLeft;
        }

        if (indexInFight == 2)
        {
            rockButton.gameObject.SetActive(false);
            paperButton.gameObject.SetActive(false);
            kunaiButton.gameObject.SetActive(false);
            myResult = resultRight;
            theirResult = resultLeft;
            myResultTemp = Result.Start;
        }

    }



    public void DisplayTheirResult(string resultName)
    {
        this.resultName = resultName;
        switch (resultName)
        {
            case "Scroll": theirResultTemp = Result.Paper; break;
            case "Rock": theirResultTemp = Result.Rock; break;
            case "Kunai": theirResultTemp = Result.Kunai; break;
            case "": theirResultTemp = Result.None; break;

            default:
                break;
        }
    }

    public void showTheirButton(string resultName)
    {
        DisplayTheirResult(resultName);
        foreach (Transform child in theirResult.transform)
        {
            if (child.gameObject.name == resultName)
            {
                child.gameObject.SetActive(true);
            }
            if (myResultTemp == Result.Clear)
            {
                child.gameObject.SetActive(false);
            }
        }
    }

    private void OnRockButtonClick(GameObject result)
    {
        foreach (Transform child in result.transform)
        {
            child.gameObject.SetActive(false);
            if (child.gameObject.name == "Rock")
            {
                myResultTemp = Result.Rock;
                child.gameObject.SetActive(true);
                GameManagerRPK.instance.ShowResult_ServerRPC(indexInFight, "Rock");
            }
        }
    }

    private void OnPaperButtonClick(GameObject result)
    {
        foreach (Transform child in result.transform)
        {
            child.gameObject.SetActive(false);
            if (child.gameObject.name == "Scroll")
            {
                myResultTemp = Result.Paper;
                child.gameObject.SetActive(true);
                GameManagerRPK.instance.ShowResult_ServerRPC(indexInFight, "Scroll");
            }
        }
    }

    private void OnKunaiButtonClick(GameObject result)
    {
        foreach (Transform child in result.transform)
        {
            child.gameObject.SetActive(false);
            if (child.gameObject.name == "Kunai")
            {
                myResultTemp = Result.Kunai;
                child.gameObject.SetActive(true);
                GameManagerRPK.instance.ShowResult_ServerRPC(indexInFight, "Kunai");
            }
        }
    }

    private void unButtonClick(GameObject result)
    {
        foreach (Transform child in result.transform)
        {
            child.gameObject.SetActive(false);
            if (!child.gameObject.activeInHierarchy)
            {
                myResultTemp = Result.None;
                GameManagerRPK.instance.ShowResult_ServerRPC(indexInFight, "");
            }
        }
    }

    public enum Result
    {
        Rock, Paper, Kunai, None, Clear, Start
    }

    public void OnClickCheckResult()
    {
        foreach (Transform child in theirResult.transform)
        {
            if (child.gameObject.activeInHierarchy)
            {
                switch (child.gameObject.name)
                {
                    case "Scroll":
                        theirResultTemp = Result.Paper;
                        break;
                    case "Rock":
                        theirResultTemp = Result.Rock;
                        break;
                    case "Kunai":
                        theirResultTemp = Result.Kunai;
                        break;
                    case "":
                        theirResultTemp = Result.None;
                        break;
                    default:
                        break;
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
                    case "Scroll":
                        myResultTemp = Result.Paper;
                        break;
                    case "Rock":
                        myResultTemp = Result.Rock;
                        break;
                    case "Kunai":
                        myResultTemp = Result.Kunai;
                        break;
                    case "":
                        myResultTemp = Result.None;
                        break;
                    default:
                        break;
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
                        draw.gameObject.SetActive(true);
                        break;
                    case Result.Paper:
                        Debug.Log("Lose !!");
                        if (player.ownerClientID.Value == NetworkManager.Singleton.LocalClientId)
                        {
                            GameManagerRPK.instance.ShowHPPlayer_ServerRPC(player.GetComponent<Player>().ownerClientID.Value);
                            lose.gameObject.SetActive(true);
                        }
                        break;
                    case Result.Kunai:
                        Debug.Log("Win !!");
                        win.gameObject.SetActive(true);
                        break;
                    case Result.None:
                        Debug.Log("Win !!");
                        win.gameObject.SetActive(true);
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
                        win.gameObject.SetActive(true);
                        break;
                    case Result.Paper:
                        Debug.Log("Draw !!");
                        draw.gameObject.SetActive(true);
                        break;
                    case Result.Kunai:
                        Debug.Log("Lose !!");
                        if (player.ownerClientID.Value == NetworkManager.Singleton.LocalClientId)
                        {
                            GameManagerRPK.instance.ShowHPPlayer_ServerRPC(player.GetComponent<Player>().ownerClientID.Value);
                            lose.gameObject.SetActive(true);
                        }
                        break;
                    case Result.None:
                        Debug.Log("Win !!");
                        win.gameObject.SetActive(true);
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
                        if (player.ownerClientID.Value == NetworkManager.Singleton.LocalClientId)
                        {
                            GameManagerRPK.instance.ShowHPPlayer_ServerRPC(player.GetComponent<Player>().ownerClientID.Value);
                            lose.gameObject.SetActive(true);
                        }
                        break;
                    case Result.Paper:
                        Debug.Log("Win !!");
                        win.gameObject.SetActive(true);
                        break;
                    case Result.Kunai:
                        Debug.Log("Draw !!");
                        draw.gameObject.SetActive(true);
                        break;
                    case Result.None:
                        Debug.Log("Win !!");
                        win.gameObject.SetActive(true);
                        break;
                    default:
                        break;
                }
                break;

            case Result.None:
                switch (theirResultTemp)
                {
                    case Result.Rock:
                        Debug.Log("Lose !!");
                        if (player.ownerClientID.Value == NetworkManager.Singleton.LocalClientId)
                        {
                            GameManagerRPK.instance.ShowHPPlayer_ServerRPC(player.GetComponent<Player>().ownerClientID.Value);
                            lose.gameObject.SetActive(true);
                        }
                        break;
                    case Result.Paper:
                        Debug.Log("Lose !!");
                        if (player.ownerClientID.Value == NetworkManager.Singleton.LocalClientId)
                        {
                            GameManagerRPK.instance.ShowHPPlayer_ServerRPC(player.GetComponent<Player>().ownerClientID.Value);
                            lose.gameObject.SetActive(true);
                        }
                        break;
                    case Result.Kunai:
                        Debug.Log("Lose !!");
                        if (player.ownerClientID.Value == NetworkManager.Singleton.LocalClientId)
                        {
                            GameManagerRPK.instance.ShowHPPlayer_ServerRPC(player.GetComponent<Player>().ownerClientID.Value);
                            lose.gameObject.SetActive(true);
                        }
                        break;
                    case Result.None:
                        Debug.Log("Lose !!");
                        if (player.ownerClientID.Value == NetworkManager.Singleton.LocalClientId)
                        {
                            GameManagerRPK.instance.ShowHPPlayer_ServerRPC(player.GetComponent<Player>().ownerClientID.Value);
                            lose.gameObject.SetActive(true);
                        }
                        break;
                    default:
                        break;
                }
                break;
            case Result.Clear:
                switch (theirResultTemp)
                {
                    case Result.Rock:
                        Debug.Log("U are not in Battle !!");
                        if (player.GetComponent<PlayerHeath>().health > 0)
                        {
                            notInBattle.gameObject.SetActive(true);
                        }
                        else
                        {
                            notInBattle.gameObject.SetActive(false);
                        }
                        break;
                    case Result.Paper:
                        Debug.Log("U are not in Battle !!");
                        if (player.GetComponent<PlayerHeath>().health > 0)
                        {
                            notInBattle.gameObject.SetActive(true);
                        }
                        else
                        {
                            notInBattle.gameObject.SetActive(false);
                        }
                        break;
                    case Result.Kunai:
                        Debug.Log("U are not in Battle !!");
                        if (player.GetComponent<PlayerHeath>().health > 0)
                        {
                            notInBattle.gameObject.SetActive(true);
                        }
                        else
                        {
                            notInBattle.gameObject.SetActive(false);
                        }
                        break;
                    case Result.None:
                        Debug.Log("U are not in Battle !!");
                        if (player.GetComponent<PlayerHeath>().health > 0)
                        {
                            notInBattle.gameObject.SetActive(true);
                        }
                        else
                        {
                            notInBattle.gameObject.SetActive(false);
                        }
                        break;
                    default:
                        break;
                }
                break;
            case Result.Start:
                switch (theirResultTemp)
                {
                    case Result.Rock:
                        Debug.Log("Game Start");
                        start.gameObject.SetActive(true);
                        break;
                    case Result.Paper:
                        Debug.Log("Game Start");
                        start.gameObject.SetActive(true);
                        break;
                    case Result.Kunai:
                        Debug.Log("Game Start");
                        start.gameObject.SetActive(true);
                        break;
                    case Result.None:
                        Debug.Log("Game Start");
                        start.gameObject.SetActive(true);
                        break;
                    default:
                        break;
                }
                break;
            default:
                break;
        }
    }

}
