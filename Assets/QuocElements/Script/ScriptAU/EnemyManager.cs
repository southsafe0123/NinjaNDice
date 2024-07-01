using System.Collections;
using System.Collections.Generic;
using System.Linq;
using QFSW.QC;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public GameObject[] objectPrefabs;
    public List<int> listNumber;
    public int i = 0;
    public List<int> listUser;

    public List<GameObject> list1;
    public List<GameObject> list2;
    public List<GameObject> list3;
    public List<GameObject> list4;




    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        //if objectPrefabs có ít nhất 1 object active true chạy hàm recycleObject
        if (objectPrefabs.Count(x => x.activeSelf) <= 1)
        {
            recycleObject(i);
            // add Random listNumber

        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            listUser.Add(2);
            Display();
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            listUser.Add(3);
            Display();

        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            listUser.Add(0);
            Display();


        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            listUser.Add(1);
            Display();


        }

        if (listUser.Count == 4)
        {
            CompareListUserAndListEnemy(listUser);
        }
        Debug.Log(listUser.Count);





    }
    public void recycleObject(int index)
    {

        if (objectPrefabs[index].activeSelf == false)
        {
            // set position ở vị trí nhất định
            objectPrefabs[index].transform.position = new Vector3(12f, -2, 0);
            Clear();
            // add Random listNumber
            listNumber = new List<int>();
            for (int i = 0; i < 4; i++)
            {
                listNumber.Add(Random.Range(0, 4));
            }
            Display();
        }

        Debug.Log(listNumber[0] + " " + listNumber[1] + " " + listNumber[2] + " " + listNumber[3]);

        // active object
        objectPrefabs[index].SetActive(true);


        //di chuyển sang trái
        objectPrefabs[index].transform.Translate(Vector3.left * 3 * Time.deltaTime);

        // ra khỏi màn hình thì active false
        if (objectPrefabs[index].transform.position.x < -15)
        {
            objectPrefabs[index].SetActive(false);
            i = Random.Range(0, objectPrefabs.Length);
        }
    }
    public void CompareListUserAndListEnemy(List<int> l)
    {
        if (l[0] == listNumber[0] && l[1] == listNumber[1] && l[2] == listNumber[2] && l[3] == listNumber[3])
        {
            Debug.Log("Win");
            listUser.Clear();
        }
        else
        {
            Debug.Log("Lose");
            listUser.Clear();
        }
    }
    public void Display()
    {
        if (listUser.Count == 0)
        {
            list1[listNumber[0]].SetActive(true);
            list2[listNumber[1]].SetActive(true);
            list3[listNumber[2]].SetActive(true);
            list4[listNumber[3]].SetActive(true);
        }
        // else if (listUser.Count == 1)
        // {
        //     list1[listNumber[0] + 4].SetActive(true);
        //     list2[listNumber[1]].SetActive(true);
        //     list3[listNumber[2]].SetActive(true);
        //     list4[listNumber[3]].SetActive(true);
        // }
        // else if (listUser.Count == 2)
        // {
        //     list1[listNumber[0] + 4].SetActive(true);
        //     list2[listNumber[1] + 4].SetActive(true);
        //     list3[listNumber[2]].SetActive(true);
        //     list4[listNumber[3]].SetActive(true);
        // }
        // else if (listUser.Count == 3)
        // {
        //     list1[listNumber[0] + 4].SetActive(true);
        //     list2[listNumber[1] + 4].SetActive(true);
        //     list3[listNumber[2] + 4].SetActive(true);
        //     list4[listNumber[3]].SetActive(true);
        // }
        // else
        // {
        //     list1[listNumber[0] + 4].SetActive(true);
        //     list2[listNumber[1] + 4].SetActive(true);
        //     list3[listNumber[2] + 4].SetActive(true);
        //     list4[listNumber[3] + 4].SetActive(true);
        // }


        else if (listUser.Count == 1)
        {
            if (listUser[0] == listNumber[0])
            {
                listNumber[0] += 4;
                list1[listNumber[0]].SetActive(true);
            }
        }
        else if (listUser.Count == 2)
        {
            if (listUser[0] == listNumber[0])
            {
                listNumber[0] += 4;
                list1[listNumber[0]].SetActive(true);
            }
            if (listUser[1] == listNumber[1])
            {
                listNumber[1] += 4;
                list2[listNumber[1]].SetActive(true);
            }
        }
        else if (listUser.Count == 3)
        {
            if (listUser[0] == listNumber[0])
            {
                listNumber[0] += 4;
                list1[listNumber[0]].SetActive(true);
            }
            if (listUser[1] == listNumber[1])
            {
                listNumber[1] += 4;
                list2[listNumber[1]].SetActive(true);
            }
            if (listUser[2] == listNumber[2])
            {
                listNumber[2] += 4;
                list3[listNumber[2]].SetActive(true);
            }

        }
        else
        {
            if (listUser[0] == listNumber[0])
            {
                listNumber[0] += 4;
                list1[listNumber[0]].SetActive(true);
            }
            if (listUser[1] == listNumber[1])
            {
                listNumber[1] += 4;
                list2[listNumber[1]].SetActive(true);
            }
            if (listUser[2] == listNumber[2])
            {
                listNumber[2] += 4;
                list3[listNumber[2]].SetActive(true);
            }
            if (listUser[3] == listNumber[3])
            {
                listNumber[3] += 4;
                list4[listNumber[3]].SetActive(true);
            }
        }




    }

    public void Clear()
    {
        //set active false all list1, list2, list3, list4

        for (int i = 0; i < list1.Count; i++)
        {
            list1[i].SetActive(false);
        }
        for (int i = 0; i < list2.Count; i++)
        {
            list2[i].SetActive(false);
        }
        for (int i = 0; i < list3.Count; i++)
        {
            list3[i].SetActive(false);
        }
        for (int i = 0; i < list4.Count; i++)
        {
            list4[i].SetActive(false);
        }

    }

}
