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
        // 서버 매칭
        Backend.Match.OnJoinMatchMakingServer = (args) =>
        {
            if(args.ErrInfo != ErrorInfo.Success)
            {
                GameManager.Instance.UIManager.ClaimError(args.ErrInfo.Category.ToString(), args.ErrInfo.Reason.ToString(), "OK");
            }
            
        };

        // 방 생성
        Backend.Match.OnMatchMakingRoomCreate = (args) =>
        {
            
            if(args.ErrInfo != ErrorCode.Success)
            {
                GameManager.Instance.UIManager.ClaimError(args.ErrInfo.ToString(), args.Reason.ToString(), "OK");
            }
            else
            {
                Debug.Log("방생성됨");
                numberOfPeopleInRoom = 1;
                LobbyScript lobby = GameObject.FindAnyObjectByType<LobbyScript>();
                lobby.OpenRoom(true);
                lobby.SetPlayerName(0, MyNickname);
                lobby.SetPlayerName(1, "");
                lobby.SetPlayerName(2, "");
                lobby.SetPlayerName(3, "");
            }
        };

        // 초대 송신
        Backend.Match.OnMatchMakingRoomInvite = (args) =>
        {
            Debug.Log("초대보냄");
            if(args.ErrInfo != ErrorCode.Success)
            {
                GameManager.Instance.UIManager.ClaimError(args.ErrInfo.ToString(), args.Reason.ToString(), "OK");
            }
        };

        // 초대 수신
        Backend.Match.OnMatchMakingRoomSomeoneInvited = (args) =>
        {
            if(args.ErrInfo != ErrorCode.Success)
            {
                GameManager.Instance.UIManager.ClaimError(args.ErrInfo.ToString(), args.Reason.ToString(), "OK");
            }
            else
            {
                Debug.Log("초대받음");
                GameManager.Instance.UIManager.ClaimInviteWindow(args.InviteUserInfo.m_nickName, args.RoomId, args.RoomToken);
            }
        };

        // 초대 수신 응답, 수락시 자동으로 대기방 입장 요청
        Backend.Match.OnMatchMakingRoomInviteResponse = (args) =>
        {
            if(args.ErrInfo != ErrorCode.Success)
            {
                GameManager.Instance.UIManager.ClaimError(args.ErrInfo.ToString(), args.Reason.ToString(), "OK");
            }
        };

        // 방 유저리스트 : 방 입장시 입장한 유저한테만 호출
        Backend.Match.OnMatchMakingRoomUserList = (args) => 
        {
            if (args.ErrInfo != ErrorCode.Success)
            {
                GameManager.Instance.UIManager.ClaimError(args.ErrInfo.ToString(), args.Reason.ToString(), "OK");
            }
            else
            {
                Debug.Log("방입장함");
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

        // 유저 입장 이벤트 : 방에 있는 모든 유저들에게(입장한 유저 포함) 호출
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

        // 대기방 퇴장 (방장도 호출 됨)
        Backend.Match.OnMatchMakingRoomLeave = (args) =>
        {
            if(args.ErrInfo != ErrorCode.Success)
            {
                GameManager.Instance.UIManager.ClaimError(args.ErrInfo.ToString(), args.Reason.ToString(), "OK");
            }
            else
            {
                Debug.Log($"{args.UserInfo.m_nickName}이/가 퇴장");
                LobbyScript lobby = GameObject.FindAnyObjectByType<LobbyScript>();
                if(args.UserInfo.m_nickName != MyNickname)
                {
                    // 남이 퇴장
                    numberOfPeopleInRoom--;
                    lobby.SetPlayerName(numberOfPeopleInRoom, "");
                }
                else
                {
                    // 내가 퇴장
                    numberOfPeopleInRoom = 0;
                    lobby.CloseRoom();
                }
            }
        };

        // 대기방 삭제 : 방장이 퇴장하면 모든 유저에게 호출 됨
        Backend.Match.OnMatchMakingRoomDestory = (args) =>
        {
            if(args.ErrInfo != ErrorCode.Success)
            {
                GameManager.Instance.UIManager.ClaimError(args.ErrInfo.ToString(), args.Reason.ToString(), "OK");
            }
            else
            {
                Debug.Log("방 삭제됨");
                numberOfPeopleInRoom = 0;
                LobbyScript lobby = GameObject.FindAnyObjectByType<LobbyScript>();
                lobby.CloseRoom();

            }
        };
    }

}
