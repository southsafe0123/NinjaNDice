using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserSessionManager : MonoBehaviour
{
    // Instance của Singleton
    private static UserSessionManager _instance;
    // Các thông tin người dùng cần lưu trữ tạm thời
    public string _id { get; set; }
    public string username { get; set; }
    public string email { get; set; }
    public int money { get; set; }
    public List<friend> friends { get; set; }
    public List<friendIngame> friendIngame { get; set; }
    public List<request> request { get; set; }
    public List<skinpurchase> skinpurchase { get; set; }
    public string createdAt { get; set; }
    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Đảm bảo chỉ có một instance của class này
    public static UserSessionManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("UserSessionManager");
                _instance = go.AddComponent<UserSessionManager>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    public void SetData(UserResponse user)
    {
        _id = user._id;
        username = user.username;
        email = user.email;
        money = user.money;
        friends = user.friends;
        request = user.request;
        skinpurchase = user.skinpurchase;
        createdAt = user.createdAt;
    }

    //set freindIngame
    public void SetFriendIngame(List<friendIngame> friendIngame)
    {
        this.friendIngame = friendIngame;
    }


    // Hàm để reset hoặc xóa thông tin khi người dùng thoát
    public void ClearSession()
    {
        _id = null;
        username = null;
        email = null;
        money = 0;
        friends = null;
        request = null;
        skinpurchase = null;
        createdAt = null;
        friendIngame = null;
    }

}
