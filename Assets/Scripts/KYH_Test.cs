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

            // NetworkRunner�� Shutdown �� �� gameObject�� Destroy�ؼ� GameManager�� ��������� GameManager�� �����ع���
            // �׷��� ���ο� ������Ʈ�� ��� �ϰ� �� ������Ʈ�� NetworkRunner�� GetComponent��.
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
