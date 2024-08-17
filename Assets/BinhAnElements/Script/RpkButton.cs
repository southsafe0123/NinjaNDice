using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class RpkButton : MonoBehaviour
{
    public GameObject resultLeft;
    public GameObject resultRight;
    public Button rockButton;
    public Button paperButton;
    public Button kunaiButton;
    public Map fightMap;
    public Player player;

    private void OnEnable()
    {
        ConnetResult();
    }
    private void Start()
    {
    }

    public void ConnetResult()
    {
        player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Player>();
        if (player.transform.position == fightMap.movePos[0].position)
        {
            rockButton.onClick.AddListener(() => OnRockButtonClick(resultLeft));
            paperButton.onClick.AddListener(() => OnPaperButtonClick(resultLeft));
            kunaiButton.onClick.AddListener(() => OnKunaiButtonClick(resultLeft));
        }
        if (player.transform.position == fightMap.movePos[1].position)
        {
            rockButton.onClick.AddListener(() => OnRockButtonClick(resultRight));
            paperButton.onClick.AddListener(() => OnPaperButtonClick(resultRight));
            kunaiButton.onClick.AddListener(() => OnKunaiButtonClick(resultRight));
        }
    }

    private void OnRockButtonClick(GameObject result)
    {
        foreach (Transform child in result.transform)
        {
            child.gameObject.SetActive(false);
            if (child.gameObject.name == "Rock")
            {
                child.gameObject.SetActive(true);
                Debug.Log("Rock");
            }
        }
    }

    private void OnPaperButtonClick(GameObject result)
    {
        foreach (Transform child in result.transform)
        {
            child.gameObject.SetActive(false);
            if (child.gameObject.name == "Scroll")
            {
                child.gameObject.SetActive(true);
                Debug.Log("Scroll");
            }
        }
    }

    private void OnKunaiButtonClick(GameObject result)
    {
        foreach (Transform child in result.transform)
        {
            child.gameObject.SetActive(false);
            if (child.gameObject.name == "Kunai")
            {
                child.gameObject.SetActive(true);
                Debug.Log("Kunai"); 
            }
        }
    }
}
