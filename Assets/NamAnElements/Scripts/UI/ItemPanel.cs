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
            itemImage.sprite = ItemPool.Instance.GetCurrentPlayerItem().item.icon;
            itemName.text = ItemPool.Instance.GetCurrentPlayerItem().item.itemName;
            itemAmount.text = $"Have: {ItemPool.Instance.GetCurrentPlayerItem().amount.ToString()}";
        }
        catch (System.Exception)
        {
            Debug.LogError("panel cant update item");
            throw;
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
