using System;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    //this is client ID
    public NetworkVariable<ulong> ownerClientID = new NetworkVariable<ulong>();

    public NetworkVariable<bool> isPlayerTurn = new NetworkVariable<bool>();
    public NetworkVariable<int> currentPos = new NetworkVariable<int>();
    private void Start()
    {
        transform.position = new Vector3(100, 100, 0);
        DontDestroyOnLoad(gameObject);
    }
}
