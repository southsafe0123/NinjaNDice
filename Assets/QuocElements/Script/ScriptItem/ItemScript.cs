using System.Collections;
using System.Collections.Generic;
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
    public GameObject playerListDefense; // UI chứa danh sách các Button Defense
    public GameObject playerListCancelDefense; // UI chứa danh sách các Button CancelDefense
    public Button buttonChange; // Nút ButtonChange
    public Button buttonFrozen; // Nút ButtonFrozen
    public Button buttonMove;  // Nút ButtonMove
    public Button buttonDefense; // Nút ButtonDefense
    public Button buttonChangeP2; // Button đại diện cho Player2
    public Button buttonChangeP3; // Button đại diện cho Player3
    public Button buttonChangeP4; // Button đại diện cho Player4
    public Button buttonDefenseP2; // Button Player2
    public Button buttonDefenseP3; // Button Player3
    public Button buttonDefenseP4; // Button Player4
    public Button buttonCancelDefenseP2; // Button Player2
    public Button buttonCancelDefenseP3; // Button Player3
    public Button buttonCancelDefenseP4; // Button Player4
    public float moveSpeed = 2f; // Tốc độ di chuyển của Player1
    public TextMeshProUGUI player1Text; // TextMeshPro cho Player1
    public TextMeshProUGUI player2Text; // TextMeshPro cho Player2
    public TextMeshProUGUI player3Text; // TextMeshPro cho Player3
    public TextMeshProUGUI player4Text; // TextMeshPro cho Player4
    public GameObject defenseImagePrefab; // Prefab của Image dùng để thay thế Player
    private GameObject defenseImageInstance; // Instance của Image đã tạo

    private bool isMoving = false;
    private Vector3 initialPosition; // Lưu vị trí ban đầu của Player1
    private GameObject selectedPlayer; // Lưu trữ Player được chọn

    void Start()
    {
        // Ẩn danh sách Player ban đầu
        playerListUI.SetActive(false);
        playerListDefense.SetActive(false);
        playerListCancelDefense.SetActive(false);


        // Thêm sự kiện click cho nút
        buttonChange.onClick.AddListener(ShowPlayerList);
        buttonFrozen.onClick.AddListener(ShowPlayerListCancelDefense);
        buttonMove.onClick.AddListener(RandomDirectionPlayerMove);
        buttonDefense.onClick.AddListener(ShowPlayerListDefense);

        // Thêm sự kiện click cho các nút Player
        buttonChangeP2.onClick.AddListener(() => SelectPlayer(player2));
        buttonChangeP3.onClick.AddListener(() => SelectPlayer(player3));
        buttonChangeP4.onClick.AddListener(() => SelectPlayer(player4));

        buttonDefenseP2.onClick.AddListener(() => DefensePlayer(player2));
        buttonDefenseP3.onClick.AddListener(() => DefensePlayer(player3));
        buttonDefenseP4.onClick.AddListener(() => DefensePlayer(player4));

        buttonCancelDefenseP2.onClick.AddListener(() => CancelDefensePlayer(player2));
        buttonCancelDefenseP3.onClick.AddListener(() => CancelDefensePlayer(player3));
        buttonCancelDefenseP4.onClick.AddListener(() => CancelDefensePlayer(player4));


        // Kiểm tra xem các thành phần đã được gán hay chưa
        if (player1 == null) Debug.LogError("Player1 is not assigned.");
        if (buttonMove == null) Debug.LogError("ButtonMove is not assigned.");
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
    void ShowPlayerListDefense()
    {
        // Hiển thị danh sách Player
        playerListDefense.SetActive(true);
    }
    void ShowPlayerListCancelDefense()
    {
        // Hiển thị danh sách Player
        playerListCancelDefense.SetActive(true);
    }
    void HidePlayerList()
    {
        // Ẩn danh sách Player
        playerListUI.SetActive(false);
    }
    void HidePlayerListDefense()
    {
        // Ẩn danh sách Player
        playerListDefense.SetActive(false);
    }
    void HidePlayerListCancelDefense()
    {
        // Ẩn danh sách Player
        playerListCancelDefense.SetActive(false);
    }

    void SelectPlayer(GameObject player)
    {
        selectedPlayer = player;
        StartCoroutine(MoveAllPlayers());
        HidePlayerList();
    }

    IEnumerator MoveAllPlayers()
    {
        //kiểm tra nếu selectedPlayer còn active thì di chuyển, nếu không thì báo
        if (selectedPlayer.activeSelf == false)
        {
            Debug.Log("Player is not active");
            yield break;
        }
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

    void FrozenPlayer(GameObject player)
    {
        // object không di chuyển
        Debug.Log("FrozenPlayer called for " + player.name);
    }

    void MovePlayer(GameObject player, int direction)
    {
        // Di chuyển object tiến thêm một khoảng cách
        Debug.Log("MovePlayer called for " + player.name);
        // player.transform.Translate(Vector3.right * 1);
        if (direction == 0)
        {
            player.transform.Translate(Vector3.right * 1);
            Debug.Log("Player move right");
        }
        else if (direction == 1)
        {
            player.transform.Translate(Vector3.left * 1);
            Debug.Log("Player move left");
        }
    }
    void RandomDirectionPlayerMove()
    {
        int randomNum = Random.Range(0, 2);
        Debug.Log("RandomDirectionPlayerMove:" + randomNum);
        if (randomNum == 0)
        {
            MovePlayer(player1, 0);
        }
        else
        {
            MovePlayer(player1, 1);
        }
    }
    void DefensePlayer(GameObject player)
    {
        Debug.Log("DefensePlayer called for " + player.name);

        // Lưu vị trí của Player
        Vector3 playerPosition = player.transform.position;

        // Vô hiệu hóa Player
        player.SetActive(false);

        // Tạo một instance của Image tại vị trí của Player
        defenseImageInstance = Instantiate(defenseImagePrefab, playerPosition, Quaternion.identity);

        // Ẩn danh sách các Player trong trạng thái phòng thủ (nếu có chức năng này)
        HidePlayerListDefense();
    }
    void CancelDefensePlayer(GameObject player)
    {
        Debug.Log("CancelDefensePlayer called for " + player.name);

        // Hiện lại player
        player.SetActive(true);

        // Hủy instance của Image tại vị trí của Player
        if (defenseImageInstance != null)
        {
            Destroy(defenseImageInstance);
            defenseImageInstance = null; // Đặt về null để tránh hủy lại trong tương lai
        }
        HidePlayerListCancelDefense();
    }

}
