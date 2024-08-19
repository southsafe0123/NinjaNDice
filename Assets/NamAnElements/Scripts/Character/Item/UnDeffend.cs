using DG.Tweening;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class UnDeffend : ItemBase
{
    public override void Effect()
    {
        if (IsTargetAreDeffendUp()) { BreakTargetPlayerDeffend(); return; }
        // Gửi ID người chơi bị hủy khiên
        SendUnDeffendPlayer_ServerRPC(targetPlayer.ownerClientID.Value);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SendUnDeffendPlayer_ServerRPC(ulong targetID)
    {
        // Kích hoạt coroutine trên client của người có khiên
        StartCoroutine(DeffendPlayerCoroutine(targetID));
    }

    private IEnumerator DeffendPlayerCoroutine(ulong targetID)
    {
        // Lấy đối tượng người chơi từ ID local
        var targetPlayer = PlayerList.Instance.GetPlayerDic_Value(targetID);
        //kiểm tra xem người chơi đó có isPlayerDeffend = true không, nếu có thì set về false
        if (targetPlayer.isPlayerDeffend.Value)
        {
            targetPlayer.isPlayerDeffend.Value = false;
        }




        yield return new WaitForSeconds(1f);  // Đợi 2 giây (hoặc thời gian mong muốn)


        // Cập nhật vòng lặp gameTurn và chuyển camera khi hết vòng chơi
        //GameManager.Singleton.NextPlayerTurn_ServerRPC();
    }
}
