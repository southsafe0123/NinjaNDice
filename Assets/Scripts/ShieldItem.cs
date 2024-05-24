using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldItem : Item
{
    //lá chắn được kích hoạt khi sử dụng item
    public bool shieldIsactive = true;

    ////phương thức start từ lớp cha
    //protected override void Start()
    //{
    //    //gọi phương thức start từ lớp cha trước khi gán giá trị cho itemName
    //    base.Start();

    //    //gán giá trị cho itemName trong lớp con
    //    itemName = "Shield Item"; 
    //}

    public override void itemEffect()
    {
        //gọi phương thức itemEffect từ lớp cha
        base.itemEffect();

        //// Viết code item cho nhân vật ở đây
        if (shieldIsactive)
        {
            damageDeal = 0;
            revertPosition = 2;
            playerHealth -= damageDeal;
            playerPosition -= revertPosition;
        }

        Debug.Log("Blocked by Shield");
    }
}
