using QFSW.QC;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.UI; 

public class NetworkLobby : MonoBehaviour
{
    public static NetworkLobby Instance;
    public static TextMeshProUGUI txtRoomCode;
    public static Button joinButton;
    private void Awake()
    {
        txtRoomCode = GameObject.Find("RoomCode").GetComponentInChildren<TextMeshProUGUI>();
        joinButton = GameObject.Find("JoinButton").GetComponent<Button>();
        txtRoomCode.transform.parent.gameObject.SetActive(false);
        //if (NetworkManager.Singleton.GetComponent<UnityTransport>().Protocol == UnityTransport.ProtocolType.UnityTransport) return;
        //SetActiveButton(false);
    }

    private async void Start()
    {
        Instance = this;
        if (NetworkManager.Singleton.GetComponent<UnityTransport>().Protocol == UnityTransport.ProtocolType.UnityTransport) return;
        await UnityServices.InitializeAsync();

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("signed in: " + AuthenticationService.Instance.PlayerId);
        };
        
    }

    [Command]
    public async void CreateRelay()
    {
        switch (NetworkManager.Singleton.GetComponent<UnityTransport>().Protocol)
        {
            case UnityTransport.ProtocolType.UnityTransport:
                NetworkManager.Singleton.StartHost();
                break;
            case UnityTransport.ProtocolType.RelayUnityTransport:
                await CreateRelayOnline();
                break;
            default:
                break;
        }
        
    }

    private static async Task CreateRelayOnline()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(3);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartHost();
            CreateJoinCode(joinCode);
            DisableJoinButton();
        }
        catch (RelayServiceException ex)
        {
            Debug.LogError("shit error in create relaynetwork" + ex);
        }

        
    }

    private static void DisableJoinButton()
    {
        joinButton.interactable = false;
    }

    static void CreateJoinCode(string joinCode)
    {
        txtRoomCode.transform.parent.gameObject.SetActive(true);
        txtRoomCode.text = joinCode;
    }

    [Command]
    public async void JoinRelay(string joinCode)
    {
        switch (NetworkManager.Singleton.GetComponent<UnityTransport>().Protocol)
        {
            case UnityTransport.ProtocolType.UnityTransport:
                NetworkManager.Singleton.StartClient();
                break;
            case UnityTransport.ProtocolType.RelayUnityTransport:
                await JoinRelayOnline(joinCode);
                break;
            default:
                break;
        }
       
    }

    private static async Task JoinRelayOnline(string joinCode)
    {
        try
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartClient();
        }
        catch (RelayServiceException ex)
        {
            Debug.LogError("shit error in join relaynetwork" + ex);
        }
    }
}
