using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using TMPro;


[System.Serializable]
public class User
{
    public string username;
    public string password;
    public string role;
    public int money;
    public bool activeState;
    public List<Friend> friends;
    public List<SkinPurchased> skinPurchased;


    public User()
    {
    }

    public User(string username, string password, string role, int money, bool activeState, List<Friend> friends, List<SkinPurchased> skinPurchased)
    {
        this.username = username;
        this.password = password;
        this.role = role;
        this.money = money;
        this.activeState = activeState;
        this.friends = friends;
        this.skinPurchased = skinPurchased;
    }

    public User(string username, string password, string role, int money, bool activeState)
    {
        this.username = username;
        this.password = password;
        this.role = role;
        this.money = money;
        this.activeState = activeState;
        this.friends = new List<Friend>();
        this.skinPurchased = new List<SkinPurchased>();
    }

    public User(string username, string password, string role, int money)
    {
        this.username = username;
        this.password = password;
        this.role = role;
        this.money = money;
        this.activeState = true;
        this.friends = new List<Friend>();
        this.skinPurchased = new List<SkinPurchased>();
    }

    public User(string username, string password, string role)
    {
        this.username = username;
        this.password = password;
        this.role = role;
        this.money = 0;
        this.activeState = true;
        this.friends = new List<Friend>();
        this.skinPurchased = new List<SkinPurchased>();
    }

}

[System.Serializable]
public class Friend
{
    public string userID;
    public string friendSince;


    public Friend()
    {
    }

    public Friend(string userID, string friendSince)
    {
        this.userID = userID;
        this.friendSince = friendSince;
    }

    public Friend(string userID)
    {
        this.userID = userID;
        this.friendSince = System.DateTime.Now.ToString();
    }
}

[System.Serializable]
public class SkinPurchased
{
    public string skinID;
    public string purchaseDate;

    public SkinPurchased()
    {
    }

    public SkinPurchased(string skinID, string purchaseDate)
    {
        this.skinID = skinID;
        this.purchaseDate = purchaseDate;
    }

    public SkinPurchased(string skinID)
    {
        this.skinID = skinID;
        this.purchaseDate = System.DateTime.Now.ToString();
    }
}

public class FirebaseManagerX : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public string role = "user";
    private DatabaseReference databaseReference;
    void Start()
    {
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void test()
    {
        GetFriends(usernameInput.text);
    }
    public void CreateUser(string username, string password, string role)
    {
        User user = new User(username, password, role);
        string json = JsonUtility.ToJson(user);
        databaseReference.Child("users").Child(username).SetRawJsonValueAsync(json);

    }

    public void ReadUser(string userId)
    {
        FirebaseDatabase.DefaultInstance
          .GetReference("users").Child(userId)
          .GetValueAsync().ContinueWith(task =>
          {
              if (task.IsFaulted)
              {
                  // Handle the error...
              }
              else if (task.IsCompleted)
              {
                  DataSnapshot snapshot = task.Result;
                  // Do something with snapshot...
              }
          });
    }

    public void UpdateUser(string username, string password, string role, int money, bool activeState, List<Friend> friends, List<SkinPurchased> skinPurchased)
    {
        User user = new User(username, password, role, money, activeState, friends, skinPurchased);
        string json = JsonUtility.ToJson(user);
        databaseReference.Child("users").Child(username).SetRawJsonValueAsync(json);
    }

    public void DeleteUser(string username)
    {
        databaseReference.Child("users").Child(username).RemoveValueAsync();
    }

    public void AddFriend(string username, string friendID)
    {
        Friend friend = new Friend(friendID);
        string json = JsonUtility.ToJson(friend);
        databaseReference.Child("users").Child(username).Child("friends").Child(friendID).SetRawJsonValueAsync(json);
    }

    public void RemoveFriend(string username, string friendID)
    {
        databaseReference.Child("users").Child(username).Child("friends").Child(friendID).RemoveValueAsync();
    }

    public void AddSkinPurchased(string username, string skinID)
    {
        SkinPurchased skinPurchased = new SkinPurchased(skinID);
        string json = JsonUtility.ToJson(skinPurchased);
        databaseReference.Child("users").Child(username).Child("skinPurchased").Child(skinID).SetRawJsonValueAsync(json);
    }

    public void RemoveSkinPurchased(string username, string skinID)
    {
        databaseReference.Child("users").Child(username).Child("skinPurchased").Child(skinID).RemoveValueAsync();
    }

    public void UpdateMoney(string username, int money)
    {
        databaseReference.Child("users").Child(username).Child("money").SetValueAsync(money);
    }

    public void UpdateActiveState(string username, bool activeState)
    {
        databaseReference.Child("users").Child(username).Child("activeState").SetValueAsync(activeState);
    }

    public void Login(string username, string password)
    {
        FirebaseDatabase.DefaultInstance
      .GetReference("users").Child(username)
      .GetValueAsync().ContinueWith(task =>
      {
          if (task.IsFaulted)
          {
              // Handle the error...
              Debug.Log("Error");
          }
          else if (task.IsCompleted)
          {
              DataSnapshot snapshot = task.Result;
              User user = JsonUtility.FromJson<User>(snapshot.GetRawJsonValue());
              if (user.password == password)
              {
                  Debug.Log("Login success");
              }
              else
              {
                  Debug.Log("Login failed");
              }
          }
      });
    }

    public void Register(string username, string password, string role)
    {
        FirebaseDatabase.DefaultInstance
      .GetReference("users").Child(username)
      .GetValueAsync().ContinueWith(task =>
      {
          if (task.IsFaulted)
          {
              // Handle the error...
              Debug.Log("Error");
          }
          else if (task.IsCompleted)
          {
              DataSnapshot snapshot = task.Result;
              if (snapshot.Exists)
              {
                  Debug.Log("Username already exists");
              }
              else
              {
                  CreateUser(username, password, role);
                  Debug.Log("Register success");
              }
          }
      });
    }

    public void GetMoney(string username)
    {
        FirebaseDatabase.DefaultInstance
      .GetReference("users").Child(username).Child("money")
      .GetValueAsync().ContinueWith(task =>
      {
          if (task.IsFaulted)
          {
              // Handle the error...
          }
          else if (task.IsCompleted)
          {
              DataSnapshot snapshot = task.Result;
              Debug.Log(snapshot.GetRawJsonValue());
          }
      });
    }

    public void GetActiveState(string username)
    {
        FirebaseDatabase.DefaultInstance
      .GetReference("users").Child(username).Child("activeState")
      .GetValueAsync().ContinueWith(task =>
      {
          if (task.IsFaulted)
          {
              // Handle the error...
          }
          else if (task.IsCompleted)
          {
              DataSnapshot snapshot = task.Result;
              Debug.Log(snapshot.GetRawJsonValue());
          }
      });
    }

    public void GetFriends(string username)
    {
        FirebaseDatabase.DefaultInstance
      .GetReference("users").Child(username).Child("friends")
      .GetValueAsync().ContinueWith(task =>
      {
          if (task.IsFaulted)
          {
              // Handle the error...
          }
          else if (task.IsCompleted)
          {
              DataSnapshot snapshot = task.Result;
              Debug.Log(snapshot.GetRawJsonValue());
          }
      });
    }

    public void GetFriendsOnline(string username)
    {
        FirebaseDatabase.DefaultInstance
      .GetReference("users").Child(username).Child("friends")
      .GetValueAsync().ContinueWith(task =>
      {
          if (task.IsFaulted)
          {
              // Handle the error...
          }
          else if (task.IsCompleted)
          {
              DataSnapshot snapshot = task.Result;
              foreach (DataSnapshot friendSnapshot in snapshot.Children)
              {
                  FirebaseDatabase.DefaultInstance
                  .GetReference("users").Child(friendSnapshot.Child("userID").Value.ToString()).Child("activeState")
                  .GetValueAsync().ContinueWith(task2 =>
                  {
                      if (task2.IsFaulted)
                      {
                          // Handle the error...
                      }
                      else if (task2.IsCompleted)
                      {
                          DataSnapshot snapshot2 = task2.Result;
                          if (snapshot2.GetRawJsonValue() == "true")
                          {
                              Debug.Log(friendSnapshot.Child("userID").Value.ToString() + " is online");
                          }
                          else
                          {
                              Debug.Log(friendSnapshot.Child("userID").Value.ToString() + " is offline");
                          }
                      }
                  });
              }
          }
      });
    }

    public void GetSkinPurchased(string username)
    {
        FirebaseDatabase.DefaultInstance
      .GetReference("users").Child(username).Child("skinPurchased")
      .GetValueAsync().ContinueWith(task =>
      {
          if (task.IsFaulted)
          {
              // Handle the error...
          }
          else if (task.IsCompleted)
          {
              DataSnapshot snapshot = task.Result;
              Debug.Log(snapshot.GetRawJsonValue());
          }
      });
    }



}
