using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class ItemBase : NetworkBehaviour
{
    public string itemName;
    public TextField description;
    public Sprite icon;
    public Player targetPlayer;

    public abstract void Effect();
    public bool IsTargetAreDeffendUp()
    {
        return targetPlayer.isPlayerDeffend.Value;
    }
    public void BreakTargetPlayerDeffend()
    {
        targetPlayer.isPlayerDeffend.Value = false;
    }
}
