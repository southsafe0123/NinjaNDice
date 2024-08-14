using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamToPlayer : MonoBehaviour
{
    public Player playerToFollow;
    public Player playerInTurn;
    private void FixedUpdate()
    {
        if(playerToFollow != null)
        {
            GetComponent<Cinemachine.CinemachineVirtualCamera>().Follow = playerToFollow.transform;
        }
        else
        {

        }
    }
}
