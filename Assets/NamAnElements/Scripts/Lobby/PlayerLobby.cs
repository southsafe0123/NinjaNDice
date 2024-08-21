using UnityEngine;
using UnityEngine.UI;

public class PlayerLobby : MonoBehaviour
{
    public Player player;
    public Image image;

    private void OnEnable()
    {
        if (player == null) return;
        image.sprite = SkinPool.instance.GetSkin(int.Parse(player.GetComponent<PlayerData>().playerSkin.Value.ToString())).skinAvatar;
    }
}