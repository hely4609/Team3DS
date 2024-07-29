using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public partial class NetworkPhotonCallbacks
{
    Vector3 moveDir;
    Vector2 mouseDelta;
    bool tryDesignBuilding, tryBuild, cancelDesignBuilding;
    bool tryInteraction;

    [SerializeField] GameObject[] buildables;

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        if (!GameManager.IsGameStart) return;

        var data = new NetworkInputData();
        data.moveDirection = moveDir;
        data.lookRotationDelta = mouseDelta;
        data.buttons.Set(MyButtons.DesignBuilding, tryDesignBuilding);
        data.buttons.Set(MyButtons.Build, tryBuild);
        data.buttons.Set(MyButtons.Interaction, tryInteraction);

        input.Set(data);

        tryDesignBuilding = false;
        tryBuild = false;
    }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnMove(InputValue value)
    {
        moveDir = value.Get<Vector3>();
    }
    public void OnScreenRotate(InputValue value)
    {
        mouseDelta = value.Get<Vector2>();
    }

    public void OnDesignBuilding()
    {
        tryDesignBuilding = true;
    }

    public void OnBuild()
    {
        tryBuild = true;
    }

    public void OnInteraction(InputValue value)
    {
        tryInteraction = value.isPressed;
    }
}
