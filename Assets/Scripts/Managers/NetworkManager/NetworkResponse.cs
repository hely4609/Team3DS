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
            
            Debug.Log("방생성됨");
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
            Debug.Log("초대받음");
            if(args.ErrInfo != ErrorCode.Success)
            {
                GameManager.Instance.UIManager.ClaimError(args.ErrInfo.ToString(), args.Reason.ToString(), "OK");
            }
            else
            {
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
            Debug.Log("방입장함");
        };

        // 유저 입장 이벤트 : 방에 있는 모든 유저들에게(입장한 유저 포함) 호출
    }

}
