using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageItem : Item
{
    //lượng sát thương gây ra từ item
    public int damageDeal = 3;

    ////phương thức start từ lớp cha
    //protected override void Start()
    //{
    //    //gọi phương thức start từ lớp cha trước khi gán giá trị cho itemName
    //    base.Start();

    //    //gán giá trị cho itemName trong lớp con
    //    itemName = "Damage Item"; 
    //}
    public override void itemEffect()
    {
        //gọi phương thức itemEffect từ lớp cha
        base.itemEffect();

        GameObject target = GameObject.FindGameObjectWithTag("Player");
        UseItem(target);

        // Viết code item cho nhân vật ở đây
        
        Debug.Log("Deal " + damageDeal + " damage with " + itemName);
    }

    public void UseItem(GameObject target)
    {
        // lấy mục tiêu muốn sử dụng item
        //PlayerHealth otherPlayer = target.GetComponent<PlayerHealth>();

        //// nếu máu của mục tiêu lớn hơn 0
        //if (otherPlayer != null)
        //{
        //    // Gây sát thương cho mục tiêu
        //    otherPlayer.Takedamage(damageDeal);
        }
    }

