using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodSpawn : MonoBehaviour
{
    public GameObject prefWood;

    private void Start()
    {
        StartCoroutine(SpawnWoodCoroutine());
    }

    private IEnumerator SpawnWoodCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(2f);
            Instantiate(prefWood,transform.position,Quaternion.identity);
        }
    }
}
