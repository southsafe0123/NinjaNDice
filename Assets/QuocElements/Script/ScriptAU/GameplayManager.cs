using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static PlayerList;

public class GameplayManager : NetworkBehaviour
{
    public const string MAIN_GAMEPLAY_SCENE = "NamAn";
    public static GameplayManager Instance;
    public GameObject[] objectPrefabs;
    public List<int> listNumber = new List<int>();
    public int currentObjectIndex = 0;
    public List<int> listUserInput = new List<int>();
    public List<GameObject> list1; // list mui ten
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
    private int playerOrder = 0;
    public Slider sliderTime;
    public GameObject gameInput;
    public bool isEndGame;

    bool isAllPlayerReady = false;
    private IEnumerator WaitForPlayer()
    {
        WaitForSeconds waithalfsecond = new WaitForSeconds(0.5f);
        isAllPlayerReady = false;
        while (true)
        {
            yield return waithalfsecond;
            foreach (Player player in PlayerList.Instance.playerDic.Values)
            {
                if (player.isReadySceneLoaded.Value == false)
                {
                    isAllPlayerReady = false;
                    break;
                }
                else
                {
                    yield return new WaitForSeconds(0.5f);
                    isAllPlayerReady = true;
                    break;
                }

            }

        }

    }
    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(WaitForPlayer());
        playerOrder = 0;
        objectMoveSpeed = defaultObjectMoveSpeed;
        LoadPlayer();
        GenerateRandomListNumber();
        Display();
        
    }

    private void LoadPlayer()
    {
        
        if (!IsHost) return;
        playerList = PlayerList.Instance.GetPlayerOrder();
        for (int i = 0; i < playerList.Count; i++)
        {
            playerList[i].gameObject.transform.position = map.movePos[i].position;
            playerList[i].isPlayerTurn.Value = false;
            AddComponent_ClientRPC(playerList[i].ownerClientID.Value);
        }
    }
    // [ClientRpc]
    // public void SortPlayerListByServer_ClientRPC(ulong clientID)
    // {
    //     if (IsHost) return;
    //     var temp = GameObject.FindObjectsByType<Player>(sortMode: FindObjectsSortMode.None).ToList();
    //     foreach (var item in temp)
    //     {
    //         if (item.ownerClientID.Value == clientID)
    //         {
    //             playerList.Add(item);
    //             break;
    //         }
    //     }
    // }
    [ClientRpc]
    private void AddComponent_ClientRPC(ulong clientID)
    {
        PlayerList.Instance.GetPlayerDic_Value(clientID).AddComponent<PlayerHeath>();
        PlayerList.Instance.GetPlayerDic_Value(clientID).GetComponent<PlayerHeath>().health = 3;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAllPlayerReady) return;
        if (NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerHeath>() == null)
        {
            canInput = false; // Không cho phép nhập thêm
            foreach (Transform child in gameInput.transform)
            {
                child.GetComponent<Button>().interactable = false;
            }
        }
        if (!isEndGame)
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
            }
        }

        if (canInput)
        {
            if (listUserInput.Count == 4 || (NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerHeath>() != null && NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerHeath>().isDead))
            {
                canInput = false; // Không cho phép nhập thêm
                foreach (Transform child in gameInput.transform)
                {
                    child.GetComponent<Button>().interactable = false;
                }
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

    public void AddInput(int num)
    {
        if (!canInput) return;
        listUserInput.Add(num);
        Display();
    }

    // hàm tái sử dụng đối tượng 
    private void RecycleObject(int index)
    {
        //if (!objectPrefabs[index].activeSelf)
        //{
        //    objectPrefabs[index].transform.position = new Vector3(12f, -2, 0);
        //    ClearDisplayLists();
        //    GenerateRandomListNumber();
        //    Display();
        //    canInput = true; // Cho phép nhập lại khi tái sử dụng đối tượng
        //}

        //objectPrefabs[index].SetActive(true);
        //objectPrefabs[index].transform.Translate(Vector3.left * objectMoveSpeed * Time.deltaTime);

        if (sliderTime.value == 0)
        {
            ClearDisplayLists();
            GenerateRandomListNumber();
            Display();
            canInput = true;
        }
        sliderTime.value += objectMoveSpeed / 100 * Time.deltaTime;
        if (sliderTime.value >= 1)
        {
            sliderTime.value = 0;
            objectMoveSpeed += objectMoveSpeedPlus;
            CompareListUserAndListEnemy();
            listUserInput.Clear();
            foreach (Transform child in gameInput.transform)
            {
                child.GetComponent<Button>().interactable = true;
            }
        }

        //if (objectPrefabs[index].transform.position.x < -15)
        //{
        //    objectMoveSpeed += objectMoveSpeedPlus;
        //    objectPrefabs[index].SetActive(false);
        //    currentObjectIndex = Random.Range(0, objectPrefabs.Length);
        //    CompareListUserAndListEnemy();
        //    listUserInput.Clear();
        //}
    }

    // hàm tạo danh sách số ngẫu nhiên
    private void GenerateRandomListNumber()
    {
        listNumber.Clear();
        for (int i = 0; i < 4; i++)
        {
            listNumber.Add(UnityEngine.Random.Range(0, 4));
        }
        //Debug.Log(string.Join(" ", listNumber));
    }

    // hàm so sánh danh sách người dùng và danh sách số ngẫu nhiên
    private void CompareListUserAndListEnemy()
    {
        if (!listUserInput.SequenceEqual(listNumber) || listUserInput.Count != 4 || listUserInput == null)
        {
            if (NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerHeath>()== null) return;
            if (NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerHeath>().health <1) return;
                Debug.Log("-1 health");
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
                TakeDamage_ClientRPC(clientID);
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
            if (player.GetComponent<PlayerHeath>().health == 0)
            {
                player.GetComponent<PlayerHeath>().isDead = true;
                isEndGame = true;
                CallThisPlayerIsDead_ServerRPC(playerID);
            }
        }

    }
    [ServerRpc(RequireOwnership = false)]
    private void CallThisPlayerIsDead_ServerRPC(ulong playerID)
    {

        var player = PlayerList.Instance.GetPlayerDic_Value(playerID);
        MiniEndGamePanel.instance.AddPlayerLose(player);

        if (MiniEndGamePanel.instance.playerLose.Count >= PlayerList.Instance.playerOrders.Count - 1) StartCoroutine(EndGame());
    }

    private IEnumerator EndGame()
    {
        isEndGame = true;
        Player playerWin = PlayerList.Instance.playerDic.First(player => !MiniEndGamePanel.instance.playerLose.Contains(player.Value)).Value != null ? PlayerList.Instance.playerDic.First(player => !MiniEndGamePanel.instance.playerLose.Contains(player.Value)).Value : null;
        Debug.Log("PlayerWin:" + playerWin.ownerClientID.Value);
        if (playerWin != null)
        {
            MiniEndGamePanel.instance.SetPlayerWin(playerWin);
            EndGameAnouncement_ClientRPC(playerWin.ownerClientID.Value);
        }
        MiniEndGamePanel.instance.playerLose.Reverse();
        yield return null;
        foreach (Player player in MiniEndGamePanel.instance.playerLose)
        {
            EndGameAnouncement_ClientRPC(player.ownerClientID.Value);
            yield return null;
        }

        RemovedComponent_ClientRPC();

        yield return new WaitForSeconds(3.5f);
        StartLoadScene_ClientRPC();
    }
    [ClientRpc]
    private void StartLoadScene_ClientRPC()
    {
            LoadScene.Instance.StartLoadSceneMultiplayer(MAIN_GAMEPLAY_SCENE, IsHost);
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
    private void EndGameAnouncement_ClientRPC(ulong playerID)
    {
        MiniEndGamePanel.instance.DisplayEndMinigame(true);
        MiniEndGamePanel.instance.DisplayPlayer(PlayerList.Instance.GetPlayerDic_Value(playerID));
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
