using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlayerController2D : NetworkBehaviour
{
    private Rigidbody2D _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput<NetworkInputData>(out var inputData))
        {
            Vector2 moveDirection = new Vector2(inputData.direction.x, inputData.direction.y);
            _rigidbody.velocity = moveDirection * 5f; // Tốc độ di chuyển
        }
    }
}
