using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerJump : MonoBehaviour
{
    public float jumpForce = 5f;
    private Rigidbody2D rb;
    public int hearts = 3; // số mạng

    private bool isGrounded;

    public GameObject bulletPrefab; // Prefab của đạn
    public float bulletSpeed = 10f; // Tốc độ của đạn
    public TextMeshProUGUI statusText; // TextMeshPro để hiển thị trạng thái
    public TextMeshProUGUI heartsText; // TextMeshPro để hiển thị số mạng
    public float fireCooldown = 3f; // Thời gian chờ trước khi bắn

    private bool canFire = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        UpdateStatusText();
        UpdateHeartsText();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.A) && canFire)
        {
            Fire();
        }
    }

    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }

    void GameOver()
    {
        Debug.Log("Game Over");
        Time.timeScale = 0; // Dừng lại game
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            hearts--;
            Debug.Log("Hearts remaining: " + hearts);
            UpdateHeartsText();

            if (hearts < 1)
            {
                GameOver();
            }
        }
    }

    void Fire()
    {
        if (bulletPrefab != null)
        {
            // Tạo một đạn tại vị trí nhân vật
            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();

            if (bulletRb != null)
            {
                bulletRb.velocity = transform.right * bulletSpeed; // Bắn theo hướng bên phải
            }

            // Bắt đầu thời gian chờ trước khi có thể bắn tiếp
            canFire = false;
            UpdateStatusText();
            StartCoroutine(FireCooldown());
        }
        else
        {
            Debug.LogWarning("Bullet prefab not assigned.");
        }
    }

    IEnumerator FireCooldown()
    {
        yield return new WaitForSeconds(fireCooldown);
        canFire = true;
        UpdateStatusText();
    }

    void UpdateStatusText()
    {
        if (statusText != null)
        {
            statusText.text = canFire ? "Attack" : "Bullet not ready";
        }
    }

    void UpdateHeartsText()
    {
        if (heartsText != null)
        {
            heartsText.text = "Hearts: " + hearts;
        }
    }
}
