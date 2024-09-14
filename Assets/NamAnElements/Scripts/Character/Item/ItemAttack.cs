using DG.Tweening;
using System;
using System.Collections;
using System.Linq.Expressions;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class ItemAttack : ItemBase
{
    public GameObject prefabEffect;
    public override void Effect()
    {
        if (IsTargetAreDeffendUp()) { BreakTargetPlayerDeffend(targetPlayer); return; }
        int attackDamage = UnityEngine.Random.Range(1, 5);
        SendAttackDamage_ServerRPC(targetPlayer.ownerClientID.Value, attackDamage);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SendAttackDamage_ServerRPC(ulong targetID, int attackDamage)
    {
        Player targetPlayer = PlayerList.Instance.GetPlayerDic_Value(targetID);
        SpawnAttackEffect_ClientRPC(targetID);
        StartCoroutine(MoveBackCoroutine(targetPlayer, attackDamage));
    }


    private IEnumerator MoveBackCoroutine(Player targetPlayer, int attackDamage)
    {
        //kiểm tra targetPlayer có isPlayerDeffend = true không, nếu có thì return và trả isPlayerDeffend = false
        // if (targetPlayer.isPlayerDeffend.Value)
        // {
        //     targetPlayer.isPlayerDeffend.Value = false;
        //     yield break;
        // }
        
        int posCount = -1;
        WaitUntil waitUntil = new WaitUntil(() => targetPlayer.gameObject.transform.position == GameManager.Singleton.map.movePos[targetPlayer.currentPos.Value].position);
        WaitForSeconds waitForSeconds = new WaitForSeconds(0.15f);
        do
        {
            yield return null;
            int newPos = targetPlayer.currentPos.Value - 1;
            if (newPos <= 0)
            {
                newPos = 0;
            }
            targetPlayer.currentPos.Value = newPos;
            try
            {
                targetPlayer.gameObject.transform.DOJump(GameManager.Singleton.map.movePos[targetPlayer.currentPos.Value].position, 0.5f, 1, 0.4f);
                AudioManager.Instance.PlaySFXJump();
                posCount--;
            }
            catch
            {
                targetPlayer.gameObject.transform.position = GameManager.Singleton.map.movePos[0].position;
                break;
            };
            yield return waitUntil;
            yield return waitForSeconds;
        } while (posCount >= -attackDamage);

        GameManager.Singleton.NextPlayerTurn_ServerRPC();
    }
    [ClientRpc]
    private void SpawnAttackEffect_ClientRPC(ulong targetPlayerID)
    {
        Player targetPlayer = PlayerList.Instance.GetPlayerDic_Value(targetPlayerID);
        Instantiate(prefabEffect, targetPlayer.transform.position, Quaternion.identity);
        AudioManager.Instance.PlaySFXItemNormalPuch();
    }
}