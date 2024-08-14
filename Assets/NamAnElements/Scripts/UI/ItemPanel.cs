using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemPanel : MonoBehaviour
{
    public Image itemImage;
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemAmount;
    private void OnEnable()
    {
        UpdateDisplayItemPanel();
    }

    private void UpdateDisplayItemPanel()
    {
        try
        {
            itemImage.sprite = ItemPool.Instance.GetCurrentPlayerItem().icon;
            itemName.text = ItemPool.Instance.GetCurrentPlayerItem().itemName;
            itemAmount.text = $"Have: {ItemPool.Instance.GetCurrentPlayerItemAmount().ToString()}";
        }
        catch (System.Exception)
        {
            itemImage.sprite = null;
            itemName.text = "You Dont Have Item";
            itemAmount.text = "";
            Debug.LogError("panel cant update item");
        }
       
    }

    public void OnClickNextItem()
    {
        ItemPool.Instance.OnClickNextItem();
        UpdateDisplayItemPanel();
    }
    public void OnClickPrevItem()
    {
        ItemPool.Instance.OnClickPrevItem();
        UpdateDisplayItemPanel();
    }
}
