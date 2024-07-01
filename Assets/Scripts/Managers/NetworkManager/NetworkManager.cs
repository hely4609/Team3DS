using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using System.Threading.Tasks;

public enum NetworkStatus
{
    Offline, Initiating, Connected, SignIn,
    OnMatchingServer, OnMatchRoom, Matching,
    JoinGameServer, OnGameServer, OnGameRoom,
    WorldJoin, Disconnected
}
public partial class NetworkManager : Manager
{
    NetworkStatus currentNetworkStatus = NetworkStatus.Offline;
    public override IEnumerator Initiate()
    {
        GameManager.ClaimLoadInfo("Network Initializing");
        yield return null;

        currentNetworkStatus = NetworkStatus.Initiating;
        var bro = Backend.Initialize(true);

        if(bro.IsSuccess())
        {
            Debug.Log("�ʱ�ȭ ����! " + bro);
            currentNetworkStatus = NetworkStatus.Connected;
        }
        else
        {
            Debug.Log("�ʱ�ȭ ���� " + bro);
            currentNetworkStatus = NetworkStatus.Disconnected;
        }
        yield return null;
    }

    // Async vs Coroutine
    // Async�� �־��� task�� ������ ���� �޼ҵ尡 ����Ǵ� ��쿡 ����
    // Coroutine�� ���� �����ӿ� ���� �޼ҵ带 �����ϴ� ��쿡 �����ϴ�
    // 
}
