using BackEnd;
using BackEnd.Tcp;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public partial class NetworkManager : Manager
{
    public static int numberOfPeopleInRoom = 0;
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
            
            if(args.ErrInfo != ErrorCode.Success)
            {
                GameManager.Instance.UIManager.ClaimError(args.ErrInfo.ToString(), args.Reason.ToString(), "OK");
            }
            else
            {
                Debug.Log("�������");
                numberOfPeopleInRoom = 1;
                LobbyScript lobby = GameObject.FindAnyObjectByType<LobbyScript>();
                lobby.OpenRoom(true);
                lobby.SetPlayerName(0, MyNickname);
                lobby.SetPlayerName(1, "");
                lobby.SetPlayerName(2, "");
                lobby.SetPlayerName(3, "");
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
            if(args.ErrInfo != ErrorCode.Success)
            {
                GameManager.Instance.UIManager.ClaimError(args.ErrInfo.ToString(), args.Reason.ToString(), "OK");
            }
            else
            {
                Debug.Log("�ʴ����");
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
            if (args.ErrInfo != ErrorCode.Success)
            {
                GameManager.Instance.UIManager.ClaimError(args.ErrInfo.ToString(), args.Reason.ToString(), "OK");
            }
            else
            {
                Debug.Log("��������");
                numberOfPeopleInRoom = args.UserInfos.Count;
                LobbyScript lobby = GameObject.FindAnyObjectByType<LobbyScript>();
                lobby.OpenRoom(false);
                for (int i=0; i<4; i++)
                {
                    if (i < args.UserInfos.Count) lobby.SetPlayerName(i, args.UserInfos[i].m_nickName);
                    else lobby.SetPlayerName(i, "");
                }
            }
        };

        // ���� ���� �̺�Ʈ : �濡 �ִ� ��� �����鿡��(������ ���� ����) ȣ��
        Backend.Match.OnMatchMakingRoomJoin = (args) =>
        {
            if (args.ErrInfo != ErrorCode.Success)
            {
                GameManager.Instance.UIManager.ClaimError(args.ErrInfo.ToString(), args.Reason.ToString(), "OK");
            }
            else
            {
                if (args.UserInfo.m_nickName != MyNickname) 
                {
                    numberOfPeopleInRoom++;
                    LobbyScript lobby = GameObject.FindAnyObjectByType<LobbyScript>();
                    lobby.SetPlayerName(numberOfPeopleInRoom - 1, args.UserInfo.m_nickName);
                }
            }
        };

        // ���� ���� (���嵵 ȣ�� ��)
        Backend.Match.OnMatchMakingRoomLeave = (args) =>
        {
            if(args.ErrInfo != ErrorCode.Success)
            {
                GameManager.Instance.UIManager.ClaimError(args.ErrInfo.ToString(), args.Reason.ToString(), "OK");
            }
            else
            {
                Debug.Log($"{args.UserInfo.m_nickName}��/�� ����");
                LobbyScript lobby = GameObject.FindAnyObjectByType<LobbyScript>();
                if(args.UserInfo.m_nickName != MyNickname)
                {
                    // ���� ����
                    numberOfPeopleInRoom--;
                    lobby.SetPlayerName(numberOfPeopleInRoom, "");
                }
                else
                {
                    // ���� ����
                    numberOfPeopleInRoom = 0;
                    lobby.CloseRoom();
                }
            }
        };

        // ���� ���� : ������ �����ϸ� ��� �������� ȣ�� ��
        Backend.Match.OnMatchMakingRoomDestory = (args) =>
        {
            if(args.ErrInfo != ErrorCode.Success)
            {
                GameManager.Instance.UIManager.ClaimError(args.ErrInfo.ToString(), args.Reason.ToString(), "OK");
            }
            else
            {
                Debug.Log("�� ������");
                numberOfPeopleInRoom = 0;
                LobbyScript lobby = GameObject.FindAnyObjectByType<LobbyScript>();
                lobby.CloseRoom();

            }
        };
    }

}
