using DG.Tweening;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class UnDeffend : ItemBase
{
    public override void Effect()
    {
        // Gửi ID người chơi bị hủy khiên
        SendUnDeffendPlayer_ServerRPC(targetPlayer.ownerClientID.Value);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SendUnDeffendPlayer_ServerRPC(ulong targetID)
    {
        // Kích hoạt coroutine trên client của người có khiên
        StartUnDeffendPlayerCoroutine_ClientRPC(targetID);
    }

    [ClientRpc]
    private void StartUnDeffendPlayerCoroutine_ClientRPC(ulong targetID)
    {
        // Nếu ID người chơi khớp với ID được gửi từ server, bắt đầu coroutine khiên
        if (NetworkManager.Singleton.LocalClientId == targetID)
        {
            StartCoroutine(DeffendPlayerCoroutine());
        }
    }

    private IEnumerator DeffendPlayerCoroutine()
    {
        // Lấy đối tượng người chơi từ ID local
        var targetPlayer = PlayerList.Instance.GetPlayerDic_Value(NetworkManager.Singleton.LocalClientId);
        //kiểm tra xem người chơi đó có isPlayerDeffend = true không, nếu có thì set về false
        if (targetPlayer.isPlayerDeffend.Value)
        {
            targetPlayer.isPlayerDeffend.Value = false;
        }




        yield return new WaitForSeconds(2f);  // Đợi 2 giây (hoặc thời gian mong muốn)


        // Cập nhật vòng lặp gameTurn và chuyển camera khi hết vòng chơi
        var oldGameTurn = GameManager.Singleton.gameTurn;
        GameManager.Singleton.gameTurn = GameManager.Singleton.playerIndex == 0 ? GameManager.Singleton.gameTurn + 1 : GameManager.Singleton.gameTurn;
        GameManager.Singleton.OnGameTurnChange(oldGameTurn, GameManager.Singleton.gameTurn);
        GameManager.Singleton.SwitchCam();
    }
}
