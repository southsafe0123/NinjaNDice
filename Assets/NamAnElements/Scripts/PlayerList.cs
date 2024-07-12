using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class PlayerList : NetworkBehaviour
{
    public static PlayerList Instance;
    public List<Player> playerList = new List<Player>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UpdatePlayerList()
    {
        playerList.Clear();
        playerList = GameObject.FindObjectsByType<Player>(sortMode: FindObjectsSortMode.None).ToList();
        foreach (Player player in playerList)
        {
            SortPlayerListByServer_ClientRPC(player.ownerClientID.Value);
        }
    }
    [ClientRpc]
    public void SortPlayerListByServer_ClientRPC(ulong clientID)
    {
        if (IsHost) return;
        playerList.Clear();
        var temp = GameObject.FindObjectsByType<Player>(sortMode: FindObjectsSortMode.None).ToList();
        foreach (var item in temp)
        {
            if (item.ownerClientID.Value == clientID)
            {
                playerList.Add(item);
            }
        }
    }
}
