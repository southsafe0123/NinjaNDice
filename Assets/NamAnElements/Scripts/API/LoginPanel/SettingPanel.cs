using TMPro;
using UnityEngine;

public class SettingPanel: MonoBehaviour
{
    public TextMeshProUGUI txtPlayerName;
    private void OnEnable()
    {
        txtPlayerName.text = UserSessionManager.Instance.username.ToString();
    }
}
