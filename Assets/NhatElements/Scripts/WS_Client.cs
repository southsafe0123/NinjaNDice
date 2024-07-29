using WebSocketSharp;
using UnityEngine;
using TMPro;

public class WS_Client : MonoBehaviour
{
    private WebSocket ws;

    [SerializeField] private TMP_InputField ID;
    [SerializeField] private TMP_Text message;
    // Start is called before the first frame update
    void Start()
    {
        //connect to ws server and send name
        ws = new WebSocket("ws://localhost:3000?userId=123");
        ws.OnOpen += (sender, e) =>
        {
            Debug.Log("Connected");
            ws.Send("Unity");
        };
        ws.OnMessage += (sender, e) =>
        {
            Debug.Log("Message: " + e.Data);
            message.text = e.Data;
        };
        ws.Connect();

    }

    // Update is called once per frame
    void Update()
    {
        if (ws == null)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ws.Ping();
        }

    }

    public void SendButton()
    {
        ws.Send(ID.text);
    }
}
