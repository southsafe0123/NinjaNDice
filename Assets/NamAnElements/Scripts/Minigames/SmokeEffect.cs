﻿using UnityEngine;

public class SmokeEffect : MonoBehaviour
{
    public GameObject objectActive;
    public void ActiveFalseObject()
    {
        objectActive.SetActive(false);
    }
    public void ActiveTrueObject()
    {
        objectActive.SetActive(true);
    }

    public void TringgerSoundEffectStart()
    {
        AudioManager.Instance.PlaySFXStartExplosion();
    }
    
    public void TringgerSoundEffectEnd()
    {
        AudioManager.Instance.PlaySFXEndExplosion();
    }
}