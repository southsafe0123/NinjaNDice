using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevertItem : Item
{
    //lượng sát thương gây ra từ item
    public int revertPosition = 2;

    ////phương thức start từ lớp cha
    //protected override void Start()
    //{
    //    //gọi phương thức start từ lớp cha trước khi gán giá trị cho itemName
    //    base.Start();

    //    //gán giá trị cho itemName trong lớp con
    //    itemName = "Revert Item"; 
    //}
    public override void itemEffect()
    {
        //gọi phương thức itemEffect từ lớp cha
        base.itemEffect();

        GameObject target = GameObject.FindGameObjectWithTag("Player");
        UseItem(target);

        // Viết code item cho nhân vật ở đây

        Debug.Log("Go back " + revertPosition + " step with " + itemName);
    }

    public void UseItem(GameObject target)
    {
        // lấy mục tiêu muốn sử dụng item
        PlayerPosition otherPlayer = target.GetComponent<PlayerPosition>();

        // nếu vị trí của mục tiêu lớn hơn 0
        if (otherPlayer > 0)
        {
            // Thay đổi vị trí mục tiêu
            otherPlayer.ChangePosition(revertPosition);
        }
    }
}
