using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class ItemBase : NetworkBehaviour
{
    public int id;
    public string itemName;
    public TextField description;
    public Sprite icon;

    public abstract void Effect();
}
