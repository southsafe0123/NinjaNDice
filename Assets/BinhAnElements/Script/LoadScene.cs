using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public static LoadScene Instance;
    public Animator trasition;
    public float transitionTime;
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartLoadScene()
    {
        StartCoroutine(PlayLoadScene(SceneManager.GetActiveScene().buildIndex + 1));
    }

    IEnumerator PlayLoadScene(int sceneIndex)
    {
        trasition.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime); 
        SceneManager.LoadScene(sceneIndex);
    }
}
