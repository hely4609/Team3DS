using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Fusion;

public class CameraManager : Manager
{
    protected Player observingPlayer;
    protected Camera mainCamera;

    public override IEnumerator Initiate()
    {
        GameManager.ManagerStarts += ManagerStart;
        return base.Initiate();
    }

    // OnSceenLoad << ���� �ε� �ɶ����� ����ī�޶� ���� �޾ƿ�����Ѵ�.

    public override void ManagerStart()
    {
        mainCamera = Camera.main;
    }


    public override void ManagerUpdate(float deltaTime)
    {
        if (GameManager.Instance.NetworkManager.LocalController == null)
        {
            LocalController[] controllers = GameObject.FindObjectsByType<LocalController>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            GameManager.Instance.NetworkManager.LocalController = System.Array.Find(controllers, target => target.GetComponent<NetworkObject>().HasInputAuthority == true);
        }
        else
        {
            // �ΰ����߿��� LocalPlayer�� CameraOffset�� �Ѿư�����.
            if (observingPlayer == null)
            {
                observingPlayer = GameManager.Instance.NetworkManager.LocalController?.ControlledPlayer;
            }
            else
            {
                mainCamera.transform.position = observingPlayer.CameraOffset.position;
                mainCamera.transform.rotation = observingPlayer.CameraOffset.rotation;
            }
        }
        
    
    }

    //public override void ManagerUpdate(float deltaTime)
    //{
    //    // �ΰ����߿��� LocalPlayer�� CameraOffset�� �Ѿư�����.
    //    if (observingPlayer == null)
    //    {
    //        GameObject inst = GameObject.Find("Player(Clone)");
    //        if (inst != null)
    //        {
    //            NetworkController controller = inst.GetComponent<NetworkController>();
    //            observingPlayer = controller.ControlledPlayer;
    //        }
    //    }

    //    else
    //    {
    //        //Vector3 offset = new Vector3(observingPlayer.transform.position.x, 1.1f, observingPlayer.transform.position.z);
    //        //mainCamera.transform.SetPositionAndRotation(offset, observingPlayer.transform.rotation);

    //        mainCamera.transform.position = observingPlayer.transform.position;
    //        mainCamera.transform.rotation = observingPlayer.CameraOffset.rotation;
    //    }
    //}

    //public void CarmeraPositionUpdate()
    //{
    //    if (observingPlayer == null)
    //    {
    //        GameObject inst = GameObject.Find("PlayerPrefab(Clone)");
    //        if (inst != null)
    //        {
    //            NetworkController controller = inst.GetComponent<NetworkController>();
    //            ObservingPlayer = controller.ControlledPlayer;
    //        }
    //    }
    //    else
    //    {
    //        //mainCamera.transform.position = observingPlayer.CameraOffset.position;
    //        //mainCamera.transform.position = Vector3.zero;
    //        //mainCamera.transform.rotation = Quaternion.identity;
    //        //mainCamera.transform.parent = observingPlayer.CameraOffset;
    //    }

    //}
}
