using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{


    void Update()
    {
        //hủy khi ra khỏi màn hình
        if (transform.position.x < -12)
        {
            Destroy(gameObject);
        }

    }
}
