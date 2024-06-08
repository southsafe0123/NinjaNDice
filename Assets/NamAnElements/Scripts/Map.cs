using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public List<Transform> movePos = new List<Transform>();

    [ContextMenu("update move position to list")]
    public void updateMovePos()
    {
        movePos.Clear();
        foreach (Transform child in transform)
        {
            movePos.Add(child);
        }
    }
}
