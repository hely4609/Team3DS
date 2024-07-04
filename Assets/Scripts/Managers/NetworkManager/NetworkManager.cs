using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using BackEnd.Tcp;

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
            Debug.Log("�ʱ�ȭ ����! " + bro);
            currentNetworkState = NetworkState.Connected;
            RegistCallBackFunction();
        }
        else
        {
            Debug.Log("�ʱ�ȭ ���� " + bro);
            currentNetworkState = NetworkState.Disconnected;
        }
        yield return null;
    }

    // Async vs Coroutine
    // Async�� �־��� task�� ������ ���� �޼ҵ尡 ����Ǵ� ��쿡 ����
    // Coroutine�� ���� �����ӿ� ���� �޼ҵ带 �����ϴ� ��쿡 �����ϴ�
    // 
}
