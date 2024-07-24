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

    [SerializeField] GameObject[] buildables;

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        var data = new NetworkInputData();
        data.moveDirection = moveDir;
        data.lookRotationDelta = mouseDelta;
        //data.buttons.Set(MyButtons.DesignBuilding, tryDesignBuilding);
        if(tryDesignBuilding)
        {
            _spawnedCharacters[runner.LocalPlayer].GetComponent<NetworkPlayer>().designBuildingPrefab = runner.Spawn(buildables[0]);

        }
        data.buttons.Set(MyButtons.Build, tryBuild);

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
}
