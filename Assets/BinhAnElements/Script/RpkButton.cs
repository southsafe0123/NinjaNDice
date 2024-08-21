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
    public GameObject resultLeft;
    public GameObject resultRight;
    public Button rockButton;
    public Button paperButton;
    public Button kunaiButton;
    public GameObject time;
    public GameObject win;
    public GameObject lose;
    public GameObject draw;
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

    [SerializeField] TextMeshProUGUI timerText;
    float remainingTime;

    private void Start()
    {
        //scroll = theirResult.transform.Find("Scroll");
        //kunai = theirResult.transform.Find("Kunai");
        //rock = theirResult.transform.Find("Rock");
        StartCoroutine(WaitForPlayerChoose());
    }

    IEnumerator WaitForPlayerChoose()
    {
        int i = 5;
        time.SetActive(true);
        WaitForSeconds waitOneSecond = new WaitForSeconds(1);
        while (GameManagerRPK.instance.checkPlayerInFight.Count > 1)
        {
            yield return waitOneSecond;
            remainingTime = i;
            timerText.text = remainingTime.ToString();
            time.SetActive(true);
            Debug.Log(i);
            i--;
            if (i < 0)
            {
                i = 5;
                yield return new WaitForSeconds(1);
                time.SetActive(false);
                rockButton.gameObject.SetActive(false);
                paperButton.gameObject.SetActive(false);
                kunaiButton.gameObject.SetActive(false);
                yield return new WaitForSeconds(1);
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
                yield return new WaitForSeconds(1);
                OnClickCheckResult();

                yield return new WaitForSeconds(2);
                foreach (Transform child in theirResult.transform)
                {
                    theirResultTemp = Result.None;
                    resultName = "";
                }
                foreach (Transform child in myResult.transform)
                {  
                    myResultTemp = Result.None;
                    resultName = "";
                }

                lose.gameObject.SetActive(false);
                win.gameObject.SetActive(false);
                draw.gameObject.SetActive(false);

                yield return new WaitForSeconds(1);
                GameManagerRPK.instance.PickingPlayerList();

                yield return null;
                yield return new WaitForSeconds(2);
                GameManagerRPK.instance.TelePlayerToFightMap(GameManagerRPK.instance.playerRange);
                Debug.Log("Số lượng player ơ trong random là: " + GameManagerRPK.instance.checkPlayerInFight.Count);

                yield return new WaitForSeconds(1);
                if (GameManagerRPK.instance.checkPlayerInFight.Count == 1)
                {
                    Debug.Log("End Game Rồi !!");
                    rockButton.gameObject.SetActive(false);
                    paperButton.gameObject.SetActive(false);
                    kunaiButton.gameObject.SetActive(false);
                }
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
            rockButton.onClick.AddListener(() => OnRockButtonClick(resultRight));
            paperButton.onClick.AddListener(() => OnPaperButtonClick(resultRight));
            kunaiButton.onClick.AddListener(() => OnKunaiButtonClick(resultRight));
            myResult = resultRight;
            theirResult = resultLeft;
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
            else
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

    public enum Result
    {
        Rock, Paper, Kunai, None
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
                        //scroll.gameObject.SetActive(true);
                            break;
                    case "Rock": 
                        theirResultTemp = Result.Rock;
                        //rock.gameObject.SetActive(true);
                        break;
                    case "Kunai": 
                        theirResultTemp = Result.Kunai;
                        //rock.gameObject.SetActive(true);
                        break;
                    case "": theirResultTemp = Result.None; break;
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
                    case "Scroll": myResultTemp = Result.Paper; break;
                    case "Rock": myResultTemp = Result.Rock; break;
                    case "Kunai": myResultTemp = Result.Kunai; break;
                    case "": theirResultTemp = Result.None; break;
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
                        var healthTemp = player.GetComponent<PlayerHeath>().health-1;
                        GameManagerRPK.instance.ShowHPPlayer_ServerRPC(healthTemp, player.GetComponent<Player>().ownerClientID.Value);
                        lose.gameObject.SetActive(true);
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
                        var healthTemp = player.GetComponent<PlayerHeath>().health - 1;
                        GameManagerRPK.instance.ShowHPPlayer_ServerRPC(healthTemp, player.GetComponent<Player>().ownerClientID.Value);
                        lose.gameObject.SetActive(true);
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
                        var healthTemp = player.GetComponent<PlayerHeath>().health - 1;
                        GameManagerRPK.instance.ShowHPPlayer_ServerRPC(healthTemp, player.GetComponent<Player>().ownerClientID.Value);
                        lose.gameObject.SetActive(true);
                        break;
                    case Result.Paper:
                        Debug.Log("Win !!");
                        win.gameObject.SetActive(true);
                        break;
                    case Result.Kunai:
                        Debug.Log("Draw !!");
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
                        var healthTemp = player.GetComponent<PlayerHeath>().health - 1;
                        GameManagerRPK.instance.ShowHPPlayer_ServerRPC(healthTemp, player.GetComponent<Player>().ownerClientID.Value);
                        lose.gameObject.SetActive(true);
                        break;
                    case Result.Paper:
                        Debug.Log("Lose !!");
                        healthTemp = player.GetComponent<PlayerHeath>().health - 1;
                        GameManagerRPK.instance.ShowHPPlayer_ServerRPC(healthTemp, player.GetComponent<Player>().ownerClientID.Value);
                        lose.gameObject.SetActive(true);
                        break;
                    case Result.Kunai:
                        Debug.Log("Lose !!");
                        healthTemp = player.GetComponent<PlayerHeath>().health - 1;
                        GameManagerRPK.instance.ShowHPPlayer_ServerRPC(healthTemp, player.GetComponent<Player>().ownerClientID.Value);
                        lose.gameObject.SetActive(true);
                        break;
                    case Result.None:
                        Debug.Log("Draw !!");
                        draw.gameObject.SetActive(true);
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
