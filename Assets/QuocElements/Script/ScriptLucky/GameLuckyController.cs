using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using System.Linq;
using Unity.VisualScripting;
using DG.Tweening;

public class GameLuckyController : NetworkBehaviour
{
    public const string MAIN_GAMEPLAY_SCENE = "NamAn";
    public List<Button> buttonsList;
    public Map standPos;
    public Transform selectedPos;
    // public TextMeshProUGUI message;
    private List<Player> playerList = new List<Player>();
    private Player selectedPlayer;
    public Transform previousPlayerStandPos;
    private Button selectedButton;
    //This is online to Lock not selectPlayer to Click button
    public bool isAllowThisPlayerClick;

    // Start is called before the first frame update
    public IEnumerator Start()
    {
        yield return new WaitUntil(() => LoadScene.Instance.isAllPlayerReady);
        // Đăng ký sự kiện click cho các button
        ButtonRegister();

        if (!IsHost) yield break;
        SetPlayersTurnToFalse();
        TeleportPlayerToStandPos();
        yield return null;
        AddPlayerHealthComponent_ClientRPC();
        AddPlayerToListSelectRandom();
        RandomlySelectPlayer();
        RandomButton();
    }
    [ClientRpc]
    private void AddPlayerHealthComponent_ClientRPC()
    {

        foreach (Player player in PlayerList.Instance.playerDic.Values)
        {
            player.AddComponent<PlayerHeath>().health = 1;
        }

    }

    private void SetPlayersTurnToFalse()
    {
        foreach (Player player in PlayerList.Instance.playerDic.Values)
        {
            player.isPlayerTurn.Value = false;
        }
    }

    private void ButtonRegister()
    {
        foreach (Button button in buttonsList)
        {
            button.onClick.AddListener(() => OnButtonClick(button));
        }
    }

    private void AddPlayerToListSelectRandom()
    {
        foreach (Player player in PlayerList.Instance.playerDic.Values)
        {
            if (player.GetComponent<PlayerHeath>() == null) return;
            if (player.GetComponent<PlayerHeath>().isDead) return;
        }
        foreach (Player player in PlayerList.Instance.playerDic.Values)
        {
            playerList.Add(player);
        }
    }

    private void TeleportPlayerToStandPos()
    {
        List<Player> players = new List<Player>();
        foreach (Player player in PlayerList.Instance.playerDic.Values)
        {
            players.Add(player);
        }
        for (int i = 0; i < players.Count; i++)
        {
            players[i].transform.position = standPos.movePos[i].transform.position;
        }
    }

    void RandomButton()
    {
        if (buttonsList.Count == 0)
        {
            Debug.Log("No buttons left to choose from.");
            return;
        }

        int randomIndex = Random.Range(0, buttonsList.Count);
        selectedButton = buttonsList[randomIndex];
        Debug.Log("Selected Button: " + selectedButton.name);
    }

    void RandomlySelectPlayer()
    {
        if (playerList.Count == 0) AddPlayerToListSelectRandom();

        // Ngẫu nhiên chọn một index từ danh sách playerList
        int randomIndex = Random.Range(0, playerList.Count);
        selectedPlayer = playerList[randomIndex];
        previousPlayerStandPos = standPos.movePos.FirstOrDefault(pos => pos.position == selectedPlayer.transform.position);

        // Hiển thị hoặc sử dụng player đã chọn theo yêu cầu của bạn
        Debug.Log("Selected Player: " + selectedPlayer.GetComponent<PlayerData>().playerName.Value);
        selectedPlayer.isPlayerTurn.Value = true;
        // Dịch chuyển player đến vị trị mới
        selectedPlayer.transform.DOJump(selectedPos.transform.position, 0.7f, 1, 0.6f);
        // Cho UI trên màn hình của người chơi được chọn có thể bấm (gọi client xuống player được chọn)
        SetIsAllowThisPlayerClick_ClientRPC(selectedPlayer.ownerClientID.Value);
        // Loại bỏ player đã chọn khỏi danh sách
        playerList.RemoveAt(randomIndex);

        // Chờ kích hoạt của Player được chọn
        //selectedPlayer.SetActive(true);
    }

    [ClientRpc]
    private void SetIsAllowThisPlayerClick_ClientRPC(ulong playerID)
    {
        if (playerID == NetworkManager.LocalClientId)
        {
            isAllowThisPlayerClick = true;
        }
        else
        {
            isAllowThisPlayerClick = false;
        }
    }

    void OnButtonClick(Button clickedButton)
    {
        if (isAllowThisPlayerClick == false) return;
        isAllowThisPlayerClick = false;
        SendButtonPlayerClick_ServerRPC(clickedButton.name);
    }
    [ServerRpc(RequireOwnership = false)]
    private void SendButtonPlayerClick_ServerRPC(string buttonName)
    {
        StartCoroutine(SendButtonPlayerClick_Coroutine(buttonName));
    }

    private IEnumerator SendButtonPlayerClick_Coroutine(string buttonName)
    {
        SendButtonPlayerClick_ClientRPC(buttonName);
        yield return new WaitForSeconds(1);

        //check if for button clicked
        if (buttonName == selectedButton.name)
        {
            //Player bị sát thương ở đoạn này
            TakeDamage_ServerRPC(selectedPlayer.GetComponent<Player>().ownerClientID.Value);
            //random lại button
            RandomButton();
            //bật tất cả các button click
            foreach (Button button in buttonsList)
            {
                button.interactable = true;
            }
        }

        ShowMessage(selectedPlayer.GetComponent<PlayerData>().playerName.Value + " clicked button: " + buttonName);
        TeleportSelectedPlayerBackToStandPos(selectedPlayer);
        yield return new WaitUntil(() => selectedPlayer.transform.position == previousPlayerStandPos.transform.position);
        yield return new WaitForSeconds(0.25f);
        RandomlySelectPlayer();
    }

    private void TeleportSelectedPlayerBackToStandPos(Player selectedPlayer)
    {
        if (previousPlayerStandPos != null)
        {
            selectedPlayer.transform.DOJump(previousPlayerStandPos.transform.position, 0.5f, 1, 0.4f);
            selectedPlayer.isPlayerTurn.Value = false;
        }
    }
    [ClientRpc]
    private void SendButtonPlayerClick_ClientRPC(string buttonName)
    {
        // tắt button đã click
        foreach (Button button in buttonsList)
        {
            if (buttonName == button.name)
            {
                button.interactable = false;
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void TakeDamage_ServerRPC(ulong clientID)
    {
        foreach (Player player in PlayerList.Instance.playerDic.Values)
        {
            if (player.ownerClientID.Value == clientID)
            {
                TakeDamage_ClientRPC(clientID);
            }

        }
    }
    [ClientRpc]
    private void TakeDamage_ClientRPC(ulong playerID)
    {
        var player = PlayerList.Instance.GetPlayerDic_Value(playerID);
        player.GetComponent<PlayerHeath>().health--;

        if (playerID == NetworkManager.Singleton.LocalClientId)
        {
            Debug.Log("Myplayer: " + NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerHeath>().health);
            if (player.GetComponent<PlayerHeath>().health == 0)
            {
                player.GetComponent<PlayerHeath>().isDead = true;
                Debug.Log("Player: " + playerID + " lose");
                CallThisPlayerIsDead_ServerRPC(playerID);
            }
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void CallThisPlayerIsDead_ServerRPC(ulong playerID)
    {
        var player = PlayerList.Instance.GetPlayerDic_Value(playerID);
        MiniEndGamePanel.instance.AddPlayerLose(player);

        if (MiniEndGamePanel.instance.playerLose.Count >= PlayerList.Instance.playerOrders.Count - 1) StartCoroutine(EndGame());
    }

    private IEnumerator EndGame()
    {
        Player playerWin = PlayerList.Instance.playerDic.First(player => !MiniEndGamePanel.instance.playerLose.Contains(player.Value)).Value != null ? PlayerList.Instance.playerDic.First(player => !MiniEndGamePanel.instance.playerLose.Contains(player.Value)).Value : null;
        Debug.Log("PlayerWin:" + playerWin.ownerClientID.Value);
        if (playerWin != null)
        {
            MiniEndGamePanel.instance.SetPlayerWin(playerWin);
            EndGameAnouncement_ClientRPC(playerWin.ownerClientID.Value);
        }
        MiniEndGamePanel.instance.playerLose.Reverse();
        yield return null;
        foreach (Player player in MiniEndGamePanel.instance.playerLose)
        {
            EndGameAnouncement_ClientRPC(player.ownerClientID.Value);
            yield return null;
        }

        RemovedComponent_ClientRPC();
        SetPlayersTurnToFalse();
        yield return new WaitForSeconds(3.5f);
        StartLoadScene_ClientRPC();
    }
    [ClientRpc]
    private void StartLoadScene_ClientRPC()
    {
        LoadScene.Instance.StartLoadSceneMultiplayer(MAIN_GAMEPLAY_SCENE, IsHost);
    }

    [ClientRpc]
    private void EndGameAnouncement_ClientRPC(ulong playerID)
    {
        MiniEndGamePanel.instance.DisplayEndMinigame(true);
        MiniEndGamePanel.instance.DisplayPlayer(PlayerList.Instance.GetPlayerDic_Value(playerID));
    }
    [ClientRpc]
    private void RemovedComponent_ClientRPC()
    {
        foreach (var player in PlayerList.Instance.playerDic)
        {
            Destroy(player.Value.GetComponent<PlayerHeath>());
        }
    }

    [ClientRpc]
    private void CallEndGame_ClientRPC(ulong playerID, bool isWin)
    {
        if (isWin) Debug.LogError("Player Win: " + playerID);
        if (!isWin) Debug.LogError("Player lose: " + playerID);
    }

    [ClientRpc]
    private void AddComponent_ClientRPC(ulong clientID)
    {
        PlayerList.Instance.GetPlayerDic_Value(clientID).AddComponent<PlayerHeath>();
        PlayerList.Instance.GetPlayerDic_Value(clientID).GetComponent<PlayerHeath>().health = 3;
    }

    void ShowMessage(string text)
    {
        Debug.Log(text);
    }
}