using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

public class PlayerLobby : MonoBehaviour
{
    public Player player;
    public Image image;
    public TextMeshProUGUI txtName;
    private void OnEnable()
    {
        
        if (player != null)
        {
            txtName.text = player.GetComponent<PlayerData>().playerName.Value.ToString();
            image.sprite = SkinPool.instance.GetSkin(int.Parse(player.GetComponent<PlayerData>().playerSkin.Value.ToString())).skinAvatar;
        }
        else
        {
            if (UserSessionManager.Instance._id.IsNullOrEmpty())
            {
                image.sprite = SkinPool.instance.GetSkin(0).skinAvatar;
                txtName.text = PrefsData.GetData(PrefsData.PLAYER_INGAME_NAME_NOLOGIN);
            }
            else
            {
                image.sprite = SkinPool.instance.GetSkin(int.Parse(ApiHandle.Instance.user.avatar)).skinAvatar;
                txtName.text = ApiHandle.Instance.user.nameingame;
            }
            
        };
    }
}