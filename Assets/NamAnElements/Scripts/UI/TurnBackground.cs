using TMPro;
using UnityEngine;

public class TurnBackground : MonoBehaviour
{
    public static TurnBackground instance;
    public TextMeshProUGUI txtTurn;
    private void Awake()
    {
        instance = this;
    }
}
