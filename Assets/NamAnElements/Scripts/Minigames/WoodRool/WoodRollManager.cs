using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class WoodRollManager : NetworkBehaviour
{
    public static WoodRollManager Instance;
    public List<Player> playerList = new List<Player>();
    public Map map;
    private void Awake()
    {
        Instance=this;
    }
    void Start()
    {
        playerList = GameObject.FindObjectsByType<Player>(sortMode: FindObjectsSortMode.None).ToList();
        if (!IsHost) return;
        for (int i = 0; i < playerList.Count; i++)
        {
            playerList[i].gameObject.transform.position = map.movePos[i].position;
        }
    }
}
