using DG.Tweening;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class UnDeffend : ItemBase
{
    public GameObject prefabEffect;
    public override void Effect()
    {
        // Gửi ID người chơi bị hủy khiên
        SendUnDeffendPlayer_ServerRPC(targetPlayer.ownerClientID.Value);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SendUnDeffendPlayer_ServerRPC(ulong targetID)
    {
        SpawnAttackEffect_ClientRPC(targetID);
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
            AudioManager.Instance.PlaySFXBreakDownDef();
        }
        else
        {
            AudioManager.Instance.PlaySFXCutForNothing();
        }

        yield return new WaitForSeconds(1.5f);  // Đợi 2 giây (hoặc thời gian mong muốn

        // Cập nhật vòng lặp gameTurn và chuyển camera khi hết vòng chơi
        GameManager.Singleton.NextPlayerTurn_ServerRPC();
    }

    [ClientRpc]
    private void SpawnAttackEffect_ClientRPC(ulong targetPlayerID)
    {
        Player targetPlayer = PlayerList.Instance.GetPlayerDic_Value(targetPlayerID);
        Instantiate(prefabEffect, targetPlayer.transform.position, Quaternion.identity);
        targetPlayer.GetComponent<SpriteRenderer>().sprite = targetPlayer.GetComponent<PlayerData>().gameplaySprite;
        AudioManager.Instance.PlaySFXItemBreakDef();
    }
}
