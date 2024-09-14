using UnityEngine;

public static class PrefsData
{
    public const string PLAYER_INGAME_NAME_NOLOGIN = "GuestName";
    public const string PLAYER_USERNAME_LOGIN = "username";
    public const string PLAYER_PASSWORD_LOGIN = "password";
    public const string PLAYER_ID_UNITY_LOGIN = "unityId";
    public const string PLAYER_SKIN_ID = "skinID";

    public static string GetData(string key)
    {
        return PlayerPrefs.HasKey(key)? PlayerPrefs.GetString(key):null;
    }

    public static bool HaveData(string key)
    {
        return PlayerPrefs.HasKey(key);
    }

    /// <summary>
    /// need to register const in PrefsData first to avoid forgot okay?
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public static void SetData(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
    }

    public static void DeleteData(string key)
    {
        if (PlayerPrefs.HasKey(key))
        {
            PlayerPrefs.DeleteKey(key);
        }
    }
}