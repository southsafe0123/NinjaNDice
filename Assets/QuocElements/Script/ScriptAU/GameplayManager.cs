using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplayManager : NetworkBehaviour
{
    public const string MAIN_GAMEPLAY_SCENE = "NamAn";
    public GameplayManager Instance;
    public GameObject[] objectPrefabs;
    public List<int> listNumber = new List<int>();
    public int currentObjectIndex = 0;
    public List<int> listUserInput = new List<int>();
    public List<GameObject> list1;
    public List<GameObject> list2;
    public List<GameObject> list3;
    public List<GameObject> list4;
    private bool canInput = true; // Biến để kiểm tra xem có thể xử lý đầu vào mới hay không

    public List<Player> playerList = new List<Player>();
    public List<Player> playerLose = new List<Player>();
    public Map map;
    private float objectMoveSpeed;
    public float defaultObjectMoveSpeed;
    public float objectMoveSpeedPlus;
    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        objectMoveSpeed = defaultObjectMoveSpeed;
        LoadPlayer();
        GenerateRandomListNumber();
        Display();
    }

    private void LoadPlayer()
    {
        if (!IsHost) return;
        playerList = GameObject.FindObjectsByType<Player>(sortMode: FindObjectsSortMode.None).ToList();
        foreach (Player player in playerList)
        {
            SortPlayerListByServer_ClientRPC(player.ownerClientID.Value);
        }
        for (int i = 0; i < playerList.Count; i++)
        {
            playerList[i].gameObject.transform.position = map.movePos[i].position;
            AddComponent_ClientRPC(i);
        }
    }
    [ClientRpc]
    public void SortPlayerListByServer_ClientRPC(ulong clientID)
    {
        if (IsHost) return;
        var temp = GameObject.FindObjectsByType<Player>(sortMode: FindObjectsSortMode.None).ToList();
        foreach (var item in temp)
        {
            if (item.ownerClientID.Value == clientID)
            {
                playerList.Add(item);
                break;
            }
        }
    }
    [ClientRpc]
    private void AddComponent_ClientRPC(int i)
    {
        playerList[i].AddComponent<PlayerHeath>();
        playerList[i].GetComponent<PlayerHeath>().health = 3;
    }

    // Update is called once per frame
    void Update()
    {
        // tái sử dụng đối tượng
        if (objectPrefabs.Count(x => x.activeSelf) <= 1)
        {
            RecycleObject(currentObjectIndex);
        }

        // xử lý đầu vào khi canInput là true
        if (canInput)
        {
            HandleInput();

            if (listUserInput.Count == 4)
            {
                canInput = false; // Không cho phép nhập thêm
                CompareListUserAndListEnemy();
            }
        }
    }

    // hàm xử lý đầu vào
    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            listUserInput.Add(2);
            Display();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            listUserInput.Add(3);
            Display();
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            listUserInput.Add(0);
            Display();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            listUserInput.Add(1);
            Display();
        }
    }

    // hàm tái sử dụng đối tượng 
    private void RecycleObject(int index)
    {
        if (!objectPrefabs[index].activeSelf)
        {
            objectPrefabs[index].transform.position = new Vector3(12f, -2, 0);
            ClearDisplayLists();
            GenerateRandomListNumber();
            Display();
            canInput = true; // Cho phép nhập lại khi tái sử dụng đối tượng
        }

        objectPrefabs[index].SetActive(true);
        objectPrefabs[index].transform.Translate(Vector3.left * objectMoveSpeed * Time.deltaTime);

        if (objectPrefabs[index].transform.position.x < -15)
        {
            objectMoveSpeed += objectMoveSpeedPlus;
            objectPrefabs[index].SetActive(false);
            currentObjectIndex = Random.Range(0, objectPrefabs.Length);
            CompareListUserAndListEnemy();
            listUserInput.Clear();
        }
    }

    // hàm tạo danh sách số ngẫu nhiên
    private void GenerateRandomListNumber()
    {
        listNumber.Clear();
        for (int i = 0; i < 4; i++)
        {
            listNumber.Add(Random.Range(0, 4));
        }
        //Debug.Log(string.Join(" ", listNumber));
    }

    // hàm so sánh danh sách người dùng và danh sách số ngẫu nhiên
    private void CompareListUserAndListEnemy()
    {
        if (!listUserInput.SequenceEqual(listNumber)|| listUserInput.Count != 4 || listUserInput==null)
        {
            Debug.Log("Lose");
            TakeDamage_ServerRPC(NetworkManager.Singleton.LocalClientId);
        }
    }
    [ServerRpc(RequireOwnership = false)]
    public void TakeDamage_ServerRPC(ulong clientID)
    {
        for (int i = 0; i < playerList.Count; i++)
        {
            if (playerList[i].ownerClientID.Value == clientID)
            {
                TakeDamage_ClientRPC(i);
            }
        }
    }
    [ClientRpc]
    private void TakeDamage_ClientRPC(int i)
    {
        playerList[i].GetComponent<PlayerHeath>().health--;
        if (playerList[i].GetComponent<PlayerHeath>().health == 0)
        {
            playerList[i].GetComponent<PlayerHeath>().isDead = true;
            playerLose.Add(playerList[i]);
            if (playerLose.Count == playerList.Count - 1) EndGame();
        }
    }

    private void EndGame()
    {
        foreach(Player player in playerList)
        {
            if (!player.GetComponent<PlayerHeath>().isDead)
            {
                Debug.LogError("Player Win: " + player.ownerClientID.Value);
            }

            Destroy(player.GetComponent<PlayerHeath>());
        }
        foreach (Player player in playerLose)
        {
            Debug.LogError("Player lose: "+player.ownerClientID.Value);
        }

        if (NetworkManager.Singleton.IsServer)
        {
            NetworkManager.Singleton.SceneManager.LoadScene(MAIN_GAMEPLAY_SCENE, LoadSceneMode.Single);
        }
    }


    // hàm hiển thị các đối tượng

    private void Display()
    {
        if (listUserInput.Count == 0)
        {
            list1[listNumber[0]].SetActive(true);
            list2[listNumber[1]].SetActive(true);
            list3[listNumber[2]].SetActive(true);
            list4[listNumber[3]].SetActive(true);
        }
        else
        {
            SetActiveBasedOnUserInput();
        }
    }
    // hàm hiển thị các đối tượng khi người dùng nhập 
    private void SetActiveBasedOnUserInput()
    {
        for (int i = 0; i < listUserInput.Count; i++)
        {
            int userInput = listUserInput[i];
            int randomValue = listNumber[i];

            if (userInput == randomValue)
            {
                switch (i)
                {
                    case 0:
                        list1[randomValue + 4].SetActive(true);
                        break;
                    case 1:
                        list2[randomValue + 4].SetActive(true);
                        break;
                    case 2:
                        list3[randomValue + 4].SetActive(true);
                        break;
                    case 3:
                        list4[randomValue + 4].SetActive(true);
                        break;
                }
            }
        }
    }

    // hàm làm sạch danh sách
    private void ClearDisplayLists()
    {
        ClearList(list1);
        ClearList(list2);
        ClearList(list3);
        ClearList(list4);
    }

    // hàm làm sạch danh sách đối tượng
    private void ClearList(List<GameObject> list)
    {
        foreach (var obj in list)
        {
            obj.SetActive(false);
        }
    }
}
