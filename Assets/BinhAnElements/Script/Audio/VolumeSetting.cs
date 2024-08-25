using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;


public class VolumeSetting : MonoBehaviour
{
    public static VolumeSetting Instance;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;


    private void Start()
    {
        if (PlayerPrefs.HasKey("musicVolume") || PlayerPrefs.HasKey("sfxVolume"))
        {
            LoadVolume();
        }
        else
        {
            SetMusicVolume();
            SetSFXVolume();
        }
    }

    public void LoadVolume()
    {
        musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
        SetMusicVolume();

        sfxSlider.value = PlayerPrefs.GetFloat("sfxVolume");
        SetSFXVolume();
    }

    public void SetMusicVolume()
    {
        float volume = musicSlider.value;
        AudioManager.Instance.musicSource.volume = volume;
        PlayerPrefs.SetFloat("musicVolume", volume);
    }

    public void SetSFXVolume()
    {
        float volume = sfxSlider.value;
        AudioManager.Instance.sfxSource.volume = volume;
        PlayerPrefs.SetFloat("sfxVolume", volume);
    }
}
