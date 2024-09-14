using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockFall : MonoBehaviour
{
    public void DestroyObject()
    {
        Destroy(gameObject.transform.parent.gameObject);
    }
    public void TringgerSoundEffectStart()
    {
        AudioManager.Instance.PlaySFXFalling();
    }

    public void TringgerSoundEffectEnd()
    {
        AudioManager.Instance.PlaySFXFallingHit();
    }
}
