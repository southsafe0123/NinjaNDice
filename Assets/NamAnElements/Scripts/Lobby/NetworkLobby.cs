using QFSW.QC;
using System.Collections;
using System.Collections.Generic;
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
        SetActiveButton(false);
    }


    private async void Start()
    {

        await UnityServices.InitializeAsync();

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("signed in: " + AuthenticationService.Instance.PlayerId);
        };

        SetActiveButton(true);
        Instance = this;
    }
    private void SetActiveButton(bool isActive)
    {
        hostButton.enabled = isActive;
        JoinButton.enabled = isActive;
    }

    [Command]
    public async void CreateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(3);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            RelayServerData relayServerData = new RelayServerData(allocation,"dtls");
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
