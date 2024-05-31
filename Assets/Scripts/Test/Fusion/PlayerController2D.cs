using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System.Linq;

public class PlayerController2D : NetworkBehaviour
{
    private Rigidbody2D _rigidbody;

    [Networked] public int turn { get; set; }
    [Networked] public int currentTurn { get; set; }


    public GameObject[] players;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        RandomTurn();
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput<NetworkInputData>(out var inputData))
        {
            Vector2 moveDirection = new Vector2(inputData.direction.x, inputData.direction.y);
            _rigidbody.velocity = moveDirection * 5f; // Tốc độ di chuyển
            if (turn == currentTurn)
            {
                if (inputData.roll)
                {
                    Vector2 rollDirection = new Vector2(2.5f, 0);
                    _rigidbody.velocity = rollDirection * 10; // Tốc độ di chuyển
                    EndTurn();
                }
            }

        }


    }


    // ham rpc gui nhanh thong tin

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority, HostMode = RpcHostMode.SourceIsHostPlayer)]
    public void RPC_SendMessage(string message, RpcInfo info = default)
    {
        RPC_RelayMessage(message, info.Source);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All, HostMode = RpcHostMode.SourceIsServer)]
    public void RPC_RelayMessage(string message, PlayerRef messageSource)
    {

    }

    public void RandomTurn()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        int[] turns = new int[4];
        for (int i = 0; i < players.Length; i++)
        {
            turns[i] = players[i].GetComponent<PlayerController2D>().turn;
            players[i].GetComponent<PlayerController2D>().currentTurn = 1;
        }
        if (turns.Length == 0)
        {
            turn = Random.Range(1, 5);
        }
        else
        {
            int randomTurn = Random.Range(1, 5);
            while (turns.Contains(randomTurn))
            {
                randomTurn = Random.Range(1, 5);
            }
            turn = randomTurn;
        }




    }

    public void EndTurn()
    {
        currentTurn++;
        if (currentTurn > 4)
        {
            currentTurn = 1;
        }

        players = GameObject.FindGameObjectsWithTag("Player");

        for (int i = 0; i < players.Length; i++)
        {

            players[i].GetComponent<PlayerController2D>().currentTurn = currentTurn;
        }

    }

}
