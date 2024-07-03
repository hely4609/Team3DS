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
                GameManager.Instance.UIManager.ClaimError(args.ErrInfo.ToString(), args.Reason.ToString(), "OK");
            }
            else
            {
                LobbyScript lobby = GameObject.FindAnyObjectByType<LobbyScript>();
                Debug.Log(lobby);
                lobby.SetPlayerName(0, MyNickname);
            }
        };

        // �ʴ� �۽�
        Backend.Match.OnMatchMakingRoomInvite = (args) =>
        {
            Debug.Log("�ʴ뺸��");
            if(args.ErrInfo != ErrorCode.Success)
            {
                GameManager.Instance.UIManager.ClaimError(args.ErrInfo.ToString(), args.Reason.ToString(), "OK");
            }
        };

        // �ʴ� ����
        Backend.Match.OnMatchMakingRoomSomeoneInvited = (args) =>
        {
            Debug.Log("�ʴ����");
            if(args.ErrInfo != ErrorCode.Success)
            {
                GameManager.Instance.UIManager.ClaimError(args.ErrInfo.ToString(), args.Reason.ToString(), "OK");
            }
            else
            {
                GameManager.Instance.UIManager.ClaimInviteWindow(args.InviteUserInfo.m_nickName, args.RoomId, args.RoomToken);
            }
        };

        // �ʴ� ���� ����, ������ �ڵ����� ���� ���� ��û
        Backend.Match.OnMatchMakingRoomInviteResponse = (args) =>
        {
            if(args.ErrInfo != ErrorCode.Success)
            {
                GameManager.Instance.UIManager.ClaimError(args.ErrInfo.ToString(), args.Reason.ToString(), "OK");
            }
        };

        // �� ��������Ʈ : �� ����� ������ �������׸� ȣ��
        Backend.Match.OnMatchMakingRoomUserList = (args) => 
        {
            Debug.Log("��������");
        };

        // ���� ���� �̺�Ʈ : �濡 �ִ� ��� �����鿡��(������ ���� ����) ȣ��
    }

}
