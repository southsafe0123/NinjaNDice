using DG.Tweening;
using System;
using System.Collections;
using System.Linq.Expressions;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class ItemAttack : ItemBase
{
    public override void Effect()
    {
        Debug.Log("InEffect");
        int attackDamage = UnityEngine.Random.Range(0, 7);
        Debug.Log(targetPlayer.ownerClientID.Value);
        SendAttackDamage_ServerRPC(targetPlayer.ownerClientID.Value,attackDamage);
    }

    [ServerRpc(RequireOwnership =false)]
    public void SendAttackDamage_ServerRPC(ulong targetID, int attackDamage)
    {
        Debug.Log(targetID);

        Player targetPlayer = PlayerList.Instance.GetPlayerDic_Value(targetID);
        StartCoroutine(MoveBackCoroutine(targetPlayer, attackDamage));
    }


    private IEnumerator MoveBackCoroutine(Player targetPlayer, int attackDamage)
    {
        int posCount = -1;
        Debug.Log("inCoroutine item");
        do
        {
            Debug.Log("running");
            yield return null;
            int newPos = targetPlayer.currentPos.Value - 1;
            if (newPos <= GameManager.Singleton.map.movePos.Count)
            {
                newPos = GameManager.Singleton.map.movePos.Count;
            }
            targetPlayer.currentPos.Value = newPos;
            try
            {
                targetPlayer.gameObject.transform.DOJump(GameManager.Singleton.map.movePos[newPos].position, 0.5f, 1, 0.4f);
                posCount++;
            }
            catch
            {
                targetPlayer.gameObject.transform.position = GameManager.Singleton.map.movePos[GameManager.Singleton.map.movePos.Count - 1].position;
                break;
            };
            yield return new WaitUntil(() => targetPlayer.gameObject.transform.position == GameManager.Singleton.map.movePos[newPos].position);
            yield return new WaitForSeconds(0.15f);
        } while (posCount >= -attackDamage);

        GameManager.Singleton.playerIndex = GameManager.Singleton.playerIndex >= GameManager.Singleton.playerList.Count - 1 ? 0 : GameManager.Singleton.playerIndex + 1;
        var oldGameTurn = GameManager.Singleton.gameTurn;
        GameManager.Singleton.gameTurn = GameManager.Singleton.playerIndex == 0 ? GameManager.Singleton.gameTurn + 1 : GameManager.Singleton.gameTurn;
        GameManager.Singleton.OnGameTurnChange(oldGameTurn, GameManager.Singleton.gameTurn);
        GameManager.Singleton.SwitchCam();
    }
}