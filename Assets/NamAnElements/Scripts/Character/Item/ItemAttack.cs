using Unity.Netcode;
using UnityEngine;

public class ItemAttack : ItemBase
{
    public Player targetPlayer;

    public override void Effect()
    {
        DoEffect_ServerRPC();

    }
    [ServerRpc(RequireOwnership = false)]
    public void DoEffect_ServerRPC()
    {
        var pushBackValue = Random.Range(1, 7);
        DoEffect_ClientRPC(pushBackValue);
    }
    [ClientRpc]
    public void DoEffect_ClientRPC(int pushBackValue)
    {
        if (IsHost)
        {
            GameManager.Singleton.TeleportPlayer(targetPlayer, -pushBackValue);
        }
    }
}