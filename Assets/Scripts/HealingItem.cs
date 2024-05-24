using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingItem : Item
{
    //số lượng máu hồi phục từ item
    public int healthAmount = 2;

    ////phương thức start từ lớp cha
    //protected override void Start()
    //{
    //    //gọi phương thức start từ lớp cha trước khi gán giá trị cho itemName
    //    base.Start();

    //    //gán giá trị cho itemName trong lớp con
    //    itemName = "Healing Item"; 
    //}

    public override void itemEffect()
    {
        //gọi phương thức itemEffect từ lớp cha
        base.itemEffect();

        // Viết code để hồi máu cho nhân vật ở đây
        // playerHealth += healthAmount;
        Debug.Log("Restoring " + healthAmount + " health with " + itemName);
    }
}