using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    [Header("----------- Audio Source -----------")]

    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("----------- Audio Clip -----------")]
    public AudioClip backgroud;
    public AudioClip button;
    public AudioClip success;
    public AudioClip claim;
    public AudioClip alert;
    public AudioClip cancel;
    public AudioClip text;
    public AudioClip item;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
       
        musicSource.volume = 0.3f;
        sfxSource.volume = 0.3f;
        musicSource.clip = backgroud;
        musicSource.Play();
    }

    public void PlaySFXButton()
    {
        sfxSource.PlayOneShot(button);
        PlayRandomPitch();
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

    public void PlayRandomPitch()
    {
        sfxSource.pitch = UnityEngine.Random.Range(2, 4);
    }
}
