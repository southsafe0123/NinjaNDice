using TMPro;
using UnityEngine;

public class PlayerEndGameItem : MonoBehaviour
{
    public string playerName;
    public int goldPlus;
    public int top;
    public TextMeshProUGUI txtPlayerName;
    public TextMeshProUGUI txtGoldPlus;
    public TextMeshProUGUI txtTop;
    public void SetPlayerEndGameItem(string playerName, int goldPlus, int top)
    {
        this.playerName = playerName;
        this.goldPlus = goldPlus;
        this.top = top;

        txtPlayerName.text = playerName;
        txtGoldPlus.text = "+"+goldPlus.ToString()+"G";
        txtTop.text = "Top "+top.ToString();
    }
}