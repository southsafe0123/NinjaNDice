using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ButtonRollDice : NetworkBehaviour
{
    public Player player;
    public Button btnRollDice;
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

    private void Update()
    {
        if (player.isPlayerTurn.Value)
        {
            btnRollDice.gameObject.SetActive(true);
        }
        else
        {
            btnRollDice.gameObject.SetActive(false);
        }
    }
    public void OnClick()
    {
        GameManager.Singleton.SendRollDiceTo_ServerRPC();
    }
}
