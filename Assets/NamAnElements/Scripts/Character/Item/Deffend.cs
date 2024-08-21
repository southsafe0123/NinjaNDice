using DG.Tweening;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Deffend : ItemBase
{
    public override void Effect()
    {
        if (IsTargetAreDeffendUp()) { BreakTargetPlayerDeffend();}
        // Gửi ID người chơi có khiên
        SendDeffendPlayer_ServerRPC(targetPlayer.ownerClientID.Value);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SendDeffendPlayer_ServerRPC(ulong targetID)
    {
        // Kích hoạt coroutine trên client của người có khiên
        StartCoroutine(DeffendPlayerCoroutine(targetID));
    }


    private IEnumerator DeffendPlayerCoroutine(ulong targetID)
    {
        // Lấy đối tượng người chơi từ ID local
        var targetPlayer = PlayerList.Instance.GetPlayerDic_Value(targetID);
        targetPlayer.isPlayerDeffend.Value = true;  // Thêm khiên người chơi


        yield return new WaitForSeconds(1f);  // Đợi 2 giây (hoặc thời gian mong muốn)


        // Cập nhật vòng lặp gameTurn và chuyển camera khi hết vòng chơi
        GameManager.Singleton.NextPlayerTurn_ServerRPC();
    }
}
