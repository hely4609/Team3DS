using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KYH_Test : MonoBehaviour
{

    [SerializeField]private GameObject _runner;
    public NetworkRunner Runner
    {
        get
        {
            if (_runner) return _runner.GetComponent<NetworkRunner>();
            else return null;
        }

    }
    NetworkState currentNetworkState = NetworkState.Offline;
    public NetworkState CurrentNetworkState => currentNetworkState;
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(2);
        GameManager.ClaimLoadInfo("Network Initializing");

            // NetworkRunner가 Shutdown 될 때 gameObject를 Destroy해서 GameManager가 들고있으면 GameManager를 삭제해버림
            // 그래서 새로운 오브젝트가 들게 하고 그 오브젝트의 NetworkRunner를 GetComponent함.
            // Create the Fusion runner and let it know that we will be providing user input
            _runner = new GameObject("NetworkRunner");
        _runner.AddComponent<NetworkRunner>();
            Runner.ProvideInput = true;

            currentNetworkState = NetworkState.Initiating;

            GameManager.NetworkUpdates += (deltaTime) => {
                if(!Runner)
                {
                    _runner = new GameObject("NetworkRunner");
        _runner.AddComponent<NetworkRunner>();
                }
            };
        
    }

}
