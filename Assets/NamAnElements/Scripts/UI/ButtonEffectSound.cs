using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonEffectSound : MonoBehaviour
{
    Button button;
    void Start()
    {
        button = GetComponent<Button>();
        button?.onClick.AddListener(() => { PlayEffect(); });
    }
    public void PlayEffect()
    {
        if (AudioManager.Instance.sfxSource.isPlaying) return;
        AudioManager.Instance.PlaySFXButton();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
