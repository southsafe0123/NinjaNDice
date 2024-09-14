using DG.Tweening;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
public class ItemSwap : ItemBase
{
    public GameObject prefabEffect;
    public override void Effect()
    {
        if (IsTargetAreDeffendUp()) { BreakTargetPlayerDeffend(targetPlayer); return; }
        // Gửi ID người chơi bị đổi vị trí và người gửi lên server
        SendSwapPlayer_ServerRPC(targetPlayer.ownerClientID.Value, NetworkManager.Singleton.LocalClientId);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SendSwapPlayer_ServerRPC(ulong targetID, ulong senderID)
    {
        // Kích hoạt coroutine trên client của người chơi bị đổi vị trí
        SpawnAttackEffect_ClientRPC(targetID, senderID);
        StartCoroutine(SwapPlayerCoroutine(targetID,senderID));
    }

    private IEnumerator SwapPlayerCoroutine(ulong targetID,ulong senderID)
    {
        // Lấy đối tượng người chơi từ ID local và ID người gửi
        var targetPlayer = PlayerList.Instance.GetPlayerDic_Value(targetID);
        var senderPlayer = PlayerList.Instance.GetPlayerDic_Value(senderID);
        var targetTemp = targetPlayer.transform.position;
        var targetValueTemp = targetPlayer.currentPos.Value;

        targetPlayer.currentPos.Value = senderPlayer.currentPos.Value;
        senderPlayer.currentPos.Value = targetValueTemp;

        targetPlayer.transform.position = senderPlayer.transform.position;
        senderPlayer.transform.position = targetTemp;

        yield return new WaitForSeconds(1f);  // Đợi 1 giây (hoặc thời gian mong muốn)

        // Cập nhật vòng lặp gameTurn và chuyển camera khi hết vòng chơi
        GameManager.Singleton.NextPlayerTurn_ServerRPC();
    }

    [ClientRpc]
    private void SpawnAttackEffect_ClientRPC(ulong targetPlayerID,ulong senderID)
    {
        Player targetPlayer = PlayerList.Instance.GetPlayerDic_Value(targetPlayerID);
        Player senderPlayer = PlayerList.Instance.GetPlayerDic_Value(senderID);
        Instantiate(prefabEffect, targetPlayer.transform.position, Quaternion.identity);
        Instantiate(prefabEffect, senderPlayer.transform.position, Quaternion.identity);
        AudioManager.Instance.PlaySFXItemSwap();
    }
}
