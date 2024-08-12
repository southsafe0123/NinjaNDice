using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;

public class ItemTargetPanel : MonoBehaviour
{
    public List<Player> players = new List<Player>();
    public int playerIndex;
    public CamToPlayer camToPlayer;
    private void OnEnable()
    {
        players.Clear();
        camToPlayer = GameObject.Find("CM vcam1").GetComponent<CamToPlayer>();
        foreach (Player player in PlayerList.Instance.playerDic.Values)
        {
            players.Add(player);
        }
    }
    public void OnClickNextPlayer()
    {
        playerIndex++;
        if (playerIndex > players.Count - 1) playerIndex = 0;
        DisplayPlayerCamera();
    }
    public void OnClickPrevPlayer()
    {
        playerIndex--;
        if (playerIndex < 0) playerIndex = players.Count - 1;
        DisplayPlayerCamera();
    }
    public void DisplayPlayerCamera()
    {
        camToPlayer.playerToFollow = players[playerIndex];
    }
    public void OnClickExitTarget()
    {
        camToPlayer.playerToFollow = camToPlayer.playerInTurn;
    }
    private void OnDisable()
    {
        OnClickExitTarget();
    }
    public void OnClickUseOnPlayer()
    {
        ItemPool.Instance.UseItem(players[playerIndex]);
    }
}
