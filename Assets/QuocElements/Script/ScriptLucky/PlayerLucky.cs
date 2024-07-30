using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLucky : MonoBehaviour
{
    private Rigidbody2D rb;
    public float speed = 10f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //di chuyển 
        rb.velocity = new Vector2(Input.GetAxis("Horizontal") * speed, Input.GetAxis("Vertical") * speed);

    }
}
