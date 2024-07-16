using System;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    //this is client ID
    public NetworkVariable<ulong> ownerClientID = new NetworkVariable<ulong>();

    public NetworkVariable<bool> isPlayerTurn = new NetworkVariable<bool>();
    public NetworkVariable<int> currentPos = new NetworkVariable<int>();
    public string answer;
    public int life = 3;
    public bool isDie = false;
    private void Start()
    {


        transform.position = new Vector3(100, 100, 0);
        // DontDestroyOnLoad(gameObject);
    }
    public void SetPlayerTurn(bool isPlayerTurn)
    {
        this.isPlayerTurn.Value = isPlayerTurn;
    }
    void Update()
    {
        if (IsLocalPlayer)
        {
            transform.Translate(Input.GetAxis("Horizontal") * Time.deltaTime * 5, Input.GetAxis("Vertical") * Time.deltaTime * 5, 0);

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                answer = "1";
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                answer = "2";
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                answer = "3";
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                answer = "4";
            }
        }



    }


    void OnTriggerEnter2D(Collider2D col)
    {
        // // Debug.Log(col.name);
        // if (col.gameObject.tag == "Answer")
        // {
        //     answerGameObject = col.gameObject;
        // }
    }

    public void WrongAnswer()
    {
        gameObject.transform.position = new Vector3(-8f, -1.6f, 0);

        // if (IsServer)
        // {
        life--;
        if (life <= 0)
        {
            Debug.Log("Game Over");
            isDie = true;
            if (IsServer)
            {
                NetworkObject.NetworkHide(NetworkObjectId);
            }
            gameObject.SetActive(false);
        }
        // }
    }
}
