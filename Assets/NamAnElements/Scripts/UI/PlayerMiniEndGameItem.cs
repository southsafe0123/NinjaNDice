using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMiniEndGameItem : MonoBehaviour
{
    public Player player;
    public string playerTop;
    public ItemBase itemPlayerGet;

    public TextMeshProUGUI txtPlayerName;
    public GameObject plus1;
    public Image itemGivePlayerImage;
    public TextMeshProUGUI txtPlayerTop;
    public Image playerImage;
    private void OnEnable()
    {
        txtPlayerTop.text = playerTop;
        txtPlayerName.text = player.GetComponent<PlayerData>().playerName.Value.ToString();
        playerImage.sprite = SkinPool.instance.GetSkin(int.Parse(player.GetComponent<PlayerData>().playerSkin.Value.ToString())).skinAvatar;
        if (itemPlayerGet != null)
        {
            plus1.SetActive(true);
            itemGivePlayerImage.gameObject.transform.parent.gameObject.SetActive(true);
            itemGivePlayerImage.sprite = itemPlayerGet.icon;
        }
        else
        {
            plus1.SetActive(false);
            itemGivePlayerImage.gameObject.transform.parent.gameObject.SetActive(false);
        }
    }
}