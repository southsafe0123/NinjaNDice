using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    public string itemName;
    public virtual void itemEffect()
    {
        Debug.Log("Tương tác với item" + itemName);
    }
}


