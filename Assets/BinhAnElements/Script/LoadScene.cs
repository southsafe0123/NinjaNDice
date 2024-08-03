using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public Animator trasition;
    public float transitionTime;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartLoadScene();
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
