using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameLuckyController : MonoBehaviour
{
    public List<GameObject> playerList;
    public List<Button> buttonsList;
    private bool isRandom = false;
    // public TextMeshProUGUI message;
    private GameObject selectedPlayer;
    private Button selectedButton;

    // Start is called before the first frame update
    void Start()
    {
        RandomlySelectPlayer();
        RandomButton();

        // Đăng ký sự kiện click cho các button
        foreach (Button button in buttonsList)
        {
            button.onClick.AddListener(() => OnButtonClick(button));
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void RandomButton()
    {
        if (buttonsList.Count == 0)
        {
            Debug.Log("No buttons left to choose from.");
            return;
        }

        int randomIndex = Random.Range(0, buttonsList.Count);
        selectedButton = buttonsList[randomIndex];
        Debug.Log("Selected Button: " + selectedButton.name);
    }

    void RandomlySelectPlayer()
    {
        if (playerList.Count == 0)
        {
            Debug.Log("No players left to choose from.");
            return;
        }

        // Ngẫu nhiên chọn một index từ danh sách playerList
        int randomIndex = Random.Range(0, playerList.Count);
        selectedPlayer = playerList[randomIndex];

        // Hiển thị hoặc sử dụng player đã chọn theo yêu cầu của bạn
        Debug.Log("Selected Player: " + selectedPlayer.name);

        // Loại bỏ player đã chọn khỏi danh sách
        playerList.RemoveAt(randomIndex);

        // Tạm thời tắt hoạt động của các player còn lại
        foreach (GameObject player in playerList)
        {
            player.SetActive(false);
        }

        // Kích hoạt player đã chọn
        selectedPlayer.SetActive(true);
    }

    void OnButtonClick(Button clickedButton)
    {
        if (clickedButton == selectedButton)
        {
            ShowMessage("You clicked the selected button: " + clickedButton.name);
            //random lại button
            RandomButton();
            //bật tất cả các button click
            foreach (Button button in buttonsList)
            {
                button.interactable = true;
            }
        }
        else
        {
            Debug.Log("Clicked Button: " + clickedButton.name);
            //hủy button click
            clickedButton.interactable = false;
        }
    }

    void ShowMessage(string text)
    {
        Debug.Log(text);
    }
}
