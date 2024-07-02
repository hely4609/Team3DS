using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyScript : MonoBehaviour
{
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
}
