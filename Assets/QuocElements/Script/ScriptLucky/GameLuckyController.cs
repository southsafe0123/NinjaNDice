using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using System.Linq;
using Unity.VisualScripting;
using DG.Tweening;
using Unity.Services.Lobbies.Models;

public class GameLuckyController : NetworkBehaviour
{
    public const string MAIN_GAMEPLAY_SCENE = "NamAn";
    public List<Button> buttonsList;
    public List<SmokeEffect> statueShotList;
    public Map standPos;
    public Transform selectedPos;
    // public TextMeshProUGUI message;
    private List<Player> playerList = new List<Player>();
    private Player selectedPlayer;
    public Transform previousPlayerStandPos;
    private Button selectedButton;
    //This is online to Lock not selectPlayer to Click button
    public bool isAllowThisPlayerClick;

    int topPlayer = 1;

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
            player.AddComponent<PlayerHeath>().health = 2;
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
        playerList.Clear();
        foreach (Player player in PlayerList.Instance.playerDic.Values)
        {
            if (player.GetComponent<PlayerHeath>() != null|| !player.GetComponent<PlayerHeath>().isDead)
            {
                playerList.Add(player);
            }
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
        AudioManager.Instance.PlaySFXJump();
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

        yield return new WaitForSeconds(1f);

        //check if for button clicked
        if (buttonName == selectedButton.name)
        {
            //Player bị sát thương ở đoạn này
            TakeDamage_ServerRPC(selectedPlayer.GetComponent<Player>().ownerClientID.Value, buttonName);
            yield return null;
            foreach (var item in statueShotList)
            {
                if (item.gameObject.activeSelf)
                {
                    yield return new WaitUntil(() => !item.gameObject.activeSelf);
                    break;
                }
            }
            //random lại button
            RandomButton();
            //bật tất cả các button click
            ResetButton_ClientRPC();
        }

        ShowMessage(selectedPlayer.GetComponent<PlayerData>().playerName.Value + " clicked button: " + buttonName);
        TeleportSelectedPlayerBackToStandPos(selectedPlayer);
        yield return new WaitUntil(() => selectedPlayer.transform.position == previousPlayerStandPos.transform.position);
        yield return new WaitForSeconds(0.25f);
        RandomlySelectPlayer();
    }
    [ClientRpc]
    private void ResetButton_ClientRPC()
    {
        foreach (Button button in buttonsList)
        {
            button.interactable = true;
        }
    }

    private void TeleportSelectedPlayerBackToStandPos(Player selectedPlayer)
    {
        if (previousPlayerStandPos != null)
        {
            selectedPlayer.transform.DOJump(previousPlayerStandPos.transform.position, 0.5f, 1, 0.4f);
            AudioManager.Instance.PlaySFXJump();
            selectedPlayer.isPlayerTurn.Value = false;
        }
    }
    [ClientRpc]
    private void SendButtonPlayerClick_ClientRPC(string buttonName)
    {
        // tắt button đã click
        for (int i = 0; i < buttonsList.Count; i++)
        {
            if (buttonsList[i].name == buttonName)
            {
                buttonsList[i].interactable = false;
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void TakeDamage_ServerRPC(ulong clientID, string buttonName)
    {
        TakeDamage_ClientRPC(clientID, buttonName);
    }
    [ClientRpc]
    private void TakeDamage_ClientRPC(ulong playerID, string buttonName)
    {
        StartCoroutine(TakeDamage_Coroutine(playerID, buttonName));
    }

    private IEnumerator TakeDamage_Coroutine(ulong playerID, string buttonName)
    {
        var player = PlayerList.Instance.GetPlayerDic_Value(playerID);
        if (player.GetComponent<PlayerHeath>().isDead)
        {
            AudioManager.Instance.PlaySFXOutOfHealth();
            yield break;
        }
        player.GetComponent<PlayerHeath>().health--;
        AudioManager.Instance.PlaySFXTakeHp();
        for (int i = 0; i < buttonsList.Count; i++)
        {
            if (buttonsList[i].name == buttonName)
            {
                statueShotList[i].gameObject.SetActive(true);
                yield return new WaitUntil(() => !statueShotList[i].gameObject.activeSelf);
                break;
            }
        }

        player.DisplayCurrentHealth();
        if (playerID == NetworkManager.Singleton.LocalClientId)
        {
            Debug.Log("Myplayer: " + NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerHeath>().health);
            LifeRemainPanel.instance.UpdateHealth();
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
        MiniEndGamePanel.Instance.AddPlayerLose(player);

        if (MiniEndGamePanel.Instance.playerLose.Count >= PlayerList.Instance.playerOrders.Count - 1) StartCoroutine(EndGame());
    }

    private IEnumerator EndGame()
    {
        WaitForSeconds wait1f = new WaitForSeconds(1.15f);
        Player playerWin = PlayerList.Instance.playerDic.First(player => !MiniEndGamePanel.Instance.playerLose.Contains(player.Value)).Value != null ? PlayerList.Instance.playerDic.First(player => !MiniEndGamePanel.Instance.playerLose.Contains(player.Value)).Value : null;
        Debug.Log("PlayerWin:" + playerWin.ownerClientID.Value);
        if (playerWin != null)
        {
            MiniEndGamePanel.Instance.SetPlayerWin(playerWin);
            EndGameAnouncement_ClientRPC(playerWin.ownerClientID.Value, topPlayer);
            topPlayer++;
        }
        MiniEndGamePanel.Instance.playerLose.Reverse();
        yield return wait1f;
        foreach (Player player in MiniEndGamePanel.Instance.playerLose)
        {
            EndGameAnouncement_ClientRPC(player.ownerClientID.Value, topPlayer);
            topPlayer++;
            yield return wait1f;
        }

        RemovedComponent_ClientRPC();
        SetPlayersTurnToFalse();

        int i = 3;
        while (i > -1)
        {
            CallToLeave_ClientRPC(i);
            yield return wait1f;
            i--;
        }

        StartLoadScene_ClientRPC();
    }

    [ClientRpc]
    private void CallToLeave_ClientRPC(int i)
    {
        MiniEndGamePanel.Instance.SettextWaitToLeave(i.ToString());
    }
    [ClientRpc]
    private void StartLoadScene_ClientRPC()
    {
        LoadScene.Instance.StartLoadSceneMultiplayer(MAIN_GAMEPLAY_SCENE, IsHost);
    }

    [ClientRpc]
    private void EndGameAnouncement_ClientRPC(ulong playerID, int topPlayer)
    {
        MiniEndGamePanel.Instance.DisplayEndMinigame(true);
        MiniEndGamePanel.Instance.DisplayPlayer(PlayerList.Instance.GetPlayerDic_Value(playerID), topPlayer);
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