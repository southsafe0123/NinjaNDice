using DG.Tweening;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
public class ItemSwap : ItemBase
{
    public override void Effect()
    {
        // Gửi ID người chơi bị đổi vị trí và người gửi lên server
        SendSwapPlayer_ServerRPC(targetPlayer.ownerClientID.Value, NetworkManager.Singleton.LocalClientId);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SendSwapPlayer_ServerRPC(ulong targetID, ulong senderID)
    {
        // Kích hoạt coroutine trên client của người chơi bị đổi vị trí
        StartSwapPlayerCoroutine_ClientRPC(targetID, senderID);
    }

    [ClientRpc]
    private void StartSwapPlayerCoroutine_ClientRPC(ulong targetID, ulong senderID)
    {
        // Nếu ID người chơi khớp với ID được gửi từ server, bắt đầu coroutine đổi vị trí
        if (NetworkManager.Singleton.LocalClientId == targetID)
        {
            StartCoroutine(SwapPlayerCoroutine(senderID));
        }
    }

    private IEnumerator SwapPlayerCoroutine(ulong senderID)
    {
        // Lấy đối tượng người chơi từ ID local và ID người gửi
        var targetPlayer = PlayerList.Instance.GetPlayerDic_Value(NetworkManager.Singleton.LocalClientId);
        var senderPlayer = PlayerList.Instance.GetPlayerDic_Value(senderID);


        yield return new WaitForSeconds(1f);  // Đợi 1 giây (hoặc thời gian mong muốn)

        // Cập nhật vòng lặp gameTurn và chuyển camera khi hết vòng chơi
        var oldGameTurn = GameManager.Singleton.gameTurn;
        GameManager.Singleton.gameTurn = GameManager.Singleton.playerIndex == 0 ? GameManager.Singleton.gameTurn + 1 : GameManager.Singleton.gameTurn;
        GameManager.Singleton.OnGameTurnChange(oldGameTurn, GameManager.Singleton.gameTurn);
        GameManager.Singleton.SwitchCam();
    }


}
