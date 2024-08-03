using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class UI_Controller : MonoBehaviour
{

    // [SerializeField] private UserSessionManager userSessionManager;
    [SerializeField] private GameObject requestContent;
    [SerializeField] private GameObject requestPrefab;
    [SerializeField] private GameObject friendContent;
    [SerializeField] private GameObject friendPrefab;
    [SerializeField] private GameObject inviteContent;
    [SerializeField] private GameObject invitePrefab;
    [SerializeField] private GameObject skinContent;
    [SerializeField] private GameObject skinPrefab;
    [SerializeField] private TMP_Text moneyText;

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
        // Clear all request
        foreach (Transform child in requestContent.transform)
        {
            Destroy(child.gameObject);
        }
        if (ApiHandle.Instance.user.request == null)
        {
            return;
        }
        foreach (var item in ApiHandle.Instance.user.request)
        {
            GameObject requestItem = Instantiate(requestPrefab, requestContent.transform);
            //check id from request in ApiHandle.Instance.wRequest
            ApiHandle.Instance.wRequest.ForEach(request =>
            {
                if (request._id == item.from)
                {
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
        foreach (Transform child in friendContent.transform)
        {
            Destroy(child.gameObject);
            Debug.Log("Destroy friend item");
        }
        try
        {
            for (int i = 0; i < ApiHandle.Instance.friendIngame.Count; i++)
            {
                GameObject friendItem = Instantiate(friendPrefab, friendContent.transform);
                friendItem.GetComponent<FriendItem>().SetData(ApiHandle.Instance.friendIngame[i].username, ApiHandle.Instance.friendIngame[i].status);

            }
        }
        catch (System.Exception)
        {
            Debug.Log("Can't get friend list");

        }

    }

    public void UpdateInvite()
    {
        // Clear all invite
        //do something
    }

    public void UpdateSkin()
    {
        // Clear all skin
        //do something
    }





}
