using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using TMPro;
using Unity.Netcode;

public class Quizz : NetworkBehaviour
{
    public TMP_Text questionText;
    public TMP_Text answerTexts;
    public NetworkVariable<int> numQ = new NetworkVariable<int>();

    public GameObject answer1;
    public GameObject answer2;
    public GameObject answer3;
    public GameObject answer4;
    public List<GameObject> PlayerList = new List<GameObject>();
    public GameObject player;





    private List<Question> questions = new List<Question>();
    // private float timeRemaining = 5f;
    // private bool timerIsRunning = false;

    void Start()
    {
        player = NetworkManager.LocalClient.PlayerObject.gameObject;

        LoadQuestionsFromFile("Assets/QuocElements/Resources/test.txt");
        StartCoroutine(AutoLoadQuestions());
    }

    IEnumerator AutoLoadQuestions()
    {
        yield return new WaitForSeconds(1f); // Wait 1 second before starting the first countdown
        while (true)
        {
            yield return new WaitForSeconds(15f); // Wait 15 seconds before loading a new question
            LoadRandomQuestion();
            ResetGameObjectAnswer();
        }
    }

    void Update()
    {

    }

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
        if (!player.GetComponent<Player>().answerGameObject.name.EndsWith(randomQuestion.correctAnswer.ToString()))
        {
            player.GetComponent<Player>().WrongAnswer();
        };
        switch (randomQuestion.correctAnswer)
        {
            case 1:
                {
                    answer1.SetActive(true);
                    answer2.SetActive(false);
                    answer3.SetActive(false);
                    answer4.SetActive(false);
                }
                break;
            case 2:
                {
                    answer2.SetActive(true);
                    answer1.SetActive(false);
                    answer3.SetActive(false);
                    answer4.SetActive(false);
                }
                break;
            case 3:
                {
                    answer3.SetActive(true);
                    answer2.SetActive(false);
                    answer1.SetActive(false);
                    answer4.SetActive(false);
                }
                break;
            case 4:
                {
                    answer4.SetActive(true);
                    answer2.SetActive(false);
                    answer3.SetActive(false);
                    answer1.SetActive(false);
                }
                break;
            default:
                break;
        }
    }




    private void ResetGameObjectAnswer()
    {
        answer1.SetActive(true);
        answer2.SetActive(true);
        answer3.SetActive(true);
        answer4.SetActive(true);
    }
}


