using BackEnd;
using BackEnd.Tcp;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public partial class NetworkManager : Manager
{
    void RegistCallBackFunction()
    {
        // ���� ��Ī
        Backend.Match.OnJoinMatchMakingServer = (args) =>
        {
            if(args.ErrInfo != ErrorInfo.Success)
            {
                GameManager.Instance.UIManager.ClaimError(args.ErrInfo.Category.ToString(), args.ErrInfo.Reason.ToString(), "OK");
            }
            
        };

        // �� ����
        Backend.Match.OnMatchMakingRoomCreate = (args) =>
        {
            
            Debug.Log("�������");
            if(args.ErrInfo != ErrorCode.Success)
            {
                GameManager.Instance.UIManager.ClaimError(args.ErrInfo.ToString(), args.ErrInfo.Summary(), "OK");
            }
            else
            {
                LobbyScript lobby = GameObject.FindAnyObjectByType<LobbyScript>();
                Debug.Log(lobby);
                lobby.SetPlayerName(0, MyNickname);
            }
        };

        // �� ��������Ʈ (�� ����� ������ �������׸� ȣ��)
        Backend.Match.OnMatchMakingRoomUserList = (args) => 
        {
            Debug.Log("��������");
        };
    }

}
