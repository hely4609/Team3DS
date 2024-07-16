using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NetworkPhotonCallbacks;

public class NetworkPlayer : Player
{
    private NetworkCharacterController _cc;

    private void Awake()
    {
        _cc = GetComponent<NetworkCharacterController>();
    }

    protected override void MyUpdate(float deltaTime)
    {

    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            data.direction.Normalize();
            _cc.Move(5 * data.direction * Runner.DeltaTime);
        }
        AnimFloat?.Invoke("Speed", rb.velocity.magnitude);

        currentDir = new Vector3(Mathf.Lerp(currentDir.x, moveDir.x, 0.1f), currentDir.y, Mathf.Lerp(currentDir.z, moveDir.z, 0.1f));

        AnimFloat?.Invoke("MoveForward", currentDir.z);
        AnimFloat?.Invoke("MoveRight", currentDir.x);
    }

}