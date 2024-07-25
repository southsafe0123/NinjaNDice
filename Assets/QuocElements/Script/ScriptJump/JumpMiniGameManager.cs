using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.VisualScripting;


public class Score : MonoBehaviour
{
    public int score = 0;
}
public class JumpMiniGameManager : NetworkBehaviour
{
    public List<Player> playerList = new List<Player>();
    public const string MAIN_GAMEPLAY_SCENE = "NamAn";
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    // private void LoadPlayer()
    // {
    //     if (!IsHost) return;
    //     playerList = PlayerList.Instance.GetPlayerOrder();
    //     for (int i = 0; i < playerList.Count; i++)
    //     {
    //         // playerList[i].gameObject.transform.position = map.movePos[i].position;
    //         // playerList[i].isPlayerTurn.Value = false;
    //         AddComponent_ClientRPC(playerList[i].ownerClientID.Value);
    //     }
    // }

    // [ClientRpc]
    // private void AddComponent_ClientRPC(ulong clientID)
    // {
    //     PlayerList.Instance.GetPlayerDic_Value(clientID).AddComponent<Score>();
    //     PlayerList.Instance.GetPlayerDic_Value(clientID).GetComponent<Score>().score = 0;
    // }


    // // hàm so sánh danh sách người dùng và danh sách số ngẫu nhiên
    // private void CompareListUserAndListEnemy()
    // {
    //     if (!listUserInput.SequenceEqual(listNumber) || listUserInput.Count != 4 || listUserInput == null)
    //     {
    //         Debug.Log("Lose");
    //         TakeDamage_ServerRPC(NetworkManager.Singleton.LocalClientId);
    //     }
    // }
    // [ServerRpc(RequireOwnership = false)]
    // public void TakeDamage_ServerRPC(ulong clientID)
    // {
    //     for (int i = 0; i < playerList.Count; i++)
    //     {
    //         if (playerList[i].ownerClientID.Value == clientID)
    //         {
    //             TakeDamage_ClientRPC(clientID);
    //         }
    //     }
    // }
    // [ClientRpc]
    // private void TakeDamage_ClientRPC(ulong playerID)
    // {
    //     var player = PlayerList.Instance.GetPlayerDic_Value(playerID);
    //     player.GetComponent<PlayerHeath>().health--;
    //     if (player.GetComponent<PlayerHeath>().health == 0)
    //     {
    //         player.GetComponent<PlayerHeath>().isDead = true;
    //         CallThisPlayerIsDead_ServerRPC(playerID);
    //     }
    // }
    // [ServerRpc(RequireOwnership = false)]
    // private void CallThisPlayerIsDead_ServerRPC(ulong playerID)
    // {
    //     var player = PlayerList.Instance.GetPlayerDic_Value(playerID);
    //     playerLose.Add(player);
    //     if (playerLose.Count >= playerList.Count - 1) EndGame();
    // }

    // private void EndGame()
    // {
    //     foreach (Player player in playerList)
    //     {
    //         if (!player.GetComponent<PlayerHeath>().isDead)
    //         {
    //             PlayerList.Instance.SetPlayerOrder(playerOrder, player);
    //             playerOrder++;
    //             CallEndGame_ClientRPC(player.ownerClientID.Value, true);
    //         }
    //     }
    //     foreach (Player player in playerLose)
    //     {
    //         CallEndGame_ClientRPC(player.ownerClientID.Value, false);
    //     }

    //     var reversePlayerList = playerLose.ToArray().Reverse();
    //     foreach (Player player in reversePlayerList)
    //     {
    //         PlayerList.Instance.SetPlayerOrder(playerOrder, player);
    //         playerOrder++;
    //     }

    //     RemovedComponent_ClientRPC();

    //     if (NetworkManager.Singleton.IsServer)
    //     {
    //         NetworkManager.Singleton.SceneManager.LoadScene(MAIN_GAMEPLAY_SCENE, LoadSceneMode.Single);
    //     }
    // }
    // [ClientRpc]
    // private void RemovedComponent_ClientRPC()
    // {
    //     foreach (var player in PlayerList.Instance.playerDic)
    //     {
    //         Destroy(player.Value.GetComponent<PlayerHeath>());
    //     }
    // }

    // [ClientRpc]
    // private void CallEndGame_ClientRPC(ulong playerID, bool isWin)
    // {
    //     if (isWin) Debug.LogError("Player Win: " + playerID);
    //     if (!isWin) Debug.LogError("Player lose: " + playerID);
    // }

}
