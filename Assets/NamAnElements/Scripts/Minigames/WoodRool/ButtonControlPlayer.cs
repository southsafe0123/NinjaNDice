using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class ButtonControlPlayer: NetworkBehaviour
{
    public static ButtonControlPlayer Instance;
    public Player player;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        Player[] playerTemp = GameObject.FindObjectsByType<Player>(0);
        for (int i = 0; i < playerTemp.Length; i++)
        {
            if (playerTemp[i].ownerClientID.Value == NetworkManager.Singleton.LocalClientId)
            {
                player = playerTemp[i];
            }
        }
    }

    public void OnClickPlayerInvisible()
    {
        StartCoroutine(PlayerInvCoroutine());
    }

    private IEnumerator PlayerInvCoroutine()
    {
        player.GetComponent<BoxCollider2D>().enabled = false;
        PlayerInv_ServerRPC(player.ownerClientID.Value, false);
        yield return new WaitForSeconds(1f);
        player.GetComponent<BoxCollider2D>().enabled = true;
        PlayerInv_ServerRPC(player.ownerClientID.Value, true);
    }
    [ServerRpc(RequireOwnership =false)]
    public void PlayerInv_ServerRPC(ulong playerID,bool isEnable)
    {
        PlayerInv_ClientRPC(playerID, isEnable);
    }
    [ClientRpc]
    private void PlayerInv_ClientRPC(ulong playerID, bool isEnable)
    {
        if (playerID == player.ownerClientID.Value) return;
        var playerLinQ = WoodRollManager.Instance.playerList.Where(player => player.ownerClientID.Value == playerID).ToList();
        playerLinQ[0].GetComponent<BoxCollider2D>().enabled = isEnable;
    }
}
