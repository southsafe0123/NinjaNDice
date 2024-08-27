using System.Collections;
using System.Collections.Generic;
// using UnityEditor.Tilemaps;
using UnityEngine;

public class RockFall : MonoBehaviour
{
    public void DestroyObject()
    {
        Destroy(gameObject.transform.parent.gameObject);
    }
}
