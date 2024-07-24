using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayer : Player
{
    NetworkCharacterController _ncc;
    [SerializeField] private GameObject[] buildables;
    private void Awake()
    {
        _ncc = GetComponent<NetworkCharacterController>();
    }
    [Networked] public NetworkButtons ButtonsPrevious { get; set; }

    protected override void MyStart()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
    protected override void MyUpdate(float deltaTime)
    {

    }

    public override void FixedUpdateNetwork()
    {
        // KeyCode.Return�� Enter��
        if(Input.GetKeyDown(KeyCode.Return))
        {
            if(Cursor.lockState == CursorLockMode.Locked) Cursor.lockState = CursorLockMode.None;
            else Cursor.lockState = CursorLockMode.Locked;
        }

        if (GetInput(out NetworkInputData data))
        {
            DoMove(data.direction);
            DoScreenRotate(data.lookRotationDelta);

            HoldingDesign();

            if(data.buttons.IsSet(MyButtons.DesignBuilding)) DoDesignBuilding(buildables[0]);
            if(data.buttons.IsSet(MyButtons.Build)) Build();
        }
    }

    void DoMove(Vector3 direction)
    {
        //transform.position += (transform.forward * direction.z + transform.right * direction.x).normalized * Runner.DeltaTime * moveSpeed * 10f;
        //rb.velocity = direction;
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
        if (cameraOffset == null)
        {
            cameraOffset = transform.Find("CameraOffset");
        }
        cameraOffset.localEulerAngles = new Vector3(rotate_x, 0f, 0f);

    }

    bool DoDesignBuilding(GameObject buildingPrefab)
    {
        if (designingBuilding == null)
        {
            Runner.Spawn(buildingPrefab);
            designingBuilding = buildingPrefab.GetComponent<Building>();
            return true;
        }
        else return false;
    }
    

    void HoldingDesign()
    {
        if (designingBuilding == null) return;

        Vector3 pickPos = transform.position + transform.forward * 5f;
        int x = (int)pickPos.x;
        int z = (int)pickPos.z;
        designingBuilding.transform.position = new Vector3(x, designingBuilding.gameObject.transform.position.y, z);
        Vector2Int currentPos = new Vector2Int(x, z);

        // �ǹ���ġ�� ��ȭ�� ������ �� �ǹ��� ���� �� �ִ� �������� üũ��.
        if (designingBuilding.TiledBuildingPos != currentPos)
        {
            designingBuilding.TiledBuildingPos = currentPos;
            designingBuilding.CheckBuild();
            }

        }

}