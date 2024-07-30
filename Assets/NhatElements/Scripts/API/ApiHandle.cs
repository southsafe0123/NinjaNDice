using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using System.Text;
using UnityEngine.SceneManagement;

public class ApiHandle : MonoBehaviour
{

    [SerializeField] private string _apiUrl = "http://localhost:3000";
    [SerializeField] private TMP_InputField username;
    [SerializeField] private TMP_InputField password;
    [SerializeField] private TMP_Text message;



    // Start is called before the first frame update
    // void Start()
    // {

    // }

    // // Update is called once per frame
    // void Update()
    // {

    // }

    public void LoginButton()
    {
        StartCoroutine(Login());
    }

    public IEnumerator Login()
    {
        UnityWebRequest www = UnityWebRequest.Post(_apiUrl + "/login", "POST");
        www.SetRequestHeader("Content-Type", "application/json");
        UserRequest userRq = new UserRequest();
        userRq.username = username.text;
        userRq.password = password.text;
        string json = JsonUtility.ToJson(userRq);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        www.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        yield return www.SendWebRequest();
        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
            ErrorRespone errorRp = JsonUtility.FromJson<ErrorRespone>(www.downloadHandler.text);
            message.text = errorRp.message;
        }
        else
        {
            Debug.Log(www.downloadHandler.text);
            UserResponse userRp = JsonUtility.FromJson<UserResponse>(www.downloadHandler.text);
            message.text = "Login success";
            UserSessionManager.Instance._id = userRp._id;
            UserSessionManager.Instance.username = userRp.username;
            UserSessionManager.Instance.email = userRp.email;
            UserSessionManager.Instance.money = userRp.money;
            UserSessionManager.Instance.friends = userRp.friends;
            UserSessionManager.Instance.request = userRp.request;
            UserSessionManager.Instance.skinpurchase = userRp.skinpurchase;
            UserSessionManager.Instance.createdAt = userRp.createdAt;

            //cho 1s sau chuyen scene
            yield return new WaitForSeconds(1);
            SceneManager.LoadScene("test");
        }

    }
}


// ------------------------------Cac class khac--------------------------

public class UserSessionManager : MonoBehaviour
{
    // Instance của Singleton
    private static UserSessionManager _instance;
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

    // Các thông tin người dùng cần lưu trữ tạm thời
    public string _id { get; set; }
    public string username { get; set; }
    public string email { get; set; }
    public int money { get; set; }
    public List<friends> friends { get; set; }
    public List<request> request { get; set; }
    public List<skinpurchase> skinpurchase { get; set; }
    public string createdAt { get; set; }




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
    }
}


// ------------------------------Cac class module--------------------------
public class ErrorRespone
{
    public string message;
}
public class UserRequest
{
    public string username;
    public string password;
}
public class UserResponse
{
    //{"_id":"66a614e9c9281f689ea4f36e","username":"test","email":"testmail","money":0,"default":[],"friends":[],"request":[],"skinpurchase":[],"createdAt":"2024-07-28T09:52:41.009Z","__v":0}
    public string _id;
    public string username;
    public string email;
    public int money;
    public List<friends> friends;
    public List<request> request;
    public List<skinpurchase> skinpurchase;
    public string createdAt;
}

public class friends
{
    public string user; // id of friend
    public string dateAdded;
}

public class request
{
    public string user; // id of target
    public string dateRequested;
    public string status;
}

public class skinpurchase
{
    public string skin; // id of skin
    public string datePurchased;
}

public class skin
{
    public string _id;
    public string name;
    public int price;
    public string skinImage;
    public string createdAt;

}


