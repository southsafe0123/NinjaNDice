using System;
using System.Collections;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using WebSocketSharp;

public class PlayerData : NetworkBehaviour
{
    public NetworkVariable<FixedString128Bytes> playerName = new NetworkVariable<FixedString128Bytes>();
    public NetworkVariable<FixedString128Bytes> playerSkin = new NetworkVariable<FixedString128Bytes>();
    public Sprite gameplaySprite;
    public GameObject playerNamePanel;
    public TextMeshProUGUI txtPlayerName;
    public Player myPlayer;

    private void Start()
    {
        string myPlayerName = UserSessionManager.Instance.username.IsNullOrEmpty() ? PrefsData.GetData(PrefsData.PLAYER_INGAME_NAME_NOLOGIN) : ApiHandle.Instance.user.nameingame.ToString();
        string myPlayerSkin = UserSessionManager.Instance._id.IsNullOrEmpty()? 0.ToString(): ApiHandle.Instance.user.avatar;
        
        SetPlayerPlayerData_ServerRPC(myPlayerName, myPlayerSkin,NetworkManager.LocalClientId);
        
    }
    [ServerRpc(RequireOwnership =false)]
    private void SetPlayerPlayerData_ServerRPC(string clientPlayerName,string clientPlayerSkin, ulong clientID)
    {
        Debug.Log(clientPlayerName + clientPlayerSkin);
        SetPlayerSkin_ClientRPC(clientID,clientPlayerSkin,clientPlayerName);
    }
    [ClientRpc]
    private void SetPlayerSkin_ClientRPC(ulong clientID, string clientPlayerSkin, string clientPlayerName)
    {
        StartCoroutine(WaitPlayerListLoaded(clientID, clientPlayerSkin,clientPlayerName));
       
    }

    private IEnumerator WaitPlayerListLoaded(ulong clientID, string clientPlayerSkin,string clientPlayerName)
    {
        yield return new WaitUntil(() => PlayerList.Instance.GetPlayerDic_Value(clientID) != null);
        PlayerList.Instance.GetPlayerDic_Value(clientID).GetComponent<PlayerData>().playerName.Value = clientPlayerName;
        PlayerList.Instance.GetPlayerDic_Value(clientID).GetComponent<PlayerData>().playerSkin.Value = clientPlayerSkin;
        PlayerList.Instance.GetPlayerDic_Value(clientID).GetComponent<PlayerData>().gameplaySprite = SkinPool.instance.GetSkin(int.Parse(clientPlayerSkin)).skinData;
        PlayerList.Instance.GetPlayerDic_Value(clientID).GetComponent<SpriteRenderer>().sprite = PlayerList.Instance.GetPlayerDic_Value(clientID).GetComponent<PlayerData>().gameplaySprite;
    }

    private void Update()
    {
        if (txtPlayerName.text != playerName.Value.ToString())
        {
            txtPlayerName.text = playerName.Value.ToString();
        }
        if (myPlayer.isPlayerTurn.Value)
        {
            playerNamePanel.SetActive(true);
        }
        else
        {
            playerNamePanel.SetActive(false);
        }
    }
}
