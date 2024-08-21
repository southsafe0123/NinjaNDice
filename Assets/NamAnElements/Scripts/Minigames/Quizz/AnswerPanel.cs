using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class AnswerPanel : MonoBehaviour
{
    public static AnswerPanel instance;
    public Button answer1;
    public TextMeshProUGUI txtAnswer1;
    public Button answer2;
    public TextMeshProUGUI txtAnswer2;
    public Button answer3;
    public TextMeshProUGUI txtAnswer3;
    public Button answer4;
    public TextMeshProUGUI txtAnswer4;

    public bool isPlayerClick;
    private void Awake()
    {
        instance = this;
        isPlayerClick = false;
    }
    private void Start()
    {
        answer1.onClick.AddListener(() => { NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Player>().answer = "1"; isPlayerClick = true; });
        answer2.onClick.AddListener(() => { NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Player>().answer = "2"; isPlayerClick = true; });
        answer3.onClick.AddListener(() => { NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Player>().answer = "3"; isPlayerClick = true; });
        answer4.onClick.AddListener(() => { NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Player>().answer = "4"; isPlayerClick = true; });
    }
}
