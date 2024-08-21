using DG.Tweening;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class FrozenAttack : ItemBase
{
    public GameObject prefabEffect;
    public override void Effect()
    {
        if (IsTargetAreDeffendUp()) { BreakTargetPlayerDeffend(targetPlayer); return; }
        // Gửi ID người chơi bị đóng băng lên server
        SendFrozenPlayer_ServerRPC(targetPlayer.ownerClientID.Value);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SendFrozenPlayer_ServerRPC(ulong targetID)
    {
        // Kích hoạt coroutine trên client của người chơi bị đóng băng
        SpawnAttackEffect_ClientRPC(targetID);
        StartCoroutine(FrozenPlayerCoroutine(targetID));
    }

    private IEnumerator FrozenPlayerCoroutine(ulong targetID)
    {
        // Lấy đối tượng người chơi từ ID local
        var targetPlayer = PlayerList.Instance.GetPlayerDic_Value(targetID);
        targetPlayer.isPlayerFrozen.Value = true;  // Đông cứng người chơi
        // targetPlayer.gameObject.transform.DOScale(new Vector2(0.8f, 0.8f), 0.2f).SetEase(Ease.OutBack);

        yield return new WaitForSeconds(1f);  // Đợi 2 giây (hoặc thời gian mong muốn)

        // Cập nhật vòng lặp gameTurn và chuyển camera khi hết vòng chơi
        GameManager.Singleton.NextPlayerTurn_ServerRPC();
    }
    [ClientRpc]
    private void SpawnAttackEffect_ClientRPC(ulong targetPlayerID)
    {
        Player targetPlayer = PlayerList.Instance.GetPlayerDic_Value(targetPlayerID);
        Instantiate(prefabEffect, targetPlayer.transform.position, Quaternion.identity);
    }
}
