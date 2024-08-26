using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum MyButtons
{
    DesignBuilding = 0,
    Build = 1,
    Cancel= 2,
    Interaction = 3,
}
public struct NetworkInputData : INetworkInput
{
    public Vector3 currentPosition;
    public Quaternion currentRotation;

    public Vector3 moveDirection;
    //public Vector2 lookRotationDelta;
    public Vector2 scrollbarDelta;
    public int selectedBuildingIndex;
    public NetworkButtons buttons;
}

public partial class NetworkPhotonCallbacks
{
    Vector3 moveDir;
    //Vector2 mouseDelta;
    Vector2 mouseWheelDelta;
    int buildingIndex = -1;
    bool tryBuild;
    bool tryCancel;
    bool tryInteraction;

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        if (!GameManager.IsGameStart) return;

        var data = new NetworkInputData();
        Player inputPlayer = GameManager.Instance.NetworkManager.LocalController?.ControlledPlayer;
        if (inputPlayer != null)
        {
            data.currentPosition = inputPlayer.transform.position;
            data.currentRotation = inputPlayer.transform.rotation;
        }

        data.moveDirection = moveDir;
        //data.lookRotationDelta = mouseDelta;
        data.selectedBuildingIndex = buildingIndex;
        data.scrollbarDelta = mouseWheelDelta;

        data.buttons.Set(MyButtons.Build, tryBuild);
        data.buttons.Set(MyButtons.Cancel, tryCancel);
        data.buttons.Set(MyButtons.Interaction, tryInteraction);

        input.Set(data);

        buildingIndex = -1;
        tryBuild = false;
        tryCancel = false;
    }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }

    public void OnMove(InputValue value)
    {
        moveDir = value.Get<Vector3>();
    }
    //public void OnScreenRotate(InputValue value)
    //{
    //    mouseDelta = value.Get<Vector2>();
    //}

    public void OnDesignBuilding(InputValue value)
    {
        Vector3 result = value.Get<Vector3>();

        if (result.magnitude == 0f) return;

        else if (result == Vector3.up)
        {
            buildingIndex = 0;
        }
        else if (result == Vector3.down)
        {
            buildingIndex = 1;
        }
        else if (result == Vector3.left)
        {
            buildingIndex = 2;
        }
        else if (result == Vector3.right)
        {
            buildingIndex = 3;
        }
        else if (result == Vector3.forward)
        {
            buildingIndex = 4;
        }
    }

    public void OnBuild()
    {
        tryBuild = true;
    }
    public void OnCancel()
    {
        tryCancel = true;
    }

    public void OnInteraction(InputValue value)
    {
        tryInteraction = value.isPressed;
    }
    public void OnMouseWheel(InputValue value)
    {
        mouseWheelDelta = value.Get<Vector2>();
    }
}
