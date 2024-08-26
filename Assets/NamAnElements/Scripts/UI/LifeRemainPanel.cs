using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LifeRemainPanel : MonoBehaviour
{
    public static LifeRemainPanel instance;
    public TextMeshProUGUI txtHealth;
    public Image imgHealth;
    public Player player;
    private void Awake()
    {
        instance = this;    
    }
    private void Start()
    {
        player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Player>();
        UpdateHealth();
    }
    public void UpdateHealth()
    {
        txtHealth.text = "X" + player.GetComponent<PlayerHeath>().health;
        imgHealth.gameObject.transform.DOShakePosition(0.5f,0.2f);
    }
}
