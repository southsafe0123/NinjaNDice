using TMPro;
using UnityEngine;

public class SettingPanel: MonoBehaviour
{
    public TextMeshProUGUI txtPlayerName;
    private void OnEnable()
    {
       
        if (UserSessionManager.Instance.username == null) return;

       txtPlayerName.text  = UserSessionManager.Instance.username.ToString();
    }
}
