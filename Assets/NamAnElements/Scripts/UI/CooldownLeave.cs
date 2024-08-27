using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CooldownLeave : MonoBehaviour
{
    public Image cdImage;
    public TextMeshProUGUI txtTime;
    public float timer;
    private void OnEnable()
    {
        StartCoroutine(Cooldown_Corutine());
    }

    private IEnumerator Cooldown_Corutine()
    {
        timer = 0;
        WaitForSeconds wait = new WaitForSeconds(1);
        float timeRemain = 0;
        while (timer < 5)
        {
            timer += Time.deltaTime;
            timeRemain = 5 - Mathf.FloorToInt(timer); ;
            txtTime.text = timeRemain.ToString();
            cdImage.fillAmount = 5/5 - timer/5;
            yield return null;
        }
        cdImage.gameObject.SetActive(false);
        txtTime.gameObject.SetActive(false);
    }
}
