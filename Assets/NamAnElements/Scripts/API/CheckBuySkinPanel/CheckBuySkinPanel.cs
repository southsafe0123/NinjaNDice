using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckBuySkinPanel : MonoBehaviour
{
    public static CheckBuySkinPanel instance;
    public GameObject checkBuySkinPanel;
    string userID; string skinID;
    private void Awake()
    {
        instance = this;
    }
    public void SetDataSkinConfirmBuy(string userID, string skinID)
    {
        this.userID = userID;
        this.skinID = skinID;
    }
    public void ConfirmBuySkin()
    {
        ApiHandle.Instance.BuySkinButton(userID,skinID);
    }
    public void DisplayCheck(bool isActive)
    {
        checkBuySkinPanel.SetActive(isActive);
    }
}
