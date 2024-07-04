using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using BackEnd.Tcp;
using Photon.Pun;

// ��Ʈ��ũ �鿣�� : �ڳ� -> ����
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

        // ���߿� while�� Ŀ���� 
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
    // Async�� �־��� task�� ������ ���� �޼ҵ尡 ����Ǵ� ��쿡 ����
    // Coroutine�� ���� �����ӿ� ���� �޼ҵ带 �����ϴ� ��쿡 �����ϴ�
    // 
}
