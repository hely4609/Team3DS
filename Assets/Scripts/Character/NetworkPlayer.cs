using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayer : Player
{
    //NetworkCharacterController _ncc;
    [SerializeField] private GameObject[] buildables;
    [SerializeField] GameObject NameTag;

    protected override void Awake()
    {
        base.Awake();
        _ncc = GetComponent<NetworkCharacterController>();
    }
    [Networked] public NetworkButtons ButtonsPrevious { get; set; }

    protected override void MyStart()
    {
        Cursor.lockState = CursorLockMode.Locked;
        base.MyStart();
    }

    public override void FixedUpdateNetwork()
    {
        // KeyCode.Return이 Enter임
        if(Input.GetKeyDown(KeyCode.Return))
        {
            if(Cursor.lockState == CursorLockMode.Locked) Cursor.lockState = CursorLockMode.None;
            else Cursor.lockState = CursorLockMode.Locked;
        }
        if(Input.GetKeyDown(KeyCode.P))
        {
            if(HasStateAuthority)
            {

                NetworkObject nameTag = Runner.Spawn(NameTag);
                nameTag.transform.SetParent(transform);
                nameTag.transform.position = new Vector3(0, 1, 2);
                nameTag.GetComponent<KYH_Test>().whatyourname();
            }

        }

        if (GetInput(out NetworkInputData data))
        {
            //DoMove(data.moveDirection);
            //DoScreenRotate(data.lookRotationDelta);

            if(HasStateAuthority)
            {
                HoldingDesign();
                //if(buildingSeletUI.activeInHierarchy) DoDesignBuilding(data.selectedBuildingIndex);

                if(data.buttons.IsSet(MyButtons.Build)) DoBuild();
                if(data.buttons.IsSet(MyButtons.Interaction)) InteractionStart();
                else InteractionEnd();
            }
        }
    }

    void DoMove(Vector3 direction)
    {
        //transform.position += (transform.forward * direction.z + transform.right * direction.x).normalized * Runner.DeltaTime * moveSpeed * 10f;
        _ncc.Move(direction, moveSpeed * 10);
        AnimFloat?.Invoke("Speed", direction.magnitude);

        //currentDir = new Vector3(Mathf.Lerp(currentDir.x, moveDir.x, 0.1f), currentDir.y, Mathf.Lerp(currentDir.z, moveDir.z, 0.1f));

        AnimFloat?.Invoke("MoveForward", direction.z);
        AnimFloat?.Invoke("MoveRight", direction.x);
    }

    void DoScreenRotate(Vector2 mouseDelta)
    {
        rotate_y = transform.eulerAngles.y + mouseDelta.x * Runner.DeltaTime * 10f;
        transform.localEulerAngles = new Vector3(0f, rotate_y, 0f);

        mouseDelta_y = -mouseDelta.y * Runner.DeltaTime * 10f;
        rotate_x += mouseDelta_y;
        rotate_x = Mathf.Clamp(rotate_x, -45f, 45f);
        if (cameraOffset_FPS == null)
        {
            cameraOffset_FPS = transform.Find("CameraOffset");
        }
        cameraOffset_FPS.localEulerAngles = new Vector3(rotate_x, 0f, 0f);
    }

    NetworkObject designBuildingPrefab;
    bool DoDesignBuilding(int index)
    {
        Debug.Log("B");
        if (index < 0 || buildableEnumArray[buildableEnumPageIndex, index] == 0) return false;

        designingBuilding = Runner.Spawn(ResourceManager.Get(buildableEnumArray[buildableEnumPageIndex, index])).GetComponent<Building>();
        //designingBuilding = GameManager.Instance.PoolManager.Instantiate(buildableEnumArray[buildableEnumPageIndex, index]).GetComponent<Building>();
        buildingSeletUI.SetActive(false);
        return true;

        //if (designBuildingPrefab == null)
        //{
        //    designBuildingPrefab = Runner.Spawn(buildingPrefab, new Vector3 (transform.position.x, 0, transform.position.z) + transform.forward * 5f);
        //    designingBuilding = designBuildingPrefab.GetComponent<Building>();
        //    return true;
        //}
        //else
        //{
        //    return false;
        //}
    }
    

    void HoldingDesign()
    {
        if (designingBuilding == null) return;
        //designingBuilding = designBuildingPrefab.GetComponent<Building>();

        Vector3 pickPos = transform.position + transform.forward * 5f;
        int x = (int)pickPos.x;
        int z = (int)pickPos.z;
        designingBuilding.transform.position = new Vector3(x, designingBuilding.transform.position.y, z);
        //임시
        //designBuildingPrefab.GetComponent<NetworkTower>().SetPickPos(x, z);
        Vector2Int currentPos = new Vector2Int(x, z);

        // 건물위치에 변화가 생겼을 때 건물을 지을 수 있는 상태인지 체크함.
        if (designingBuilding.TiledBuildingPos != currentPos)
        {
            designingBuilding.TiledBuildingPos = currentPos;
            designingBuilding.CheckBuild();
        }
    }

    bool DoBuild()
    {
        Debug.Log("1");
        if (designingBuilding == null)
        {
            buildingSeletUI.SetActive(true);
            return true;
        }
        else
        {
            if (designingBuilding.FixPlace())
            {
                designingBuilding = null;
                return true;
            }
        }

        return false;



        //if (designingBuilding != null)
        //{
        //    if (designingBuilding.FixPlace())
        //    {
        //        designingBuilding = null;
        //        designBuildingPrefab = null;
        //        return true;
        //    }
        //}
        //return false;
    }



}