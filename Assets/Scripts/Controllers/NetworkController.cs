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
//        // 어떤 건물을 지을지 UI를 띄워준다.
//        // 그리고 그 버튼을 누르면 거기서 플레이어의 건물짓기를 시도한다.
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
