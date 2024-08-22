using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamToPlayer : MonoBehaviour
{
    public static CamToPlayer instance;
    public Player playerToFollow;
    public Player playerInTurn;

    private void Awake()
    {
        instance = this;
    }
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
