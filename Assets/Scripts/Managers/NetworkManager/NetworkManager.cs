using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

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
    private GameObject _runner;
    private ControllerBase _controller;
    public ControllerBase LocalController { get =>_controller; set => _controller = value; }
    public NetworkRunner Runner
    {
        get
        {
            if(_runner) return _runner.GetComponent<NetworkRunner>();
            else return null;
        }

    }
    NetworkState currentNetworkState = NetworkState.Offline;
    public NetworkState CurrentNetworkState => currentNetworkState;

    //string gameVersion = "1";

    public class UserInfo
    {
        public string gamerId;
        public string nickname;
    }

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

        // NetworkRunner�� Shutdown �� �� gameObject�� Destroy�ؼ� GameManager�� ��������� GameManager�� �����ع���
        // �׷��� ���ο� ������Ʈ�� ��� ��
        // ���������� callback ��ũ��Ʈ�� �������ִ� ���ӿ�����Ʈ�� ����� ���� �ν��Ͻÿ���Ʈ��
        // �� ������Ʈ�� NetworkRunner�� AddComponent��.
        _runner = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Photon"));
        _runner.AddComponent<NetworkRunner>();
        Runner.ProvideInput = true;

        currentNetworkState = NetworkState.Initiating;

        GameManager.NetworkUpdates += (deltaTime) => {
            if(!Runner)
            {
                _runner = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Photon"));
                _runner.AddComponent<NetworkRunner>();
            }
        };

        //_controller = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/LocalController")).GetComponent<ControllerBase>();
        // ��Ʈ�ѷ��� �����. �Է¹ޱ��.
        
        yield return null;
    }

    public IEnumerator StartGame(GameMode mode)
    {
        // Create the NetworkSceneInfo from the current scene
        var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
        var sceneInfo = new NetworkSceneInfo();
        if (scene.IsValid)
        {
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
        }

        // Start or join (depends on gamemode) a session with a specific name
        yield return new WaitForFunction(()=> Runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = "TestRoom",
            Scene = scene,
            SceneManager = GameManager.Instance.NetworkManager.Runner.AddComponent<NetworkSceneManagerDefault>()
        }));

    }
}
