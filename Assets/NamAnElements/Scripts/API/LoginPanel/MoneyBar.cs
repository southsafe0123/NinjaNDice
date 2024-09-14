using TMPro;
using UnityEngine;

public class MoneyBar : MonoBehaviour
{
    public TextMeshProUGUI txtMoney;
    private void Start()
    {
        UI_Controller.Instance.moneyText = txtMoney;
        UI_Controller.Instance.UpdateMoney();
    }
}
