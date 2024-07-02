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
                GameManager.Instance.UIManager.ClaimError(args.ErrInfo.ToString(), args.ErrInfo.Summary(), "OK");
            }
            else
            {
                LobbyScript lobby = GameObject.FindAnyObjectByType<LobbyScript>();
                Debug.Log(lobby);
                lobby.SetPlayerName(0, MyNickname);
            }
        };

        // 방 유저리스트 (방 입장시 입장한 유저한테만 호출)
        Backend.Match.OnMatchMakingRoomUserList = (args) => 
        {
            Debug.Log("방입장함");
        };
    }

}
