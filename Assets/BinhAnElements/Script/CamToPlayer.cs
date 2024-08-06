using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamToPlayer : MonoBehaviour
{
    public Player playerToFollow;
    public Player playerInTurn;
    private void FixedUpdate()
    {
        GetComponent<Cinemachine.CinemachineVirtualCamera>().Follow = playerToFollow.transform;
    }
}
