using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("----------- Audio Source -----------")]

    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource sfxSource;

    [Header("----------- Audio Clip -----------")]
    public AudioClip backgroud;
    public AudioClip button;
    public AudioClip success;
    public AudioClip claim;
    public AudioClip alert;
    public AudioClip cancel;
    public AudioClip text;
    public AudioClip item;

    private void Start()
    {
        musicSource.clip = backgroud;
        musicSource.Play();
    }

    public void PlaySFXButton()
    {
        sfxSource.PlayOneShot(button);
    }

    public void PlaySFXSuccess()
    {
        sfxSource.PlayOneShot(success);
    }
    
    public void PlaySFXClaim()
    {
        sfxSource.PlayOneShot(claim);
    }

    public void PlaySFXAlert()
    {
        sfxSource.PlayOneShot(alert);
    }

    public void PlaySFXCancel()
    {
        sfxSource.PlayOneShot(cancel);
    }

    public void PlaySFXInputText()
    {
        sfxSource.PlayOneShot(text);
    }

    public void PlaySFXPickItem()
    {
        sfxSource.PlayOneShot(item);
    }
}
