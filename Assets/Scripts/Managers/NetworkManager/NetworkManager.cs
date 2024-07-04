using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using BackEnd.Tcp;

// 네트워크 백엔드 : 뒤끝 -> 포톤
public enum NetworkState
{
    Offline, Initiating, Connected, SignIn,
    OnMatchingServer, OnMatchRoom, Matching,
    JoinGameServer, OnGameServer, OnGameRoom,
    WorldJoin, Disconnected
}
public partial class NetworkManager : Manager
{
    NetworkState currentNetworkState = NetworkState.Offline;
    public NetworkState CurrentNetworkState => currentNetworkState;

    public class UserInfo
    {
        public string gamerId;
        public string nickname;
    }

    public class MatchCard
    {
        public string inDate;
        public MatchType matchType;
        public MatchModeType matchModeType;
    }

    public MatchCard[] matchCardArray;

    UserInfo myInfo;
    public string MyNickname
    {
        get => myInfo?.nickname;
        set
        {
            if (myInfo != null) return;
            else
            {
                myInfo.nickname = value;
            }
        }
    }

    

    public override IEnumerator Initiate()
    {
        GameManager.ClaimLoadInfo("Network Initializing");
        yield return null;

        currentNetworkState = NetworkState.Initiating;
        var bro = Backend.Initialize(true);

        if(bro.IsSuccess())
        {
            Debug.Log("초기화 성공! " + bro);
            currentNetworkState = NetworkState.Connected;
            RegistCallBackFunction();
        }
        else
        {
            Debug.Log("초기화 실패 " + bro);
            currentNetworkState = NetworkState.Disconnected;
        }
        yield return null;
    }

    // Async vs Coroutine
    // Async는 주어진 task가 끝나면 다음 메소드가 시행되는 경우에 좋고
    // Coroutine은 여러 프레임에 걸쳐 메소드를 실행하는 경우에 유용하다
    // 
}
