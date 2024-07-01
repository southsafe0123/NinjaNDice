using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class playerFrozen : MonoBehaviour
{
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
        if (collision.gameObject.tag == "Cell")
        {
            StartCoroutine(HandleCollision(collision.gameObject));

        }

    }
    private IEnumerator HandleCollision(GameObject obj)
    {
        // Sau 1 giây đổi màu game object tag "Cell" thành màu xanh
        yield return new WaitForSeconds(1f);
        obj.GetComponent<SpriteRenderer>().color = Color.green;

        // Sau 2 giây đổi màu game object tag "Cell" thành màu đỏ
        yield return new WaitForSeconds(1f);
        obj.GetComponent<SpriteRenderer>().color = Color.red;

        // Sau 3 giây biến mất game object
        yield return new WaitForSeconds(1f);
        Destroy(obj);
    }

}
