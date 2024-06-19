using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetroyGameObject : MonoBehaviour
{
    private float screenWidth;
    // Start is called before the first frame update
    void Start()
    {
        screenWidth = Camera.main.aspect * Camera.main.orthographicSize;

    }

    // Update is called once per frame
    void Update()
    {
        //kiểm tra object vượt quá màn hình thì hủy
        if (transform.position.x < -screenWidth)
        {
            Destroy(gameObject);
        }
    }
}
