using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRun : MonoBehaviour
{
    public float speed = 10f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // di chuyển 
        transform.Translate(Input.GetAxis("Horizontal") * Time.deltaTime * 5, Input.GetAxis("Vertical") * Time.deltaTime * 5, 0);


    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //chạm tag "cell" quay về vị trí cũ mặc định
        if (collision.tag == "Cell")
        {
            transform.position = new Vector2(-10, 0);
        }
    }
}
