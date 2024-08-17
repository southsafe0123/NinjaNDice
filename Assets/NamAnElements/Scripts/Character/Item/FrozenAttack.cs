using DG.Tweening;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class FrozenAttack : ItemBase
{
    public override void Effect()
    {
        // Gửi ID người chơi bị đóng băng lên server
        SendFrozenPlayer_ServerRPC(targetPlayer.ownerClientID.Value);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SendFrozenPlayer_ServerRPC(ulong targetID)
    {
        // Kích hoạt coroutine trên client của người chơi bị đóng băng
        StartFrozenPlayerCoroutine_ClientRPC(targetID);
    }

    [ClientRpc]
    private void StartFrozenPlayerCoroutine_ClientRPC(ulong targetID)
    {
        // Nếu ID người chơi khớp với ID được gửi từ server, bắt đầu coroutine đóng băng
        if (NetworkManager.Singleton.LocalClientId == targetID)
        {
            StartCoroutine(FrozenPlayerCoroutine());
        }
    }

    private IEnumerator FrozenPlayerCoroutine()
    {
        // Lấy đối tượng người chơi từ ID local
        var targetPlayer = PlayerList.Instance.GetPlayerDic_Value(NetworkManager.Singleton.LocalClientId);
        targetPlayer.isPlayerFrozen.Value = true;  // Đông cứng người chơi
        // targetPlayer.gameObject.transform.DOScale(new Vector2(0.8f, 0.8f), 0.2f).SetEase(Ease.OutBack);

        yield return new WaitForSeconds(2f);  // Đợi 2 giây (hoặc thời gian mong muốn)



        // Cập nhật vòng lặp gameTurn và chuyển camera khi hết vòng chơi
        var oldGameTurn = GameManager.Singleton.gameTurn;
        GameManager.Singleton.gameTurn = GameManager.Singleton.playerIndex == 0 ? GameManager.Singleton.gameTurn + 1 : GameManager.Singleton.gameTurn;
        GameManager.Singleton.OnGameTurnChange(oldGameTurn, GameManager.Singleton.gameTurn);
        GameManager.Singleton.SwitchCam();
    }
}
