using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemScript : MonoBehaviour
{
    public GameObject player1;
    public GameObject player2;
    public GameObject player3;
    public GameObject player4;
    public GameObject playerListUI; // UI chứa danh sách các Button
    public Button buttonChange; // Nút ButtonChange
    public Button buttonP2; // Button đại diện cho Player2
    public Button buttonP3; // Button đại diện cho Player3
    public Button buttonP4; // Button đại diện cho Player4
    public float moveSpeed = 2f; // Tốc độ di chuyển của Player1
    public TextMeshProUGUI player1Text; // TextMeshPro cho Player1
    public TextMeshProUGUI player2Text; // TextMeshPro cho Player2
    public TextMeshProUGUI player3Text; // TextMeshPro cho Player3
    public TextMeshProUGUI player4Text; // TextMeshPro cho Player4

    private bool isMoving = false;
    private Vector3 initialPosition; // Lưu vị trí ban đầu của Player1
    private GameObject selectedPlayer; // Lưu trữ Player được chọn

    void Start()
    {
        // Ẩn danh sách Player ban đầu
        playerListUI.SetActive(false);

        // Thêm sự kiện click cho nút ButtonChange
        buttonChange.onClick.AddListener(ShowPlayerList);

        // Thêm sự kiện click cho các nút Player
        buttonP2.onClick.AddListener(() => SelectPlayer(player2));
        buttonP3.onClick.AddListener(() => SelectPlayer(player3));
        buttonP4.onClick.AddListener(() => SelectPlayer(player4));
    }

    void Update()
    {
        // Cập nhật vị trí của TextMeshPro để luôn theo dõi Player1
        player1Text.transform.position = Camera.main.WorldToScreenPoint(player1.transform.position + Vector3.up);
        player2Text.transform.position = Camera.main.WorldToScreenPoint(player2.transform.position + Vector3.up);
        player3Text.transform.position = Camera.main.WorldToScreenPoint(player3.transform.position + Vector3.up);
        player4Text.transform.position = Camera.main.WorldToScreenPoint(player4.transform.position + Vector3.up);
    }

    void ShowPlayerList()
    {
        // Hiển thị danh sách Player
        playerListUI.SetActive(true);
    }

    void SelectPlayer(GameObject player)
    {
        selectedPlayer = player;
        StartCoroutine(MoveAllPlayers());
    }

    IEnumerator MoveAllPlayers()
    {
        // Di chuyển Player1 đến vị trí của selectedPlayer
        yield return MoveToPosition(player1, selectedPlayer.transform.position);

        // Di chuyển selectedPlayer đến vị trí ban đầu của Player1
        yield return MoveToPosition(selectedPlayer, initialPosition);
    }

    IEnumerator MoveToPosition(GameObject player, Vector3 targetPosition)
    {
        if (isMoving)
            yield break;

        isMoving = true;
        playerListUI.SetActive(false);

        // Lưu vị trí ban đầu của Player1
        if (player == player1)
        {
            initialPosition = player.transform.position;
        }

        // Di chuyển dần dần tới vị trí mục tiêu
        while (Vector3.Distance(player.transform.position, targetPosition) > 0.1f)
        {
            player.transform.position = Vector3.MoveTowards(player.transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }

        // Đảm bảo vị trí cuối cùng là vị trí mục tiêu
        player.transform.position = targetPosition;

        isMoving = false;
    }
}
