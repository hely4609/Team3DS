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
    private void Start()
    {
        playerNicknames = new TextMeshProUGUI[players.Length];
        for (int i=0; i<players.Length; i++)
        {
            playerNicknames[i] = players[i].GetComponentInChildren<TextMeshProUGUI>();
            playerNicknames[i].text = "";
        }
        GameManager.ManagerStarts += (() =>
        {
            GameManager.Instance.UIManager.Open(UIEnum.SignInCanvas);
        });
    }
    public void MakingRoom()
    {
        NetworkManager.ClaimMakeRoom();
    }

    public void OpenRoom()
    {
        room.SetActive(true);
    }

    public void CloseRoom()
    {
        room.SetActive(false);
    }

    public void SetPlayerName(int index, string name)
    {
        playerNicknames[index].text = name;
    }

    public void Invite()
    {
        NetworkManager.ClaimInvite(nicknameWhoesToInvite.text);
        CloseInviteWindow();
    }

    public void OpenInviteWindow()
    {
        inviteWindow.SetActive(true);
    }

    public void CloseInviteWindow()
    {
        inviteWindow.SetActive(false);
    }
}
