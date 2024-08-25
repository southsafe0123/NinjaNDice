using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    [Header("----------- Audio Source -----------")]

    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("----------- Audio Clip -----------")]
    public AudioClip[] background;
    public AudioClip button;
    public AudioClip success;
    public AudioClip claim;
    public AudioClip alert;
    public AudioClip cancel;
    public AudioClip text;
    public AudioClip item;

    private List<AudioClip> remainingClips = new List<AudioClip>();
    public float fadeDuration = 2f;
    private Coroutine fadeCoroutine;

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
        remainingClips.AddRange(background);
        SceneManager.sceneLoaded += OnSceneLoaded;

        Debug.Log("Số nhạc còn lại trong list là: " + remainingClips.Count);
        int index = Random.Range(0, remainingClips.Count);
        musicSource.clip = remainingClips[index];
        musicSource.Play();
        remainingClips.RemoveAt(index);
        Debug.Log("Số nhạc còn lại trong list là: " + remainingClips.Count);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (remainingClips.Count == 0)
        {
            remainingClips.AddRange(background);
        }
        int index = Random.Range(0, remainingClips.Count);
        musicSource.clip = remainingClips[index];
        musicSource.Play();
        remainingClips.RemoveAt(index);
        Debug.Log("Số nhạc còn lại trong list là: " + remainingClips.Count);
    }

    private void Update()
    {
        if (!musicSource.isPlaying && musicSource.clip != null)
        {
            if (remainingClips.Count > 0)
            {
                PlayRandomBackgroundMusic();
                Debug.Log("Số nhạc còn lại trong list là: " + remainingClips.Count);
            }
            else
            {
                ResetPlaylist();
                PlayRandomBackgroundMusic();
                Debug.Log("Số nhạc còn lại trong list là: " + remainingClips.Count);
            }
        }
    }

    private void ResetPlaylist()
    {
        remainingClips.Clear();
        remainingClips.AddRange(background);
    }

    private void PlayRandomBackgroundMusic()
    {
        if (remainingClips.Count > 0)
        {
            int index = Random.Range(0, remainingClips.Count);
            AudioClip nextClip = remainingClips[index];

            // Nếu có một coroutine fade-out đang chạy, dừng nó
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }

            // Thực hiện fade-out cho bài nhạc hiện tại và chuyển sang bài nhạc mới
            fadeCoroutine = StartCoroutine(FadeOutAndPlayNewMusic(nextClip));

            musicSource.clip = nextClip;
            musicSource.Play();
            remainingClips.RemoveAt(index);
        }
    }


    private IEnumerator FadeOutAndPlayNewMusic(AudioClip newClip)
    {
        // Thực hiện fade-out cho bài nhạc hiện tại
        float startVolume = musicSource.volume;
        float elapsedTime = 1.25f;

        while (elapsedTime < fadeDuration)
        {
            musicSource.volume = Mathf.Lerp(startVolume, 0f, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        musicSource.volume = 0f;
        musicSource.Stop();

        musicSource.clip = newClip;
        musicSource.volume = 0f;
        musicSource.Play();
         
        elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            musicSource.volume = Mathf.Lerp(0f, startVolume, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        musicSource.volume = startVolume;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
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
