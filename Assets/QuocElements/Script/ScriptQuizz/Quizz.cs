using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using TMPro;
using Unity.Netcode;
using System.Linq;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using UnityEngine.UI;
using WebSocketSharp;
using System;

public class Quizz : NetworkBehaviour
{
    public TMP_Text questionText;
    public TMP_Text answerTexts;
    public TextAsset textFile;
    public int numQ;
    public const string MAIN_GAMEPLAY_SCENE = "NamAn";

    public List<GameObject> answerObjects;

    public List<GameObject> standPos = new List<GameObject>();
    public List<Player> playerList = new List<Player>();
    public GameObject player;
    private List<Question> questions = new List<Question>();
    private int playerOrder = 0;
    public List<Player> playerLose = new List<Player>();
    public TextMeshProUGUI txtTimer;
    public Image imgTimer;
    public TMP_Text lifeText;
    // private float timeRemaining = 5f;
    // private bool timerIsRunning = false;
    int topPlayer = 1;
    void Start()
    {
        Debug.Log(NetworkManager.LocalClientId);
        player = PlayerList.Instance.GetPlayerDic_Value(NetworkManager.LocalClientId).gameObject;
        AnswerPanel.instance.player = player.GetComponent<Player>();

        if (IsHost)
        {
            player.GetComponent<Player>().isPlayerTurn.Value = true;
            foreach (var player in PlayerList.Instance.playerDic)
            {
                playerList.Add(player.Value);
            }
            for (int i = 0; i < PlayerList.Instance.playerDic.Count; i++)
            {
                TeleportPlayer(playerList[i], i);
                playerList[i].isPlayerTurn.Value = false;
                AddComponent_ClientRPC(playerList[i].ownerClientID.Value);
            }
        }
        lifeText.text = "Life: " + player.GetComponent<PlayerHeath>().health.ToString();
        LoadQuestionsFromFile(textFile);
        StartCoroutine(AutoLoadQuestions());
    }

    IEnumerator AutoLoadQuestions()
    {
        yield return new WaitUntil(() => LoadScene.Instance.isAllPlayerReady);
        WaitForSeconds wait = new WaitForSeconds(1);
        yield return wait; // Wait 1 second before starting the first countdown
        ResetGameObjectAnswer();
        while (true)
        {
            //Random question
            
            Question randomQuestion;

            numQ = UnityEngine.Random.Range(0, questions.Count);
            randomQuestion = questions[numQ];

            questionText.text = randomQuestion.text;
            AnswerPanel.instance.txtAnswer1.text = randomQuestion.answers[0];
            AnswerPanel.instance.txtAnswer2.text = randomQuestion.answers[1];
            AnswerPanel.instance.txtAnswer3.text = randomQuestion.answers[2];
            AnswerPanel.instance.txtAnswer4.text = randomQuestion.answers[3];

            yield return StartCoroutine(WaitAndDisplayCorrectAnswer(randomQuestion));
            if (topPlayer != 1) break;
            yield return wait;
            ResetGameObjectAnswer();
        }
    }

    public void TeleportPlayer(Player player, int value)
    {
        player.transform.position = standPos[value].transform.position;
    }

    private void LoadQuestionsFromFile(TextAsset file)
    {
        questions.Clear();
        string[] lines = file.text.Split('\n');
        foreach (string line in lines)
        {
            string[] parts = line.Split(',');
            Question question = new Question();
            question.id = int.Parse(parts[0]);
            question.text = parts[1];
            question.answers = new string[4];
            question.answers[0] = parts[2];
            question.answers[1] = parts[3];
            question.answers[2] = parts[4];
            question.answers[3] = parts[5];
            question.correctAnswer = int.Parse(parts[6]);
            questions.Add(question);
        }
    }

    private IEnumerator WaitAndDisplayCorrectAnswer(Question randomQuestion)
    {
        float timer = 0;
        int timeRemain = 0;
        WaitForSeconds wait = new WaitForSeconds(1);
        while(timer<5)
        {
            yield return null;
            if (AnswerPanel.instance.isPlayerClick) break;
            timeRemain = 5 - Mathf.FloorToInt(timer);
            timer+= Time.deltaTime;
            txtTimer.text = ""+ timeRemain;
            imgTimer.fillAmount = timer / 5;
            //Debug.Log("timer: " + Mathf.FloorToInt(timer));
        }
        bool isPlayerClickOrTimeout = AnswerPanel.instance.isPlayerClick || timer >= 5;
        yield return new WaitUntil(() => isPlayerClickOrTimeout);
        AnswerPanel.instance.isPlayerClick = false;
        // Call ChooseCorrectAnswer to display correct answer
        ChooseCorrectAnswer(randomQuestion);
    }

    private void ChooseCorrectAnswer(Question randomQuestion)
    {
        ShowAnswer(answerObjects[randomQuestion.correctAnswer - 1]);

        if (player.GetComponent<Player>().answer != randomQuestion.correctAnswer.ToString() || player.GetComponent<Player>().answer.IsNullOrEmpty())
        {
            TakeDamage_ServerRPC(player.GetComponent<Player>().ownerClientID.Value);
        };
    }

    private void ResetGameObjectAnswer()
    {
        for (int i = 0; i < answerObjects.Count; i++)
        {
            answerObjects[i].gameObject.SetActive(true);
            answerObjects[i].GetComponent<Button>().interactable = true;
        }
        AnswerPanel.instance.player.answer = "";
    }

    public void ShowAnswer(GameObject answer)
    {
        for (int i = 0; i < answerObjects.Count; i++)
        {
            if (answerObjects[i] != answer)
            {
                answerObjects[i].gameObject.SetActive(false);
            }
            else
            {
                answerObjects[i].GetComponent<Button>().interactable = false;
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void TakeDamage_ServerRPC(ulong clientID)
    {
        for (int i = 0; i < playerList.Count; i++)
        {
            if (playerList[i].ownerClientID.Value == clientID)
            {
                TakeDamage_ClientRPC(clientID);
                break;
            }
        }
    }
    [ClientRpc]
    private void TakeDamage_ClientRPC(ulong playerID)
    {
        var player = PlayerList.Instance.GetPlayerDic_Value(playerID);
        if (player.GetComponent<PlayerHeath>().isDead) 
        {
            AudioManager.Instance.PlaySFXOutOfHealth();
            return;
        }
        player.GetComponent<PlayerHeath>().health--;
        AudioManager.Instance.PlaySFXTakeHp();
        player.DisplayCurrentHealth();
        if (playerID == NetworkManager.Singleton.LocalClientId)
        {
            LifeRemainPanel.instance.UpdateHealth();
            Debug.Log("Myplayer: " + NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerHeath>().health);
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

        if (MiniEndGamePanel.Instance.playerLose.Count >= PlayerList.Instance.playerOrders.Count - 1) StartCoroutine(EndGame());
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

    [ClientRpc]
    private void AddComponent_ClientRPC(ulong clientID)
    {
        PlayerList.Instance.GetPlayerDic_Value(clientID).AddComponent<PlayerHeath>();
        PlayerList.Instance.GetPlayerDic_Value(clientID).GetComponent<PlayerHeath>().health = 3;
    }

}


