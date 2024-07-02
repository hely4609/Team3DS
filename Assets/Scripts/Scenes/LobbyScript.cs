using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LobbyScript : MonoBehaviour
{
    [SerializeField] GameObject room;
    [SerializeField] GameObject[] players;
    private void Start()
    {
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
        players[index].GetComponentInChildren<TextMeshProUGUI>().text = name;
    }
}
