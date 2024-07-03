using BackEnd.Tcp;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BeInvitedWindow : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI message;
    SessionId roomId;
    string roomToken;

    public void SetInviteInfo(string inviter, SessionId roomId, string roomToken)
    {
        message.text = $"You has been invited by <b><i>{inviter}</b></i>";
        this.roomId = roomId;
        this.roomToken = roomToken;
    }

    public void Accept()
    {
        NetworkManager.ClaimAcceptInvite(roomId, roomToken);
        Destroy(gameObject);
    }

    public void Reject()
    {
        NetworkManager.ClaimRejectInvite(roomId, roomToken);
        Destroy(gameObject);
    }
}
