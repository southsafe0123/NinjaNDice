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

public class Quizz : NetworkBehaviour
{
    public TMP_Text questionText;
    public TMP_Text answerTexts;
    public NetworkVariable<int> numQ = new NetworkVariable<int>();
    public const string MAIN_GAMEPLAY_SCENE = "NamAn";


    public GameObject answer1;
    public GameObject answer2;
    public GameObject answer3;
    public GameObject answer4;
    public List<NetworkObject> networkObjects;

    public List<GameObject> standPos = new List<GameObject>();
    public List<Player> playerList = new List<Player>();
    public GameObject player;
    private List<Question> questions = new List<Question>();
    private int playerOrder = 0;
    public List<Player> playerLose = new List<Player>();


    public TMP_Text lifeText;
    // private float timeRemaining = 5f;
    // private bool timerIsRunning = false;

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
        networkObjects.Add(answer1.GetComponent<NetworkObject>());
        networkObjects.Add(answer2.GetComponent<NetworkObject>());
        networkObjects.Add(answer3.GetComponent<NetworkObject>());
        networkObjects.Add(answer4.GetComponent<NetworkObject>());
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
            yield return new WaitForSeconds(7f); // Wait 15 seconds before loading a new question
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
        answerTexts.text = " A : " + randomQuestion.answers[0] + " B : " + randomQuestion.answers[1] + " C : " + randomQuestion.answers[2] + " D : " + randomQuestion.answers[3];


        StartCoroutine(WaitAndDisplayCorrectAnswer(randomQuestion));
    }

    private IEnumerator WaitAndDisplayCorrectAnswer(Question randomQuestion)
    {
        yield return new WaitForSeconds(5f);

        // Call ChooseCorrectAnswer to display correct answer
        ChooseCorrectAnswer(randomQuestion);
    }

    private void ChooseCorrectAnswer(Question randomQuestion)
    {
        ShowAnswer(networkObjects[randomQuestion.correctAnswer - 1]);

        if (player.GetComponent<Player>().answer != randomQuestion.correctAnswer.ToString() || player.GetComponent<Player>().answer == null)
        {
            TakeDamage_ServerRPC(player.GetComponent<Player>().ownerClientID.Value);

        };

        lifeText.text = "Life: " + player.GetComponent<PlayerHeath>().health.ToString();

    }




    private void ResetGameObjectAnswer()
    {
        for (int i = 0; i < networkObjects.Count; i++)
        {
            networkObjects[i].gameObject.SetActive(true);
        }
    }

    public void ShowAnswer(NetworkObject answer)
    {
        for (int i = 0; i < networkObjects.Count; i++)
        {
            if (networkObjects[i].NetworkObjectId != answer.NetworkObjectId)
            {
                networkObjects[i].gameObject.SetActive(false);
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
            }
        }
    }
    [ClientRpc]
    private void TakeDamage_ClientRPC(ulong playerID)
    {
        var player = PlayerList.Instance.GetPlayerDic_Value(playerID);
        player.GetComponent<PlayerHeath>().health--;
        if (player.GetComponent<PlayerHeath>().health == 0)
        {
            player.GetComponent<PlayerHeath>().isDead = true;
            Debug.Log("Player: " + playerID + " lose");
            CallThisPlayerIsDead_ServerRPC(playerID);
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void CallThisPlayerIsDead_ServerRPC(ulong playerID)
    {
        var player = PlayerList.Instance.GetPlayerDic_Value(playerID);
        playerLose.Add(player);
        if (playerLose.Count >= playerList.Count - 1) EndGame();
    }

    private void EndGame()
    {
        foreach (Player player in playerList)
        {
            if (!player.GetComponent<PlayerHeath>().isDead)
            {
                PlayerList.Instance.SetPlayerOrder(playerOrder, player);
                playerOrder++;
                CallEndGame_ClientRPC(player.ownerClientID.Value, true);
            }
        }
        foreach (Player player in playerLose)
        {
            CallEndGame_ClientRPC(player.ownerClientID.Value, false);
        }

        var reversePlayerList = playerLose.ToArray().Reverse();
        foreach (Player player in reversePlayerList)
        {
            PlayerList.Instance.SetPlayerOrder(playerOrder, player);
            playerOrder++;
        }

        RemovedComponent_ClientRPC();

        if (NetworkManager.Singleton.IsServer)
        {
            NetworkManager.Singleton.SceneManager.LoadScene(MAIN_GAMEPLAY_SCENE, LoadSceneMode.Single);
        }
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


