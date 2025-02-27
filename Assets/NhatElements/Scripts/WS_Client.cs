using WebSocketSharp;
using UnityEngine;
using TMPro;
using System;
using System.Collections;
using UnityEngine.SceneManagement;

public class WS_Client : MonoBehaviour
{
    private WebSocket ws;
    private enum SslProtocolsHack
    {
        Tls = 192,
        Tls11 = 768,
        Tls12 = 3072
    }

    [SerializeField] private string url = "retstudio.io.vn";
    //[SerializeField] private string url = "mrxgame.loca.lt";

    [SerializeField] private TMP_InputField ID;
    [SerializeField] private TMP_Text message;
    [SerializeField] private string myID;
    private Coroutine reConnectCoroutine;
    public bool isConnect = false;

    // Start is called before the first frame update

    void Start()
    {

        //connect to ws server and send name
        //var ws = new WebSocket ("wss://example.com");
        //'wss://retstudio.io.vn/?userId=66aa283a7093a0984b7188e5'
        //WebSocket(string url, params string[] protocols)
        ws = new WebSocket("wss://" + url + "/?userId=" + UserSessionManager.Instance._id);
        // ws.Log.File = "Assets/log/WebSocketLog.txt";
        // ws.Log.Level = LogLevel.Debug;
        //ws = new WebSocket("wss://" + url + "/?userId=" + myID);
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
                }
                else if (e.Data == "money")
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
                                        AnouncementManager.instance.DisplayAnouncement("You got game invite from: "+ item.username);
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
            var sslProtocolHack = (System.Security.Authentication.SslProtocols)(SslProtocolsHack.Tls12 | SslProtocolsHack.Tls11 | SslProtocolsHack.Tls);
            //TlsHandshakeFailure
            if (e.Code == 1015 && ws.SslConfiguration.EnabledSslProtocols != sslProtocolHack)
            {
                ws.SslConfiguration.EnabledSslProtocols = sslProtocolHack;
                ws.Connect();
            }
            else
            {
                Debug.Log("WebSocket Close: " + e.Code + " " + e.Reason);
                isConnect = false;
                Debug.Log("disconnected");
            }

        };
        ws.Connect();
        reConnectCoroutine = StartCoroutine(TryReconnect());
    }

    private IEnumerator TryReconnect()
    {
        WaitUntil waitUntill = new WaitUntil(() => !isConnect);
        WaitForSecondsRealtime wait = new WaitForSecondsRealtime(5f);
        while (true)
        {
            yield return null;
            yield return waitUntill;
            ws.Connect();
            Debug.Log("Reconnect: " + ws.Url);
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
        ws.Send("invite:" + ApiHandle.Instance.user._id + ":" + _id + ":" + code);
    }

    //close connection when application is closed
    private void OnApplicationQuit()
    {
        DisconnectWS();
    }

    private void OnDestroy()
    {
        DisconnectWS();
    }

    private void OnDisable()
    {
        DisconnectWS();
    }
    public void DisconnectWS()
    {
        ws.Close();
        if (reConnectCoroutine != null)
        {
            StopCoroutine(reConnectCoroutine);
            reConnectCoroutine = null;
        }
        AnouncementManager.instance.DisplayAnouncement("Logged out!");
        AudioManager.Instance.PlaySFXShowDialog();
        Debug.Log("Disable");
    }
    public void reloadData()
    {
        ApiHandle.Instance.reloadData();
    }






}
