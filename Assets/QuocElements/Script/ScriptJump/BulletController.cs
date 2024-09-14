using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{


    void Start()
    {
        ///hủy khi ra khỏi màn hình
        if (transform.position.x > 10)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Xử lý va chạm với các đối tượng khác nếu cần
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(collision.gameObject); // Hủy đối tượng enemy
            Destroy(gameObject); // Hủy đạn
        }
    }
}
