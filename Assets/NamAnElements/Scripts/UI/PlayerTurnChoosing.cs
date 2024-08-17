using Unity.Netcode;
using UnityEngine.UI;

public class PlayerTurnChoosing : NetworkBehaviour
{
    public Player player;
    public Button btnRollDice;
    public Button btnItem;
    private void Start()
    {
        player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Player>();
        btnRollDice.onClick.AddListener(() => OnClickRollDice());
    }

    private void Update()
    {
        if (player.isPlayerTurn.Value)
        {
            btnRollDice.gameObject.SetActive(true);
            btnItem.gameObject.SetActive(true);
        }
        else
        {
            btnRollDice.gameObject.SetActive(false);
            btnItem.gameObject.SetActive(false);
        }
    }
    public void OnClickRollDice()
    {
        GameManager.Singleton.SendRollDiceTo_ServerRPC(player.ownerClientID.Value);
    }
}