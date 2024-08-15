using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Authentication.PlayerAccounts;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    public event Action<PlayerProfile> OnSignedIn;
    private PlayerInfo playerInfo;
    public PlayerProfile PlayerProfile;
    public TMP_InputField txtName;


    async void Awake()
    {
        try
        {
            await UnityServices.InitializeAsync();
            PlayerAccountService.Instance.SignedIn += SignInWithUnity;
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    public async void ButtonSignIn()
    {
        await InitSignIn();
        // lay gameobject kich hoat ham nay
        // var go = EventSystem.current.currentSelectedGameObject;
        // go.GetComponent<Button>().interactable = !PlayerAccountService.Instance.IsSignedIn;
    }

    public async void ButtonChangeName()
    {
        if (!PlayerAccountService.Instance.IsSignedIn)
        {
            Debug.Log("Please sign in first");
            return;
        }
        if (PlayerProfile.Name == txtName.text)
        {
            Debug.Log("Name is the same");
            return;
        }
        if (txtName.text == "" || txtName.text == null || txtName.text.Length > 50 || txtName.text.Contains(" "))
        {
            Debug.Log("Name is invalid");
            return;
        }

        await ChangePlayerName(txtName.text);
    }

    public async Task InitSignIn()
    {
        await PlayerAccountService.Instance.StartSignInAsync();
    }

    public async Task ChangePlayerName(string name)
    {
        try
        {
            await AuthenticationService.Instance.UpdatePlayerNameAsync(name);
            Debug.Log("Player name is changed to: " + name);
        }
        catch (AuthenticationException ex)
        {
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
    }

    private async void SignInWithUnity()
    {
        try
        {
            var accessToken = PlayerAccountService.Instance.AccessToken;
            await SignInWithUnityAsync(PlayerAccountService.Instance.AccessToken);
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    async Task SignInWithUnityAsync(string accessToken)
    {
        try
        {
            await AuthenticationService.Instance.SignInWithUnityAsync(accessToken);
            Debug.Log("SignIn is successful.");
            playerInfo = AuthenticationService.Instance.PlayerInfo;
            var name = await AuthenticationService.Instance.GetPlayerNameAsync();
            PlayerProfile.playerInfo = playerInfo;
            PlayerProfile.Name = name;
            Debug.Log("Player Name: " + name);
            Debug.Log("Player ID: " + playerInfo.Id);
            // txtName.text = name.Split('#')[0];
            ApiHandle.Instance.Loginid(playerInfo.Id, "Unity");
            OnSignedIn?.Invoke(PlayerProfile);

        }
        catch (AuthenticationException ex)
        {
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
    }

    // bien tai khoan anonymous thanh tai khoan Unity
    async Task LinkWithUnityAsync(string accessToken)
    {
        try
        {
            await AuthenticationService.Instance.LinkWithUnityAsync(accessToken);
            Debug.Log("Link is successful.");
        }
        catch (AuthenticationException ex) when (ex.ErrorCode == AuthenticationErrorCodes.AccountAlreadyLinked)
        {
            // Prompt the player with an error message.
            Debug.LogError("This user is already linked with another account. Log in instead.");
        }

        catch (AuthenticationException ex)
        {
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
    }


    // go lien ket tai khoan Unity
    //         async Task UnlinkUnityAsync(string idToken)
    // {
    //    try
    //    {
    //        await AuthenticationService.Instance.UnlinkUnityAsync(idToken);
    //        Debug.Log("Unlink is successful.");
    //    }
    //    catch (AuthenticationException ex)
    //    {
    //        // Compare error code to AuthenticationErrorCodes
    //        // Notify the player with the proper error message
    //        Debug.LogException(ex);
    //    }
    //    catch (RequestFailedException ex)
    //    {
    //        // Compare error code to CommonErrorCodes
    //        // Notify the player with the proper error message
    //        Debug.LogException(ex);
    //    }
    // }

    private void OnDestroy()
    {
        PlayerAccountService.Instance.SignedIn -= SignInWithUnity;
    }
}

[Serializable]
public struct PlayerProfile
{
    public PlayerInfo playerInfo;
    public string Name;
}