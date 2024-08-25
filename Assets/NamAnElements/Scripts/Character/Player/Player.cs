using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : NetworkBehaviour
{
    //this is client ID
    public NetworkVariable<ulong> ownerClientID = new NetworkVariable<ulong>();
    public NetworkVariable<bool> isReadySceneLoaded = new NetworkVariable<bool>();
    public NetworkVariable<bool> isPlayerTurn = new NetworkVariable<bool>();
    public NetworkVariable<int> currentPos = new NetworkVariable<int>();
    public NetworkVariable<bool> isPlayerDoneGame = new NetworkVariable<bool>();
    public NetworkVariable<bool> isPlayerFrozen = new NetworkVariable<bool>();
    public NetworkVariable<bool> isPlayerDeffend = new NetworkVariable<bool>();

    public GameObject uiPopupCurrentHeath;
    public TextMeshProUGUI txtCurrentHeath;

    public string answer;
    public int life = 3;
    public bool isDie = false;
    private void Start()
    {
        transform.position = new Vector3(100, 100, 0);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetPlayerReadySceneLoaded_ServerRPC(ulong clientID, bool isReadySceneLoaded)
    {
        PlayerList.Instance.GetPlayerDic_Value(clientID).isReadySceneLoaded.Value = isReadySceneLoaded;
    }
    public void SetPlayerTurn(bool isPlayerTurn)
    {
        this.isPlayerTurn.Value = isPlayerTurn;
    }

    public void DisplayCurrentHealth()
    {
        var playerHeath = GetComponent<PlayerHeath>();
        uiPopupCurrentHeath.SetActive(true);
        if (playerHeath != null)
        {
            txtCurrentHeath.text = "X" + GetComponent<PlayerHeath>().health;
            if (playerHeath.isDead)
            {
                GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f, 1);
            }
            else
            {
                GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1);
            }
        }


    }
}
