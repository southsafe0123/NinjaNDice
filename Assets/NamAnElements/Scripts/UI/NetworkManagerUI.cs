using Mono.CSharp;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine.UI;

public class NetworkManagerUI : NetworkBehaviour
{
    public static NetworkManagerUI Instance;
    public ButtonRollDice btnRollDice;
    public TextMeshProUGUI numDiceText;
    
    private void Awake()
    {
        Instance = this;
    }
}
