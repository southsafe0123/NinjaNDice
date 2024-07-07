using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamToPlayer : MonoBehaviour
{
    public Player playerToFollow;
    private void Update()
    {
        Transform playerTransform = new GameObject().transform;
        playerTransform.position = new Vector3(0, playerToFollow.gameObject.transform.position.y, 0);
        GetComponent<Cinemachine.CinemachineVirtualCamera>().Follow = playerTransform;
    }
}
