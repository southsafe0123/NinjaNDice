using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerScript : NetworkBehaviour
{
    public GameObject answerGameObject;
    public int life = 3;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Input.GetAxis("Horizontal") * Time.deltaTime * 5, Input.GetAxis("Vertical") * Time.deltaTime * 5, 0);

        // if (answerGameObject != null)
        // {

        //     if (!answerGameObject.activeSelf)
        //     {
        //         WrongAnswer();
        //     }
        // }
    }


    void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log(col.name);
        answerGameObject = col.gameObject;
    }

    public void WrongAnswer()
    {
        gameObject.transform.position = new Vector3(-8f, -1.6f, 0);
        life--;
        if (life == 0)
        {
            Debug.Log("Game Over");
            gameObject.SetActive(false);
        }
    }

}
