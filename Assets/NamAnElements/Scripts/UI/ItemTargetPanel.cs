using JetBrains.Annotations;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemTargetPanel : MonoBehaviour
{
    public List<Player> players = new List<Player>();
    public int playerIndex;
    public CamToPlayer camToPlayer;
    public TextMeshProUGUI txtPlayerName;
    public CanvasGroup checkingPanel;
    private void OnEnable()
    {
        players.Clear();
        camToPlayer = GameObject.Find("CM vcam1").GetComponent<CamToPlayer>();
        foreach (Player player in PlayerList.Instance.playerDic.Values)
        {
            players.Add(player);
        }
        DisplayPlayerCamera();
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
        txtPlayerName.text = players[playerIndex].GetComponent<PlayerData>().playerName.Value.ToString();
    }
    public void OnClickExitTarget()
    {
        camToPlayer.playerToFollow = camToPlayer.playerInTurn;
    }
    private void OnDisable()
    {
        OnClickExitTarget();
    }
    public void SetAlphaView(int alpha)
    {
        checkingPanel.alpha = alpha;
    }
    public void OnClickUseOnPlayer()
    {
        ItemPool.Instance.UseItem(players[playerIndex]);
    }
}
