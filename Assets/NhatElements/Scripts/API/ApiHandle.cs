using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using System.Text;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using System.Linq;
using System;
using Google.MiniJSON;
using System.Threading;
using UnityEditor.PackageManager;

public class ApiHandle : MonoBehaviour
{        //int timer = 0;
         //WaitForSeconds wait1sec = new WaitForSeconds(1);
         //while (timer < maxLoadTime)
         //{
         //    while (!www.downloadHandler.isDone && timer < maxLoadTime)
         //    {
         //        timer += 1;
         //        yield return wait1sec;
         //    } 
         //}
    public static ApiHandle Instance { get; private set; }

    [SerializeField] private string _apiUrl = "https://retstudio.io.vn";
    //https://retstudio.io.vn
    // [SerializeField] private string _apiUrl = "http://localhost:3000";
    [SerializeField] private TMP_Text message;
    [SerializeField] private UI_Controller uiController;

    [SerializeField] public List<friendIngame> friendIngame;

    public UserResponse user;

    [SerializeField] public bool isNewData = false;

    //list name of user request
    [SerializeField] public List<whoRequest> wRequest;

    [SerializeField] public List<skin> skins;

    bool isCoroutineDone = false;

    public int maxLoadTime = 5;

    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null)
        {
            Application.targetFrameRate = 60;
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        if (!PrefsData.HaveData(PrefsData.PLAYER_INGAME_NAME_NOLOGIN))
        {
            PrefsData.SetData(PrefsData.PLAYER_INGAME_NAME_NOLOGIN, "Player_" + UnityEngine.Random.Range(1000, 9000));
        }


        StartCoroutineWithTimeout(CheckUrlConnection());

        GetAllSkin();
    }
    public void StartCoroutineWithTimeout(IEnumerator coroutine)
    {
        Coroutine coroutineToStop = StartCoroutine(coroutine);
        StartCoroutine(CheckGetDataTimeout(coroutineToStop));
    }
    IEnumerator CheckGetDataTimeout(Coroutine coroutine)
    {
        float i = 0f;

        while (i < maxLoadTime)
        {
            i += Time.deltaTime;
            if (isCoroutineDone)
            {
                break;
            }
            // No timeout yet.
            yield return null;
        }
        // Request timeout.
        if (isCoroutineDone)
        {
            isCoroutineDone = false;
            yield break;
        }
        StopCoroutine(coroutine);
        AnouncementManager.instance.DisplayAnouncement("Connection timeout!");
        LoadingPanel.Instance.SetDisplayLoading(false);
    }
    IEnumerator CheckUrlConnection()
    {
        LoadingPanel.Instance.SetDisplayLoading(true);
        yield return null;
        UnityWebRequest www = new UnityWebRequest(_apiUrl + "/ping", "GET");
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");
        yield return www.SendWebRequest();

        try
        {
            Debug.Log(www.downloadHandler.text);
            ErrorRespone errorRp = JsonConvert.DeserializeObject<ErrorRespone>(www.downloadHandler.text);
            if (errorRp.message == "Pong!")
            {
                Debug.Log("Connected to database successfully!");
                Debug.Log(PlayerPrefs.HasKey("username") + "/playerPrefs/" + PlayerPrefs.HasKey("unityId"));

                if (PrefsData.HaveData(PrefsData.PLAYER_USERNAME_LOGIN) || PrefsData.HaveData(PrefsData.PLAYER_PASSWORD_LOGIN))
                {
                    StartCoroutine(Login(PrefsData.GetData(PrefsData.PLAYER_USERNAME_LOGIN), PrefsData.GetData(PrefsData.PLAYER_PASSWORD_LOGIN)));
                }
                else if (PrefsData.HaveData(PrefsData.PLAYER_ID_UNITY_LOGIN))
                {
                    StartCoroutine(LoginID(PrefsData.GetData(PrefsData.PLAYER_ID_UNITY_LOGIN), "", "Unity"));
                }
                else
                {
                    LoadingPanel.Instance.SetDisplayLoading(false);
                    isCoroutineDone = true;
                }
            }
            else
            {
                AnouncementManager.instance.DisplayAnouncement("Login failed, check your internet connection");
                Debug.Log("Connected to database failed!");
                LoadingPanel.Instance.SetDisplayLoading(false);
                isCoroutineDone = true;
            }
        }
        catch (Exception)
        {

        }

    }

    // Update is called once per frame
    // void Update()
    // {

    // }
    public void SetSkinButton(string skinId)
    {
        StartCoroutineWithTimeout(SetSkin(skinId));
    }
    public void GetAllSkin()
    {
        StartCoroutine(getAllSkins());
    }
    public void ChangeNameButton(string name)
    {
        StartCoroutineWithTimeout(ChangeNameIngame(name));
    }
    public void LoginButton(string usernameLogin, string passwordLogin)
    {
        StartCoroutineWithTimeout(Login(usernameLogin, passwordLogin));

    }

    public void BuySkinButton(string userId, string skinId)
    {
        StartCoroutine(BuySkin(userId, skinId));
    }

    public skin skisn1(skin s1)
    {
        StartCoroutine(getSkin(s1));
        return s1;
    }
    public void Loginid(string ID, string username, string type)
    {
        StartCoroutineWithTimeout(LoginID(ID, username, type));
    }
    public void LogoutButton()
    {
        UserSessionManager.Instance.ClearSession();
    }

    public void SearchFriendButton(TMP_InputField nameSearch)
    {
        StartCoroutine(SearchFriend(nameSearch));
    }

    public void AddFriendButton(string _id)
    {
        StartCoroutine(AddFriend(_id));
    }
    public void DeleteFriendButton(string _id)
    {
        StartCoroutine(DeleteFriend(_id));
    }
    public void AcceptFriendButton()
    {
        StartCoroutine(AcceptFriend(UserSessionManager.Instance.request[0]));
    }

    public void reloadData()
    {
        StartCoroutine(getMe());
    }
    public IEnumerator SetSkin(string skinId)
    {
        LoadingPanel.Instance.SetDisplayLoading(true);
        // post, endpoint: /sendFriendRequest , body: {from: "id", to: "id"}
        PlayerAvatar playerAvatar = new PlayerAvatar();
        playerAvatar.userId = user._id;
        playerAvatar.avatar = skinId;
        string json = JsonUtility.ToJson(playerAvatar);
        Debug.Log(json);

        UnityWebRequest www = new UnityWebRequest(_apiUrl + "/changeAvatar", "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();

        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);

            if (www.downloadHandler != null)
            {
                ErrorRespone errorRp = JsonConvert.DeserializeObject<ErrorRespone>(www.downloadHandler.text);
                if (message != null) { message.text = errorRp.message; }
                else
                {
                    Debug.Log(errorRp.message);
                    AnouncementManager.instance.DisplayAnouncement(errorRp.message);
                }
            }
            else
            {
                Debug.LogError("Download handler is null");
            }
        }
        else
        {
            Debug.Log(www.downloadHandler.text);
            AnouncementManager.instance.DisplayAnouncement("Skin equiped");

            user.avatar = skinId;

            if (message != null) { message.text = "Send request success"; }
            else { Debug.Log("Send request success"); }
        }

        LoadingPanel.Instance.SetDisplayLoading(false);
        isCoroutineDone = true;

    }
    public void RegisterButton(TMP_InputField usernameRegister, TMP_InputField EmailRegister, TMP_InputField passwordRegister, TMP_InputField RepasswordRegister)
    {
        StartCoroutineWithTimeout(Register(usernameRegister, EmailRegister, passwordRegister, RepasswordRegister));
    }

    public IEnumerator LoginID(string ID, string username, string type = "Unity")
    {
        if (ID == null)
        {
            Debug.LogError("field is null");
            yield break;
        }

        for (int i = 0; i < 3; i++)
        {
            LoadingPanel.Instance.SetDisplayLoading(true);

            userIDrequest userRq = new userIDrequest();
            userRq.type = type;
            userRq.username = username;
            userRq.id = ID;

            string json = JsonUtility.ToJson(userRq);
            Debug.Log(json);

            UnityWebRequest www = new UnityWebRequest(_apiUrl + "/loginId", "POST");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                //if HTTP/1.1 502 Bad Gateway try again two times
                if (www.downloadHandler != null)
                {
                    Debug.Log(www.downloadHandler.text);
                    ErrorRespone errorRp = JsonConvert.DeserializeObject<ErrorRespone>(www.downloadHandler.text);



                    if (message != null)
                    {
                        message.text = errorRp.message;

                    }
                    else
                    {
                        Debug.Log(errorRp.message);
                        AnouncementManager.instance.DisplayAnouncement(errorRp.message);
                    }
                }
                else
                {
                    Debug.LogError("Download handler is null");
                }

                AnouncementManager.instance.DisplayAnouncement("Bad Connection, please try again");
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
                UserResponse userRp = JsonConvert.DeserializeObject<UserResponse>(www.downloadHandler.text);
                user = userRp;
                //Debug.Log("username: " + userRp.username + " money: " + userRp.money + " email: " + userRp.email + " role: " + userRp.role + "numrequest: " + userRp.request.Count);
                if (message != null) { message.text = "Login success"; }
                else { Debug.Log("Login success"); }
                try
                {
                    //luu du lieu autologin
                    PrefsData.SetData(PrefsData.PLAYER_ID_UNITY_LOGIN, ID);

                    if (UserSessionManager.Instance != null)
                    {
                        UserSessionManager.Instance.SetData(userRp);
                    }

                    else
                    {
                        Debug.LogError("UserSessionManager instance is null");
                    }
                }
                catch (System.Exception)
                {
                    Debug.Log("Can't set data to UserSessionManager");
                }
                Debug.Log("username: " + UserSessionManager.Instance.username + " money: " + UserSessionManager.Instance.money + " email: " + UserSessionManager.Instance.email + "numrequest: " + UserSessionManager.Instance.request.Count);

                yield return new WaitForSeconds(1);
                gameObject.AddComponent<WS_Client>();

                WelcomePanel.instance.DisplayPanel(false);

                if (uiController != null)
                {
                    uiController.UpdateMoney();
                    StartCoroutine(GetAllRequestname(user.request));
                    uiController.UpdateFriend();
                    uiController.UpdateSkin();
                    if (SettingPanel.instance != null) SettingPanel.instance.SetAvatar();
                    PlayerSkin.instance.UpdateSkin();
                }
                AnouncementManager.instance.DisplayAnouncement("Welcome back, " + user.nameingame);
                break;
            }

            yield return null;
        }
        LoadingPanel.Instance.SetDisplayLoading(false);
        isCoroutineDone = true;
    }

    public IEnumerator ChangeNameIngame(string name)
    {
        UserChangeNameData userchangename = new UserChangeNameData();
        userchangename.userId = UserSessionManager.Instance._id;
        userchangename.nameingame = name;

        string json = JsonUtility.ToJson(userchangename);
        Debug.Log(json);

        UnityWebRequest www = new UnityWebRequest(_apiUrl + "/changeNameInGame", "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();

        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
            if (www.responseCode == 502)
            {
                LoadingPanel.Instance.SetDisplayLoading(true);
                yield return new WaitForSeconds(1.5f);
                yield return StartCoroutine(ChangeNameIngame(name));
                LoadingPanel.Instance.SetDisplayLoading(false);
            }
            if (www.downloadHandler != null)
            {
                ErrorRespone errorRp = JsonConvert.DeserializeObject<ErrorRespone>(www.downloadHandler.text);
                if (message != null) { message.text = errorRp.message; }
                else
                {
                    Debug.Log(errorRp.message);
                    AnouncementManager.instance.DisplayAnouncement(errorRp.message);
                }
            }
            else
            {
                Debug.LogError("Download handler is null");
            }
        }
        else
        {
            Debug.Log(www.downloadHandler.text);

            yield return StartCoroutine(getMe());
            if (message != null) { message.text = "Change Success!"; }
            AnouncementManager.instance.DisplayAnouncement("Change Success!");

        }
        LoadingPanel.Instance.SetDisplayLoading(false);
        isCoroutineDone = true;
    }
    public IEnumerator Login(string usernameLogin, string passwordLogin)
    {
        if (usernameLogin == null || passwordLogin == null)
        {
            AnouncementManager.instance.DisplayAnouncement("Field is null");
            Debug.LogError("Username or Password field is null");
            yield break;
        }

        for (int i = 0; i < 3; i++)
        {
            LoadingPanel.Instance.SetDisplayLoading(true);

            UserRequest userRq = new UserRequest();
            userRq.username = usernameLogin;
            userRq.password = passwordLogin;
            string json = JsonUtility.ToJson(userRq);
            Debug.Log(json);

            UnityWebRequest www = new UnityWebRequest(_apiUrl + "/login", "POST");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            yield return www.SendWebRequest();


            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                //if (www.responseCode == 502)
                //{
                //    LoadingPanel.Instance.SetDisplayLoading(true);
                //    yield return new WaitForSeconds(1.5f);
                //    yield return StartCoroutine(Login(usernameLogin, passwordLogin));
                //    LoadingPanel.Instance.SetDisplayLoading(false);
                //}
                if (www.downloadHandler != null)
                {
                    ErrorRespone errorRp = JsonConvert.DeserializeObject<ErrorRespone>(www.downloadHandler.text);
                    if (message != null)
                    {
                        message.text = errorRp.message;


                    }
                    else
                    {
                        Debug.Log(errorRp.message);
                        AnouncementManager.instance.DisplayAnouncement(errorRp.message);
                    }


                }
                else
                {
                    Debug.LogError("Download handler is null");
                    AnouncementManager.instance.DisplayAnouncement("Bad Connection, please try again ");
                }

            }
            else
            {
                Debug.Log(www.downloadHandler.text);
                UserResponse userRp = JsonConvert.DeserializeObject<UserResponse>(www.downloadHandler.text);
                user = userRp;
                //Debug.Log("username: " + userRp.username + " money: " + userRp.money + " email: " + userRp.email + " role: " + userRp.role + "numrequest: " + userRp.request.Count);
                if (message != null) { message.text = "Login success"; }
                else { Debug.Log("Login success"); }
                try
                {
                    //luu du lieu autologin
                    PrefsData.SetData(PrefsData.PLAYER_USERNAME_LOGIN, usernameLogin);
                    PrefsData.SetData(PrefsData.PLAYER_PASSWORD_LOGIN, passwordLogin);

                    if (UserSessionManager.Instance != null)
                    {
                        UserSessionManager.Instance.SetData(userRp);
                    }

                    else
                    {
                        Debug.LogError("UserSessionManager instance is null");
                    }
                }
                catch (System.Exception)
                {

                    Debug.Log("Can't set data to UserSessionManager");
                }
                Debug.Log("username: " + UserSessionManager.Instance.username + " money: " + UserSessionManager.Instance.money + " email: " + UserSessionManager.Instance.email + "numrequest: " + UserSessionManager.Instance.request.Count);

                yield return new WaitForSeconds(1);
                gameObject.AddComponent<WS_Client>();
                if (uiController != null)
                {
                    uiController.UpdateMoney();
                    StartCoroutine(GetAllRequestname(user.request));
                    uiController.UpdateFriend();
                    uiController.UpdateSkin();
                    if (SettingPanel.instance != null) SettingPanel.instance.SetAvatar();
                    PlayerSkin.instance.UpdateSkin();
                }

                AnouncementManager.instance.DisplayAnouncement("Welcome back, " + usernameLogin);
                break;
            }

        }
        LoadingPanel.Instance.SetDisplayLoading(false);
        isCoroutineDone = true;
    }

    public IEnumerator GetAllFriendsStatus(List<friend> friends)
    {
        foreach (var item in friends)
        {
            yield return StartCoroutine(getStatus(item._id));
        }
    }
    public IEnumerator GetAllRequestname(List<request> requests)
    {
        foreach (var item in requests)
        {
            if (item.status == "pending" && item.to == UserSessionManager.Instance._id)
            {
                yield return StartCoroutine(getName(item.from));
            }
        }
    }

    public IEnumerator Register(TMP_InputField usernameRegister, TMP_InputField EmailRegister, TMP_InputField passwordRegister, TMP_InputField RepasswordRegister)
    {
        if (passwordRegister == null)
        {
            isCoroutineDone = true;
            yield break;
        } 
        if (passwordRegister.text != RepasswordRegister.text)
        {
            AnouncementManager.instance.DisplayAnouncement("Password not match!");
            if (message != null) { message.text = "Password not match"; }
            else { Debug.Log("Password not match"); }
            isCoroutineDone = true;
            yield break;
        }
        LoadingPanel.Instance.SetDisplayLoading(false);
        if (usernameRegister == null || passwordRegister == null)
        {
            Debug.LogError("Username or Password field is null");
            isCoroutineDone = true;
            yield break;
        }

        UserRequestRegister userRq = new UserRequestRegister();
        userRq.username = usernameRegister.text;
        userRq.password = passwordRegister.text;
        userRq.email = EmailRegister.text;
        string json = JsonConvert.SerializeObject(userRq);
        Debug.Log(json);

        UnityWebRequest www = new UnityWebRequest(_apiUrl + "/register", "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
            Debug.Log(www.downloadHandler.text);

            if (www.downloadHandler != null)
            {
                ErrorRespone errorRp = JsonConvert.DeserializeObject<ErrorRespone>(www.downloadHandler.text);
                if (message != null) { message.text = errorRp.message; }
                else
                {
                    Debug.Log(errorRp.message);
                    //AnouncementManager.instance.DisplayAnouncement(errorRp.message);
                }
            }
            else
            {
                AnouncementManager.instance.DisplayAnouncement("Bad Connection!");
                Debug.LogError("Download handler is null");
            }
        }
        else
        {
            Debug.Log(www.downloadHandler.text);
            if (message != null) { message.text = "Register success"; }
            else
            {
                Debug.Log("Register success");
                AnouncementManager.instance.DisplayAnouncement("Register success");
                StartCoroutine(Login(usernameRegister.text, passwordRegister.text));
                RegisterPanel.instance.gameObject.SetActive(false);
                isCoroutineDone = true;
            }
        }
        LoadingPanel.Instance.SetDisplayLoading(true);
        
    }

    public IEnumerator SearchFriend(TMP_InputField NameSearch)
    {
        UnityWebRequest www = UnityWebRequest.Get(_apiUrl + "/findFriend/" + NameSearch.text);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            if (www.responseCode == 502)
            {
                LoadingPanel.Instance.SetDisplayLoading(true);
                yield return new WaitForSeconds(1.5f);
                yield return StartCoroutine(SearchFriend(NameSearch));
                LoadingPanel.Instance.SetDisplayLoading(false);
            }
            if (www.downloadHandler != null)
            {
                ErrorRespone errorRp = JsonConvert.DeserializeObject<ErrorRespone>(www.downloadHandler.text);
                if (message != null) { message.text = errorRp.message; }
                else { Debug.Log(errorRp.message); }
            }
            else
            {
                Debug.LogError("Download handler is null");
            }
        }
        else
        {
            List<friendSearch> friend = JsonConvert.DeserializeObject<List<friendSearch>>(www.downloadHandler.text);

            uiController.UpdateSearch(friend);
        }
    }

    public IEnumerator AddFriend(string id)
    {
        // post, endpoint: /sendFriendRequest , body: {from: "id", to: "id"}
        AddFriendRequest addFriendRq = new AddFriendRequest();
        addFriendRq.from = UserSessionManager.Instance._id;
        addFriendRq.to = id;
        string json = JsonUtility.ToJson(addFriendRq);
        Debug.Log(json);

        UnityWebRequest www = new UnityWebRequest(_apiUrl + "/sendFriendRequest", "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();

        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
            if (www.responseCode == 502)
            {
                LoadingPanel.Instance.SetDisplayLoading(true);
                yield return new WaitForSeconds(1.5f);
                yield return StartCoroutine(AddFriend(id));
                LoadingPanel.Instance.SetDisplayLoading(false);
            }
            if (www.downloadHandler != null)
            {
                ErrorRespone errorRp = JsonConvert.DeserializeObject<ErrorRespone>(www.downloadHandler.text);
                if (message != null) { message.text = errorRp.message; }
                else { Debug.Log(errorRp.message); }
            }
            else
            {
                Debug.LogError("Download handler is null");
            }
        }
        else
        {
            Debug.Log(www.downloadHandler.text);
            if (message != null) { message.text = "Send request success"; }
            else { Debug.Log("Send request success"); }
        }
        uiController.UpdateRequest();
    }
    public IEnumerator DeleteFriend(string id)
    {
        // post, endpoint: /sendFriendRequest , body: {from: "id", to: "id"}
        AddFriendRequest addFriendRq = new AddFriendRequest();
        addFriendRq.from = UserSessionManager.Instance._id;
        addFriendRq.to = id;
        string json = JsonUtility.ToJson(addFriendRq);
        Debug.Log(json);

        UnityWebRequest www = new UnityWebRequest(_apiUrl + "/removeFriend", "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();

        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
            if (www.responseCode == 502)
            {
                LoadingPanel.Instance.SetDisplayLoading(true);
                yield return new WaitForSeconds(1.5f);
                yield return StartCoroutine(DeleteFriend(id));
                LoadingPanel.Instance.SetDisplayLoading(false);
            }
            if (www.downloadHandler != null)
            {
                ErrorRespone errorRp = JsonConvert.DeserializeObject<ErrorRespone>(www.downloadHandler.text);
                if (message != null) { message.text = errorRp.message; }
                else { Debug.Log(errorRp.message); }
            }
            else
            {
                Debug.LogError("Download handler is null");
            }
        }
        else
        {
            Debug.Log(www.downloadHandler.text);
            if (message != null) { message.text = "Send request success"; }
            else { Debug.Log("Send request success"); }
        }
        uiController.UpdateFriend();
    }
    public IEnumerator AcceptFriend(request request)
    {
        // post, endpoint: /acceptFriendRequest , 
        //body:{
        //     "from": "66a614e9c9281f689ea4f36e",
        //     "to": "66a715ed13792ae24a2d447e",
        //     "status": "pending",
        //     "_id": "66a74cde0d7be3e4c13b1d75",
        //     "dateRequested": "2024-07-29T08:03:42.207Z"
        // } body la reques do

        string json = JsonUtility.ToJson(request);
        Debug.Log(json);

        UnityWebRequest www = new UnityWebRequest(_apiUrl + "/acceptFriendRequest", "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();

        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);

            if (www.downloadHandler != null)
            {
                ErrorRespone errorRp = JsonConvert.DeserializeObject<ErrorRespone>(www.downloadHandler.text);
                if (message != null) { message.text = errorRp.message; }
                else
                {
                    Debug.Log(errorRp.message);
                    AnouncementManager.instance.DisplayAnouncement(errorRp.message);
                }
            }
            else
            {
                AnouncementManager.instance.DisplayAnouncement("Bad connection!");
                Debug.LogError("Download handler is null");
            }
        }
        else
        {
            Debug.Log(www.downloadHandler.text);
            if (message != null) { message.text = "Accept request success"; }
            else
            {
                Debug.Log("Accept request success");
                AnouncementManager.instance.DisplayAnouncement("Accept request success");
            }

        }
        uiController.UpdateRequest();
        isCoroutineDone = true;
    }

    // declineFriendRequest cung giong nhu acceptFriendRequest
    public IEnumerator DeclineFriend(request request)
    {
        // post, endpoint: /declineFriendRequest , 
        //body:{
        //     "from": "66a614e9c9281f689ea4f36e",
        //     "to": "66a715ed13792ae24a2d447e",
        //     "status": "pending",
        //     "_id": "66a74cde0d7be3e4c13b1d75",
        //     "dateRequested": "2024-07-29T08:03:42.207Z"
        // } body la reques do

        string json = JsonUtility.ToJson(request);
        Debug.Log(json);

        UnityWebRequest www = new UnityWebRequest(_apiUrl + "/declineFriendRequest", "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();

        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);

            if (www.downloadHandler != null)
            {
                ErrorRespone errorRp = JsonConvert.DeserializeObject<ErrorRespone>(www.downloadHandler.text);
                if (message != null) { message.text = errorRp.message; }
                else
                {
                    Debug.Log(errorRp.message);
                    AnouncementManager.instance.DisplayAnouncement(errorRp.message);
                }
            }
            else
            {
                AnouncementManager.instance.DisplayAnouncement("Bad connection!");
                Debug.LogError("Download handler is null");
            }
        }
        else
        {
            Debug.Log(www.downloadHandler.text);
            if (message != null) { message.text = "Decline request success"; }
            else
            {
                Debug.Log("Decline request success");
                AnouncementManager.instance.DisplayAnouncement("Decline request success");
            }

        }
        uiController.UpdateRequest();
        isCoroutineDone = true;
    }

    //update list friend enpoint: /friends/:id , method: GET , id la id cua user UserSessionManager.Instance._id
    public IEnumerator getFriends()
    {
        UnityWebRequest www = UnityWebRequest.Get(_apiUrl + "/friends/" + UserSessionManager.Instance._id);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            if (www.downloadHandler != null)
            {
                ErrorRespone errorRp = JsonConvert.DeserializeObject<ErrorRespone>(www.downloadHandler.text);
                if (message != null) { message.text = errorRp.message; }
                else { Debug.Log(errorRp.message); }
            }
            else
            {
                Debug.LogError("Download handler is null");
            }
        }
        else
        {
            Debug.Log(www.downloadHandler.text);
            List<friend> friend = JsonConvert.DeserializeObject<List<friend>>(www.downloadHandler.text);
            UserSessionManager.Instance.friends = friend;
        }
    }


    //get skin by id, endpoint: /skin/:id , method: GET , id la id cua skin
    public IEnumerator getSkin(skin s1)
    {
        UnityWebRequest www = UnityWebRequest.Get(_apiUrl + "/skin/" + s1._id);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            if (www.downloadHandler != null)
            {
                ErrorRespone errorRp = JsonConvert.DeserializeObject<ErrorRespone>(www.downloadHandler.text);
                if (message != null) { message.text = errorRp.message; }
                else { Debug.Log(errorRp.message); }
            }
            else
            {
                Debug.LogError("Download handler is null");
            }
        }
        else
        {
            Debug.Log(www.downloadHandler.text);
            skin skin = JsonConvert.DeserializeObject<skin>(www.downloadHandler.text);
            //so sanh s1 va skin xem co giong nhau khong
            s1 = skin;
        }
    }



    public IEnumerator getStatus(string id)
    {
        UnityWebRequest www = UnityWebRequest.Get(_apiUrl + "/status/" + id);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            if (www.responseCode == 502)
            {
                LoadingPanel.Instance.SetDisplayLoading(true);
                yield return new WaitForSeconds(1.5f);
                yield return StartCoroutine(getStatus(id));
                LoadingPanel.Instance.SetDisplayLoading(false);
            }
            if (www.downloadHandler != null)
            {
                ErrorRespone errorRp = JsonConvert.DeserializeObject<ErrorRespone>(www.downloadHandler.text);
                if (message != null) { message.text = errorRp.message; }
                else { Debug.Log(errorRp.message); }
            }
            else
            {
                Debug.LogError("Download handler is null");
            }
        }
        else
        {
            Debug.Log(www.downloadHandler.text);
            friendIngame friend = JsonConvert.DeserializeObject<friendIngame>(www.downloadHandler.text);
            List<friendIngame> friendIn = new List<friendIngame>();
            ///check xem friend da co trong friendIngame chua
            ///neu chua co thi add vao friendIn
            if (friendIngame != null)
            {
                friendIn = friendIngame;
                if (friendIngame.Where(x => x._id == friend._id).Count() == 0)
                {
                    friendIn.Add(friend);
                }
            }
            else
            {
                friendIn.Add(friend);
            }
            friendIngame = friendIn;

            uiController.UpdateFriend();

        }
    }

    //lam moi du lieu cua user endpoint: /me/:id , method: GET , id la id cua user UserSessionManager.Instance._id
    public IEnumerator getMe()
    {
        UnityWebRequest www = UnityWebRequest.Get(_apiUrl + "/me/" + UserSessionManager.Instance._id);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            if (www.responseCode == 502)
            {
                LoadingPanel.Instance.SetDisplayLoading(true);
                yield return new WaitForSeconds(1.5f);
                yield return StartCoroutine(getMe());
                LoadingPanel.Instance.SetDisplayLoading(false);
            }
            if (www.downloadHandler != null)
            {
                Debug.Log(www.downloadHandler);
                ErrorRespone errorRp = JsonConvert.DeserializeObject<ErrorRespone>(www.downloadHandler.text);
                if (message != null) { message.text = errorRp.message; }
                else { Debug.Log(errorRp.message); }
            }
            else
            {
                Debug.LogError("Download handler is null");
            }
        }
        else
        {
            Debug.Log(www.downloadHandler.text);
            UserResponse user1 = JsonConvert.DeserializeObject<UserResponse>(www.downloadHandler.text);
            UserSessionManager.Instance.SetData(user1);
            user = user1;
            foreach (var item in user.friends)
            {
                StartCoroutine(getStatus(item._id));
            }
            UserSessionManager.Instance.SetFriendIngame(friendIngame);
            foreach (var item in user.request)
            {
                StartCoroutine(getName(item._id));
            }
        }
    }

    // get name user, enpoint /name/:id > {_id,username}
    public IEnumerator getName(string id)
    {
        UnityWebRequest www = UnityWebRequest.Get(_apiUrl + "/name/" + id);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {

            if (www.downloadHandler != null)
            {
                ErrorRespone errorRp = JsonConvert.DeserializeObject<ErrorRespone>(www.downloadHandler.text);
                if (message != null) { message.text = errorRp.message; }
                else { Debug.Log(errorRp.message); }
            }
            else
            {
                Debug.LogError("Download handler is null");
            }
        }
        else
        {
            Debug.Log(www.downloadHandler.text);
            whoRequest who = JsonConvert.DeserializeObject<whoRequest>(www.downloadHandler.text);
            List<whoRequest> wRequest1 = new List<whoRequest>();
            wRequest1.Add(who);
            wRequest = wRequest1;
        }
        uiController.UpdateRequest();
    }

    //get all skins, endpoint: /skins , method: GET

    public IEnumerator getAllSkins()
    {
        for (int i = 0; i < 2; i++)
        {
            UnityWebRequest www = UnityWebRequest.Get(_apiUrl + "/skins");
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                if (www.downloadHandler != null)
                {
                    ErrorRespone errorRp = JsonConvert.DeserializeObject<ErrorRespone>(www.downloadHandler.text);
                    if (message != null) { message.text = errorRp.message; }
                    else { Debug.Log(errorRp.message); }
                }
                else
                {
                    Debug.LogError("Download handler is null");
                }
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
                skins = JsonConvert.DeserializeObject<List<skin>>(www.downloadHandler.text);
                Debug.Log(skins.Count);
                UpdateSkinsHandle.instance.LoadPrefab();

                break;
            }

        }

    }


    public IEnumerator BuySkin(string userId, string skinId)
    {
        foreach (skinpurchase skin in user.skinpurchase)
        {
            if (skin._id == skinId)
            {
                AnouncementManager.instance.DisplayAnouncement("Skin Already Buy!");
                CheckBuySkinPanel.instance.DisplayCheck(false);
                yield break;
            }
        }
        LoadingPanel.Instance.SetDisplayLoading(true);
        // post, endpoint: /sendFriendRequest , body: {from: "id", to: "id"}
        BuySkinRequest buySkinrq = new BuySkinRequest();
        buySkinrq.userId = userId;
        buySkinrq.skinId = skinId;
        string json = JsonUtility.ToJson(buySkinrq);
        Debug.Log(json);

        UnityWebRequest www = new UnityWebRequest(_apiUrl + "/buySkin", "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();

        www.SetRequestHeader("Content-Type", "application/json");
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);

            if (www.downloadHandler != null)
            {
                ErrorRespone errorRp = JsonConvert.DeserializeObject<ErrorRespone>(www.downloadHandler.text);
                if (message != null) { message.text = errorRp.message; }
                else
                {
                    Debug.Log(errorRp.message);
                    AnouncementManager.instance.DisplayAnouncement(errorRp.message);
                }
            }
            else
            {
                Debug.LogError("Download handler is null");
            }
        }
        else
        {
            Debug.Log(www.downloadHandler.text);
            if (message != null) { message.text = "Send request success"; }
            else
            {
                Debug.Log("Send request success");
            }

            AnouncementManager.instance.DisplayAnouncement("Buying Success");
            CheckBuySkinPanel.instance.DisplayCheck(false);
        }
        LoadingPanel.Instance.SetDisplayLoading(false);
    }


}



// ------------------------------Cac class khac--------------------------




// ------------------------------Cac class model--------------------------
[System.Serializable]
public class AddFriendRequest
{
    public string from;
    public string to;
}
[System.Serializable]
public class PlayerAvatar
{
    public string userId;
    public string avatar;
}
[System.Serializable]
public class whoRequest
{
    public string _id;
    public string username;
}
[System.Serializable]
public class friendSearch
{
    public string _id;
    public string username;
    public string nameingame;
    public string avatar;
}
[System.Serializable]
public class friendIngame
{
    public string _id;
    public string username;
    public string status;
    public string avatar;
}
[System.Serializable]
public class ErrorRespone
{
    public string message;
}
[System.Serializable]
public class UserRequest
{
    public string username;
    public string password;
}
[System.Serializable]
public class UserRequestRegister
{
    public string username;
    public string password;
    public string email;
}
public class UserResponseMessage
{
    public UserResponse userResponse;
    public string message;
}
[System.Serializable]
public class UserResponse
{
    /*{
    "_id": "66aa283a7093a0984b7188e5",
    "username": "test",
    "email": "testmail",
    "password": "123123123",
    "role": "user",
    "money": 0,
    "status": "offline",
    "friends": [
      
    ],
    "request": [
      {
        "from": "66aa283a7093a0984b7188e5",
        "to": "66aa2ba97093a0984b7188ed",
        "status": "pending",
        "_id": "66aa32d06f9bf87e477dac1f",
        "dateRequested": "2024-07-31T12:49:20.715Z"
      }
    ],
    "skinpurchase": [
      
    ],
    "createdAt": "2024-07-31T12:04:10.675Z",
    "updatedAt": "2024-07-31T13:03:40.063Z",
    "__v": 1
  },
    */
    public string _id;
    public string username;
    public string email;
    public string nameingame;
    public string avatar;
    public string role;
    public int money;
    public List<friend> friends;
    public List<request> request;
    public List<skinpurchase> skinpurchase;
    public string createdAt;

}
[System.Serializable]
public class friend
{
    public string _id; // id of friend
    //public string dateAdded;
    public string username;
    public string status;
}
[System.Serializable]
public class request
{
    // {
    //     "from": "66a614e9c9281f689ea4f36e",
    //     "to": "66a715ed13792ae24a2d447e",
    //     "status": "pending",
    //     "_id": "66a74cde0d7be3e4c13b1d75",
    //     "dateRequested": "2024-07-29T08:03:42.207Z"
    // }
    public string from;
    public string to;
    public string status;
    public string _id;
    public string dateRequested;
}
[System.Serializable]
public class skinpurchase
{
    public string _id; // id of skin
    public string datePurchased;
}
[System.Serializable]
public class skin
{

    //     {
    //   "_id": "66bc57c5ea0484839ae5c2c1",
    //   "name": "thành quốc",
    //   "price": 222,
    //   "skinImage": "skins/thành quốc/LionOrange.png",
    //   "status": "available",
    //   "createdAt": "2024-08-14T07:07:49.756Z",
    //   "__v": 0
    // }
    public string _id;
    public string name;
    public int price;
    public string skinImage;
    public string status;
    public string createdAt;

}

public class userIDrequest
{
    public string type;
    public string id;
    public string username;
}

public class UserChangeNameData
{
    public string userId;
    public string nameingame;
}
public class BuySkinRequest
{
    public string userId;
    public string skinId;
}

