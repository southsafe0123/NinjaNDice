using System;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    public NetworkVariable<bool> isPlayerTurn = new NetworkVariable<bool>();
    private void Start()
    {
        isPlayerTurn.OnValueChanged += OnValueChange;
    }
    public void SetPlayerTurn(bool isPlayerTurn)
    {
        this.isPlayerTurn.Value = isPlayerTurn;
    }

    void OnValueChange(bool oldBool, bool newBool)
    {
        if (oldBool != newBool) NetworkManagerUI.Singleton.btnRollDice.enabled = newBool;
    }
}
