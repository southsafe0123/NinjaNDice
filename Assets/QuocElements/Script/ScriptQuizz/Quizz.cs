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

public class Quizz : NetworkBehaviour
{
    public TMP_Text questionText;
    public TMP_Text answerTexts;
    public NetworkVariable<int> numQ = new NetworkVariable<int>();
    public const string MAIN_GAMEPLAY_SCENE = "NamAn";

    public List<GameObject> answerObjects;

    public List<GameObject> standPos = new List<GameObject>();
    public List<Player> playerList = new List<Player>();
    public GameObject player;
    private List<Question> questions = new List<Question>();
    private int playerOrder = 0;
    public List<Player> playerLose = new List<Player>();

    public TMP_Text lifeText;
    // private float timeRemaining = 5f;
    // private bool timerIsRunning = false;
    int topPlayer = 1;
    private void Awake()
    {
        if (!IsHost) return;
        // playerList = GameObject.FindObjectsByType<Player>(sortMode: FindObjectsSortMode.None).ToList();
        // foreach (var player in PlayerList.Instance.playerDic)
        // {
        //     playerList.Add(player.Value);
        // }





    }
    void Start()
    {
        Debug.Log(NetworkManager.LocalClientId);
        player = PlayerList.Instance.GetPlayerDic_Value(NetworkManager.LocalClientId).gameObject;



        if (IsHost)
        {
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
        LoadQuestionsFromFile("Assets/QuocElements/Resources/test.txt");
        StartCoroutine(AutoLoadQuestions());
    }

    IEnumerator AutoLoadQuestions()
    {
        yield return new WaitForSeconds(1f); // Wait 1 second before starting the first countdown
        while (true)
        {
            yield return new WaitForSeconds(4f); // Wait 15 seconds before loading a new question
            LoadRandomQuestion();
            ResetGameObjectAnswer();
        }
    }

    // void Update()
    // {

    // }

    // [MenuItem("Tools/Write file")]
    // static void WriteString()
    // {
    //     string path = "Assets/QuocElements/Resources/test.txt";
    //     //Write some text to the test.txt file
    //     StreamWriter writer = new StreamWriter(path, true);
    //     writer.WriteLine("1,câu hỏi 1,a,b,c,d,1");
    //     writer.WriteLine("2,câu hỏi 2,a,b,c,d,2");
    //     writer.WriteLine("3,câu hỏi 3,a,b,c,d,3");
    //     writer.Close();
    //     //Re-import the file to update the reference in the editor
    //     AssetDatabase.ImportAsset(path);
    //     TextAsset asset = (TextAsset)Resources.Load("test");
    //     //Print the text from the file
    //     Debug.Log(asset.text);
    // }

    // [MenuItem("Tools/Read file")]
    public void ReadString()
    {
        string path = "Assets/QuocElements/Resources/test.txt";
        LoadQuestionsFromFile(path);
        LoadRandomQuestion();
    }

    public void TeleportPlayer(Player player, int value)
    {
        player.transform.position = standPos[value].transform.position;
    }

    private void LoadQuestionsFromFile(string path)
    {
        questions.Clear();
        string[] lines = File.ReadAllLines(path);
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

    private void LoadRandomQuestion(int q = 0)
    {
        //Random question
        if (questions.Count == 0)
        {
            Debug.LogError("No questions loaded from file.");
            return;
        }
        Question randomQuestion;
        if (q == 0)
        {
            if (IsServer) { numQ.Value = Random.Range(0, questions.Count); }
            randomQuestion = questions[numQ.Value];
        }
        else
        {
            randomQuestion = questions[q];
        }

        questionText.text = randomQuestion.text;
        AnswerPanel.instance.txtAnswer1.text = randomQuestion.answers[0];
        AnswerPanel.instance.txtAnswer2.text = randomQuestion.answers[1];
        AnswerPanel.instance.txtAnswer3.text = randomQuestion.answers[2];
        AnswerPanel.instance.txtAnswer4.text = randomQuestion.answers[3];

        StartCoroutine(WaitAndDisplayCorrectAnswer(randomQuestion));
    }

    private IEnumerator WaitAndDisplayCorrectAnswer(Question randomQuestion)
    {
        yield return new WaitUntil(() => AnswerPanel.instance.isPlayerClick);
        AnswerPanel.instance.isPlayerClick = false;
        // Call ChooseCorrectAnswer to display correct answer
        ChooseCorrectAnswer(randomQuestion);
    }

    private void ChooseCorrectAnswer(Question randomQuestion)
    {
        ShowAnswer(answerObjects[randomQuestion.correctAnswer - 1]);

        if (player.GetComponent<Player>().answer != randomQuestion.correctAnswer.ToString() || player.GetComponent<Player>().answer == null)
        {
            TakeDamage_ServerRPC(player.GetComponent<Player>().ownerClientID.Value);
        };
        lifeText.text = "Life: " + player.GetComponent<PlayerHeath>().health.ToString();
    }

    private void ResetGameObjectAnswer()
    {
        for (int i = 0; i < answerObjects.Count; i++)
        {
            answerObjects[i].gameObject.SetActive(true);
            answerObjects[i].GetComponent<Button>().interactable = true;
        }
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
        player.GetComponent<PlayerHeath>().health--;

        if (playerID == NetworkManager.Singleton.LocalClientId)
        {
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


