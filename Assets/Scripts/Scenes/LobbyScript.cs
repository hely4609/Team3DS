using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LobbyScript : MonoBehaviour
{
    [SerializeField] GameObject room;
    [SerializeField] GameObject[] players;
    TextMeshProUGUI[] playerNicknames;
    [SerializeField] GameObject inviteWindow;
    [SerializeField] TMP_InputField nicknameWhoesToInvite;
    [SerializeField] GameObject startBtn;
    private void Start()
    {
        playerNicknames = new TextMeshProUGUI[players.Length];
        for (int i=0; i<players.Length; i++)
        {
            playerNicknames[i] = players[i].GetComponentInChildren<TextMeshProUGUI>();
            SetPlayerName(i, "");
        }
        
        GameManager.ManagerStarts += (() =>
        {
            GameManager.Instance.UIManager.Open(UIEnum.SetNicknameCanvas);
        });
        
    }
    public void StartHost()
    {
        NetworkManager.ClaimStartHost();
    }

    public void LeaveRoom()
    {
        NetworkManager.ClaimLeaveRoom();
    }

    public void JoinRandomRoom()
    {
        NetworkManager.ClaimJoinRandomRoom();
    }

    public void JoinRoom(string roomName)
    {
        NetworkManager.ClaimJoinRoom(roomName);
    }

    public void OpenRoom(bool isHost)
    {
        room.SetActive(true);
        if (isHost) startBtn.SetActive(true);
        else startBtn.SetActive(false);
    }

    public void CloseRoom()
    {
        room.SetActive(false);
    }

    public void SetPlayerName(int index, string name)
    {
        if(name == "") playerNicknames[index].text = "<i>Click here to invite</i>";
        else playerNicknames[index].text = name;
    }

    public void Invite()
    {
        //NetworkManager.ClaimInvite(nicknameWhoesToInvite.text);
        //CloseInviteWindow();
    }

    public void OpenInviteWindow()
    {
        inviteWindow.SetActive(true);
    }

    public void CloseInviteWindow()
    {
        nicknameWhoesToInvite.text = "";
        inviteWindow.SetActive(false);
    }

    public void OnClickGameStart()
    {

    }

}
