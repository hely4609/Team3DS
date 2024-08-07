using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

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

        // NetworkRunner가 Shutdown 될 때 gameObject를 Destroy해서 GameManager가 들고있으면 GameManager를 삭제해버림
        // 그래서 새로운 오브젝트가 들게 함
        // 프리팹으로 callback 스크립트를 가지고있는 게임오브젝트를 만들어 놓고 인스턴시에이트함
        // 그 오브젝트에 NetworkRunner를 AddComponent함.
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
        // 컨트롤러를 만든다. 입력받기용.
        
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
