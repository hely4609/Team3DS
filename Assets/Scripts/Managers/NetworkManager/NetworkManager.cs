using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using BackEnd.Tcp;
using Photon.Pun;

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
    public NetworkPunCallBacks punCallBacks;
    NetworkState currentNetworkState = NetworkState.Offline;
    public NetworkState CurrentNetworkState => currentNetworkState;

    string gameVersion = "1";

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

        currentNetworkState = NetworkState.Initiating;

        // 나중에 while로 커넥팅 
        yield return new WaitForFunction(() =>
        {
            PhotonNetwork.PhotonServerSettings.DevRegion = "kr";
            PhotonNetwork.ConnectUsingSettings();
            if (!PhotonNetwork.IsConnected)
            {
                PhotonNetwork.GameVersion = gameVersion;
                PhotonNetwork.ConnectUsingSettings();
            }
        });
        
    }

    // Async vs Coroutine
    // Async는 주어진 task가 끝나면 다음 메소드가 시행되는 경우에 좋고
    // Coroutine은 여러 프레임에 걸쳐 메소드를 실행하는 경우에 유용하다
    // 
}
