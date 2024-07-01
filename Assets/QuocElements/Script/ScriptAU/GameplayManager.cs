using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    public GameObject[] objectPrefabs;
    public List<int> listNumber = new List<int>();
    public int currentObjectIndex = 0;
    public List<int> listUser = new List<int>();

    public List<GameObject> list1;
    public List<GameObject> list2;
    public List<GameObject> list3;
    public List<GameObject> list4;

    private bool canInput = true; // Biến để kiểm tra xem có thể xử lý đầu vào mới hay không

    // Start is called before the first frame update
    void Start()
    {
        GenerateRandomListNumber();
        Display();
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

            if (listUser.Count == 4)
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
            listUser.Add(2);
            Display();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            listUser.Add(3);
            Display();
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            listUser.Add(0);
            Display();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            listUser.Add(1);
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
        objectPrefabs[index].transform.Translate(Vector3.left * 3 * Time.deltaTime);

        if (objectPrefabs[index].transform.position.x < -15)
        {
            objectPrefabs[index].SetActive(false);
            currentObjectIndex = Random.Range(0, objectPrefabs.Length);
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
        Debug.Log(string.Join(" ", listNumber));
    }

    // hàm so sánh danh sách người dùng và danh sách số ngẫu nhiên
    private void CompareListUserAndListEnemy()
    {
        if (listUser.SequenceEqual(listNumber))
        {
            Debug.Log("Win");
        }
        else
        {
            Debug.Log("Lose");
        }
        listUser.Clear();
    }

    // hàm hiển thị các đối tượng

    private void Display()
    {
        if (listUser.Count == 0)
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
        for (int i = 0; i < listUser.Count; i++)
        {
            int userInput = listUser[i];
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
