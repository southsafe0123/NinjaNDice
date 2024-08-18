using WebSocketSharp;
using UnityEngine;
using TMPro;
using System;
using System.Collections;
using UnityEngine.SceneManagement;

public class WS_Client : MonoBehaviour
{
    private WebSocket ws;

    [SerializeField] private string url = "mrxgame.loca.lt";
    [SerializeField] private TMP_InputField ID;
    [SerializeField] private TMP_Text message;
    [SerializeField] private string myID;

    [SerializeField] private bool isConnect = false;
    // Start is called before the first frame update

    void Start()
    {
        
        StartCoroutine(TryReconnect());
        //connect to ws server and send name
        ws = new WebSocket("ws://" + url + "?userId=" + UserSessionManager.Instance._id);

        myID = UserSessionManager.Instance._id;
        ws.OnOpen += (sender, e) =>
        {
            Debug.Log("Connected");
            isConnect = true;
        };
        ws.OnMessage += (sender, e) =>
        {

            try
            {
                Debug.Log("Message received: " + e.Data);
                // Xử lý thông điệp ở đây
                if (e.Data == "request")
                {
                    UnityMainThreadDispatcher.Instance().Enqueue(() => reloadData());

                    UnityMainThreadDispatcher.Instance().Enqueue(() =>
                    {
                        StartCoroutine(ApiHandle.Instance.GetAllRequestname(ApiHandle.Instance.user.request));
                    });


                }
                else if (e.Data == "friend")
                {
                    UnityMainThreadDispatcher.Instance().Enqueue(() => reloadData());
                    UnityMainThreadDispatcher.Instance().Enqueue(() => UI_Controller.Instance.UpdateFriend());
                }else if (e.Data == "money")
                {
                    UnityMainThreadDispatcher.Instance().Enqueue(() => reloadData());
                    UnityMainThreadDispatcher.Instance().Enqueue(() => UI_Controller.Instance.UpdateMoney());
                }
                else
                {
                    //invite:idfriend:code
                    UnityMainThreadDispatcher.Instance().Enqueue(() => reloadData());

                    UnityMainThreadDispatcher.Instance().Enqueue(() =>
                    {
                        string[] data = e.Data.Split(':');
                        if (data[0] == "invite")
                        {
                            if (SceneManager.GetActiveScene().name == "MenuScene")
                            {

                            }
                            if (SceneManager.GetActiveScene().name == "LobbyScene")
                            {
                                foreach (var item in ApiHandle.Instance.user.friends)
                                {
                                    if (item._id == data[1])
                                    {
                                        UI_Controller.Instance.UpdateInvite(item.username, data[2]);
                                        Debug.Log("Invite from: " + item.username + " code: " + data[2]);
                                    }
                                }
                            }

                        }
                    });
                    

                }
            }
            catch (Exception ex)
            {
                Debug.LogError("An error occurred during OnMessage event: " + ex.Message);
                Debug.LogError(ex.StackTrace);
            }


        };
        ws.OnError += (sender, e) =>
            {
                Debug.LogError("WebSocket Error: " + e.Message);
                if (e.Exception != null)
                {
                    Debug.LogError("Exception: " + e.Exception.Message);
                    Debug.LogError(e.Exception.StackTrace);
                }
            };
        ws.OnClose += (sender, e) =>
        {
            isConnect = false;
            Debug.Log("disconnected");
        };
        ws.Connect();

    }

    private IEnumerator TryReconnect()
    {
        WaitUntil waitUntill = new WaitUntil(() => !isConnect);
        WaitForSeconds wait = new WaitForSeconds(3f);
        while (true)
        {
            yield return null;
            yield return waitUntill;
            ws.Connect();
            yield return wait;
        }
        //if (ws == null)
        //{
        //    return;
        //}
        //if (isConnect == false)
        //{
        //    try
        //    {
        //        ws.Connect();
        //    }
        //    catch (System.Exception)
        //    {

        //        Debug.Log("Can't connect to server" + ws.Url + " " + ws.ReadyState);
        //    }
        //}
    }

    // Update is called once per frame
    void Update()
    {
        //if (ws == null)
        //{
        //    return;
        //}
        //if (isConnect == false)
        //{
        //    try
        //    {
        //        ws.Connect();
        //    }
        //    catch (System.Exception)
        //    {

        //        Debug.Log("Can't connect to server" + ws.Url + " " + ws.ReadyState);
        //    }
        //}

    }

    public void SendButton(string _id, string code)
    {
        ws.Send("invite:"+ApiHandle.Instance.user._id+":"+_id+":"+code);
    }

    //close connection when application is closed
    private void OnApplicationQuit()
    {
        isConnect = false;
        ws.Close();
        Debug.Log("Application ending after " + Time.time + " seconds");

    }

    private void OnDestroy()
    {
        isConnect = false;
        ws.Close();
        Debug.Log("Destroy");
    }

    private void OnDisable()
    {
        isConnect = false;
        ws.Close();
        Debug.Log("Disable");
    }

    public void reloadData()
    {
        ApiHandle.Instance.reloadData();
    }






}
