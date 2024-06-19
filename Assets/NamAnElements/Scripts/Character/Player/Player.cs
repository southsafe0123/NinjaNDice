using System;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    public NetworkVariable<bool> isPlayerTurn = new NetworkVariable<bool>();
    public NetworkVariable<int> currentPos = new NetworkVariable<int>();
    public GameObject answerGameObject;
    public NetworkVariable<int> life = new NetworkVariable<int>();
    public NetworkVariable<bool> isDie = new NetworkVariable<bool>();
    private void Start()
    {
        isPlayerTurn.OnValueChanged += OnValueChange;
        life.Value = 3;
    }
    public void SetPlayerTurn(bool isPlayerTurn)
    {
        this.isPlayerTurn.Value = isPlayerTurn;
    }

    void OnValueChange(bool oldBool, bool newBool)
    {
        if (oldBool != newBool) NetworkManagerUI.Singleton.btnRollDice.enabled = newBool;
    }

    void Update()
    {
        transform.Translate(Input.GetAxis("Horizontal") * Time.deltaTime * 5, Input.GetAxis("Vertical") * Time.deltaTime * 5, 0);


    }


    void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log(col.name);
        answerGameObject = col.gameObject;
    }

    public void WrongAnswer()
    {
        gameObject.transform.position = new Vector3(-8f, -1.6f, 0);
        // life--;
        // if (life == 0)
        // {
        //     Debug.Log("Game Over");
        //     isDie.Value = true;
        //     gameObject.SetActive(false);

        // }
    }
}
