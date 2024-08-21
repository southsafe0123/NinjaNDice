using System;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using WebSocketSharp;

public class PlayerData : NetworkBehaviour
{
    public NetworkVariable<FixedString32Bytes> playerName = new NetworkVariable<FixedString32Bytes>(writePerm: NetworkVariableWritePermission.Server);
    public NetworkVariable<FixedString32Bytes> playerSkin = new NetworkVariable<FixedString32Bytes>(writePerm: NetworkVariableWritePermission.Server);
    public GameObject playerNamePanel;
    public TextMeshProUGUI txtPlayerName;
    public Player myPlayer;

    private void Start()
    {
        string myPlayerName = UserSessionManager.Instance.username.IsNullOrEmpty() ? PrefsData.GetData(PrefsData.PLAYER_INGAME_NAME_NOLOGIN) : ApiHandle.Instance.user.nameingame.ToString();
        string myPlayerSkin = PrefsData.GetData(PrefsData.PLAYER_SKIN_ID);
        SetPlayerPlayerData_ServerRPC(myPlayerName, myPlayerSkin,NetworkManager.LocalClientId);
    }
    [ServerRpc(RequireOwnership =false)]
    private void SetPlayerPlayerData_ServerRPC(string clientPlayerName,string clientPlayerSkin, ulong clientID)
    {   if (!IsHost) return;
        PlayerList.Instance.GetPlayerDic_Value(clientID).GetComponent<PlayerData>().playerName.Value = clientPlayerName;
        PlayerList.Instance.GetPlayerDic_Value(clientID).GetComponent<PlayerData>().playerSkin.Value = clientPlayerSkin;
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
