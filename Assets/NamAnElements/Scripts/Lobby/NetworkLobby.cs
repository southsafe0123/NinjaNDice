using QFSW.QC;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    public Button hostButton;
    public Button JoinButton;
    private void Awake()
    {
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

        SetActiveButton(true);
        
    }
    private void SetActiveButton(bool isActive)
    {
        hostButton.enabled = isActive;
        JoinButton.enabled = isActive;
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
            Debug.Log(joinCode);
        }
        catch (RelayServiceException ex)
        {
            Debug.LogError("shit error in create relaynetwork" + ex);
        }
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
