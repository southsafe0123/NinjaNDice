using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mono.CSharp;
using System.Linq;

public class UI_Controller : MonoBehaviour
{

    // [SerializeField] private UserSessionManager userSessionManager;
    public GameObject requestContent;
    public GameObject requestPrefab;
    public GameObject friendContent;
    public GameObject friendPrefab;
    public GameObject searchContent;
    public GameObject searchPrefab;
    public GameObject inviteContent;
    public GameObject invitePrefab;
    public GameObject skinContent;
    public GameObject skinPrefab;
    public TextMeshProUGUI moneyText;

    public static UI_Controller Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {



    }



    public void UpdateMoney()
    {
        moneyText.text = UserSessionManager.Instance.money.ToString();
    }

    public void UpdateRequest()
    {
        if (requestContent == null || requestPrefab == null) return;

        // Clear all request
        foreach (Transform child in requestContent.transform)
        {
            Destroy(child.gameObject);
            Debug.Log("delete Request");
        }
        if (ApiHandle.Instance.user.request == null)
        {
            return;
        }
        foreach (var item in ApiHandle.Instance.user.request)
        {

            //check id from request in ApiHandle.Instance.wRequest
            ApiHandle.Instance.wRequest.ForEach(request =>
            {
                if (request._id.Contains(item.from))
                {
                    GameObject requestItem = Instantiate(requestPrefab, requestContent.transform);
                    requestItem.GetComponent<RequestItem>().SetData(request.username);
                    requestItem.GetComponent<RequestItem>().setRequest(item);
                }
            });
        }
    }

    public void UpdateFriend()
    {
        // if (UserSessionManager.Instance.friendIngame == null)
        // {
        //     try
        //     {
        //         foreach (var item in UserSessionManager.Instance.friends)
        //         {
        //             StartCoroutine(ApiHandle.Instance.getStatus(item._id));
        //         }

        //     }
        //     catch (System.Exception)
        //     {

        //         Debug.Log("Can't get friend list");
        //         return;
        //     }
        // }

        // Clear all friend in friendContent
        if (friendContent == null || friendPrefab == null) return;
        foreach (Transform child in friendContent.transform)
        {
            Destroy(child.gameObject);
            Debug.Log("Destroy friend item");
        }
        try
        {
            for (int i = 0; i < ApiHandle.Instance.user.friends.Count; i++)
            {
                GameObject friendItem = Instantiate(friendPrefab, friendContent.transform);
                friendItem.GetComponent<FriendItem>().SetData(ApiHandle.Instance.user.friends[i].username, ApiHandle.Instance.user.friends[i].status, ApiHandle.Instance.user.friends[i]._id);
            }
        }
        catch (System.Exception)
        {
            Debug.Log("Can't get friend list");

        }

    }

    public void UpdateSearch(List<friendSearch> friend)
    {
        if (searchContent == null || searchPrefab == null) return;
        foreach (Transform child in searchContent.transform)
        {
            Destroy(child.gameObject);
            Debug.Log("Destroy friend item");
        }
        try
        {
            foreach (friendSearch friendsearch in friend)
            {
                Debug.Log(friendsearch.nameingame);
                if (friendsearch._id.Contains(UserSessionManager.Instance._id)) return;
                GameObject searchItem = Instantiate(searchPrefab, searchContent.transform);
                searchItem.GetComponent<SearchFriendItem>().SetData(friendsearch.username, friendsearch._id);

                foreach (friend myFriend in ApiHandle.Instance.user.friends)
                {
                    if (myFriend._id == friendsearch._id)
                    {
                        searchItem.GetComponent<SearchFriendItem>().btnAddFriend.interactable = false;
                        break;
                    }
                }
            }
           

        }
        catch (System.Exception e)
        {
            Debug.Log("Can't get search friend list: " + e);
        }
    }

    public void UpdateInvite(string name, string inviteCode)
    {

        // Clear all invite
        if (inviteContent == null || invitePrefab == null) return;
        foreach (Transform child in inviteContent.transform)
        {
            Destroy(child.gameObject);
            Debug.Log("Destroy friend item");
        }
        //do something
        try
        {
            foreach (Transform child in inviteContent.transform)
            {
                if (child.GetComponent<InviteItem>().name == name) return;
            }
            GameObject inviteItem = Instantiate(invitePrefab, inviteContent.transform);
            inviteItem.GetComponent<InviteItem>().SetData(name, inviteCode);
            AnouncementManager.instance.DisplayAnouncement("Invited from " + name);
        }
        catch (System.Exception e)
        {
            Debug.Log("Can't get search invite list: " + e);
        }
    }

    public void UpdateSkin()
    {
        if (skinContent == null || skinPrefab == null) return;
        foreach (Transform child in skinContent.transform)
        {
            if(child.GetComponent<ItemSkin>().skinName == "Default")
            {
                continue;
            }
            Destroy(child.gameObject);
            Debug.Log("Destroy skin item");
        }
        //do something
        try
        {
            foreach (skinpurchase skinPurchased in ApiHandle.Instance.user.skinpurchase)
            {
                GameObject skin = Instantiate(skinPrefab, skinContent.transform);
                skin.GetComponent<ItemSkin>().skinId = skinPurchased._id;
                skin.GetComponent<ItemSkin>().UpdateInfoSkin();
            }
            
      
        }
        catch (System.Exception e)
        {
            Debug.Log("Can't get search invite list: " + e);
        }
    }





}
