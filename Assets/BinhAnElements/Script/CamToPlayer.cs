using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamToPlayer : MonoBehaviour
{
    public Player playerToFollow;
  
    private void FixedUpdate()
    {
        GetComponent<Cinemachine.CinemachineVirtualCamera>().Follow = playerToFollow.transform;
    }
}
