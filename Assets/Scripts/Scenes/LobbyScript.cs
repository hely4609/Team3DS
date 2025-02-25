using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LobbyScript : MonoBehaviour
{
    //[SerializeField] GameObject room;
    //[SerializeField] GameObject[] players;
    //TextMeshProUGUI[] playerNicknames;
    //[SerializeField] GameObject inviteWindow;
    //[SerializeField] TMP_InputField nicknameWhoesToInvite;
    //[SerializeField] GameObject startBtn;
    [SerializeField] GameObject sessionIDInputFieldWindow;
    [SerializeField] TMP_InputField sessionIDInptField;
    //private void Start()
    //{
    //    playerNicknames = new TextMeshProUGUI[players.Length];
    //    for (int i=0; i<players.Length; i++)
    //    {
    //        playerNicknames[i] = players[i].GetComponentInChildren<TextMeshProUGUI>();
    //        SetPlayerName(i, "");
    //    }
    //}

    private void Update()
    {
        // Return¿Ã Enter¿”
        if ((Input.GetKeyUp(KeyCode.Return) || Input.GetKeyUp(KeyCode.KeypadEnter)) && sessionIDInptField.text != string.Empty)
        {
            JoinRoom();
            CloseSessionIDInputFieldWindow();
        }
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            CloseSessionIDInputFieldWindow();
        }
    }

    public void StartTutorial()
    {
        NetworkManager.ClaimTutorial();
    }

    
    public void StartHost()
    {
        NetworkManager.ClaimStartHost();
    }

    public void JoinRandomRoom()
    {
        NetworkManager.ClaimJoinRandomRoom();
    }

    public void JoinRoom()
    {
        NetworkManager.ClaimJoinRoom(sessionIDInptField.text);
        sessionIDInptField.text = string.Empty;
    }

    public void OpenSessionIDInputFieldWindow()
    {
        if (!sessionIDInputFieldWindow.activeSelf) OptionManager.cancelable++;
        sessionIDInputFieldWindow.SetActive(true);
        sessionIDInptField.ActivateInputField();
    }

    public void CloseSessionIDInputFieldWindow()
    {
        if (sessionIDInputFieldWindow.activeSelf) OptionManager.cancelable--;
        sessionIDInputFieldWindow.SetActive(false);
    }

    //public void SetPlayerName(int index, string name)
    //{
    //    if(name == "") playerNicknames[index].text = "<i>Click here to invite</i>";
    //    else playerNicknames[index].text = name;
    //}

}
