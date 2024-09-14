using JetBrains.Annotations;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemTargetPanel : MonoBehaviour
{
    public List<Player> players = new List<Player>();
    public int playerIndex;
    public CamToPlayer camToPlayer;
    public TextMeshProUGUI txtPlayerName;
    public CanvasGroup checkingPanel;
    public Image playerAvatar;
    public Button btnBack;
    public Button btnUse;
    private void Start()
    {
        btnBack.onClick.AddListener(() =>
        {
            OnClickExitTarget();
        });
        btnUse.onClick.AddListener(() =>
        {
            CamToPlayer.instance.playerToFollow = players[playerIndex];
        });
    }
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
        
        Player player = players[playerIndex];
        camToPlayer.playerToFollow = player;
        txtPlayerName.text = player.GetComponent<PlayerData>().playerName.Value.ToString();
        var playerAvatarSlot = player.GetComponent<PlayerData>().playerSkin.Value.ToString();
        playerAvatar.sprite = SkinPool.instance.GetSkin(int.Parse(playerAvatarSlot)).skinAvatar;
        player.GetComponent<SpriteRenderer>().sortingOrder = 2;
        foreach (Player playerr in players)
        {
            if(playerr != player)
            {
                playerr.GetComponent<SpriteRenderer>().sortingOrder = 1;
            }
        }
    }
    public void OnClickExitTarget()
    {
        Player playerInTurn = camToPlayer.playerInTurn;
        camToPlayer.playerToFollow = camToPlayer.playerInTurn;
        playerInTurn.GetComponent<SpriteRenderer>().sortingOrder = 2;
        foreach (Player playerr in players)
        {
            if (playerr != playerInTurn)
            {
                playerr.GetComponent<SpriteRenderer>().sortingOrder = 1;
            }
        }
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
