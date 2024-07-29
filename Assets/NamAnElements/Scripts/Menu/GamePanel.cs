using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GamePanel : MonoBehaviour
{
    public Button btnPlay;
    //public Slider musicSlider;
    //public Slider soundSlider;
    //public AudioSource audioSource;


    private void Start()
    {
        btnPlay.onClick.AddListener(OnClickPlay);
        //musicSlider.value = audioSource.volume;
        //soundSlider.value = audioSource.volume;

        //musicSlider.onValueChanged.AddListener(SetVolume);

        //soundSlider.onValueChanged.AddListener(SetVolume);
    }

    //public void SetVolume(float volume)
    //{
    //    audioSource.volume = volume;
    //}



    private void OnClickPlay()
    {
        SceneManager.LoadScene("LobbyScene");
    }


}
