using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wood : MonoBehaviour
{
    public float moveSpeed = 10;

    private void Update()
    {
        transform.position -= new Vector3(moveSpeed, 0,0) * Time.deltaTime;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("WoodDespawn"))
        {
            Destroy(gameObject);
        }
        if (collision.CompareTag("Player"))
        {
            collision.transform.position -= Vector3.one;
        }
    }
}
