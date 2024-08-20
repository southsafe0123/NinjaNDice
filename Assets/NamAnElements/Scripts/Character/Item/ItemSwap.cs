using DG.Tweening;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
public class ItemSwap : ItemBase
{
    public override void Effect()
    {
        if (IsTargetAreDeffendUp()) { BreakTargetPlayerDeffend(); return; }
        // Gửi ID người chơi bị đổi vị trí và người gửi lên server
        SendSwapPlayer_ServerRPC(targetPlayer.ownerClientID.Value, NetworkManager.Singleton.LocalClientId);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SendSwapPlayer_ServerRPC(ulong targetID, ulong senderID)
    {
        // Kích hoạt coroutine trên client của người chơi bị đổi vị trí
        StartCoroutine(SwapPlayerCoroutine(targetID,senderID));
    }

    private IEnumerator SwapPlayerCoroutine(ulong targetID,ulong senderID)
    {
        // Lấy đối tượng người chơi từ ID local và ID người gửi
        var targetPlayer = PlayerList.Instance.GetPlayerDic_Value(targetID);
        var senderPlayer = PlayerList.Instance.GetPlayerDic_Value(senderID);
        var targetTemp = targetPlayer.transform.position;

        targetPlayer.transform.position = senderPlayer.transform.position;
        senderPlayer.transform.position = targetTemp;

        yield return new WaitForSeconds(1f);  // Đợi 1 giây (hoặc thời gian mong muốn)

        // Cập nhật vòng lặp gameTurn và chuyển camera khi hết vòng chơi
        GameManager.Singleton.NextPlayerTurn_ServerRPC();
    }


}
