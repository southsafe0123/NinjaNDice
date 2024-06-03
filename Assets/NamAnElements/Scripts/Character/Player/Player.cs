using Unity.Netcode;
using UnityEngine;

public class Player:NetworkBehaviour
{
    private void Update()
    {
        if (!IsOwner) return;

        transform.position += new Vector3(3 * Input.GetAxisRaw("Horizontal"), 3* Input.GetAxisRaw("Vertical"),0)*Time.deltaTime;
    }
}
