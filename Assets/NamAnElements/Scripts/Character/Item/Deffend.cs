using DG.Tweening;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Deffend : ItemBase
{
    public Sprite deffendSprite;
    public override void Effect()
    {
        // Gửi ID người chơi có khiên
        SendDeffendPlayer_ServerRPC(targetPlayer.ownerClientID.Value);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SendDeffendPlayer_ServerRPC(ulong targetID)
    {
        SpawnAttackEffect_ClientRPC(targetID);
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
    [ClientRpc]
    private void SpawnAttackEffect_ClientRPC(ulong targetPlayerID)
    {
        Player targetPlayer = PlayerList.Instance.GetPlayerDic_Value(targetPlayerID);
        targetPlayer.GetComponent<SpriteRenderer>().sprite = deffendSprite;
    }
}
