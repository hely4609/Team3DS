//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.InputSystem;

//public class NetworkController : ControllerBase
//{
//    protected override void MyStart()
//    {
//        controlledPlayer = GetComponent<NetworkPlayer>();
//        controlledPlayer.Possession(this);
//    }
//    protected void OnMove(InputValue value)
//    {
//        DoMove?.Invoke(value.Get<Vector3>());
//    }

//    protected void OnScreenRotate(InputValue value)
//    {
//        DoScreenRotate?.Invoke(value.Get<Vector2>());
//    }

//    protected void OnPickUp() { }
//    protected void OnPutDown() { }
//    protected void OnDesignBuiling()
//    {
//        // � �ǹ��� ������ UI�� ����ش�.
//        // �׸��� �� ��ư�� ������ �ű⼭ �÷��̾��� �ǹ����⸦ �õ��Ѵ�.
//        // DoDesignBuilding?.Invoke(ResourceEnum.Prefab.Tower);
//    }
//    protected void OnBuild()
//    {
//        DoBuild?.Invoke();
//    }
//    protected void OnRepair() { }
//    ////////////////////////////////////////////
//    //protected void OnInteract() { }
//}
