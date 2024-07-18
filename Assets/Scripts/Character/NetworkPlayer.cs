using Fusion;
using Fusion.Addons.SimpleKCC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NetworkPlayer : Player, IBeforeUpdate
{
    [Networked] public NetworkButtons ButtonsPrevious { get; set; }
    private NetworkCharacterController _cc;
    //private NetworkInputData _accumulatedInput;
    private Vector2Accumulator _lookRotationAccumulator = new Vector2Accumulator(0.02f, true);

    private void Awake()
    {
        _cc = GetComponent<NetworkCharacterController>();
    }

    protected override void MyUpdate(float deltaTime)
    {

    }

    

    public override void Spawned()
    {
        if (HasInputAuthority == false)
            return;

        // Register to Fusion input poll callback.
        var networkEvents = Runner.GetComponent<NetworkEvents>();
        networkEvents.OnInput.AddListener(OnInput);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        if (runner == null)
            return;

        var networkEvents = runner.GetComponent<NetworkEvents>();
        if (networkEvents != null)
        {
            networkEvents.OnInput.RemoveListener(OnInput);
        }
    }
    void IBeforeUpdate.BeforeUpdate()
    {

    }
    Vector3 lastPos;
    float velocity;
    public override void FixedUpdateNetwork()
    {
        if (HasInputAuthority == false)
            return;

        // Enter key is used for locking/unlocking cursor in game view.
        var keyboard = Keyboard.current;
        if (keyboard != null && (keyboard.enterKey.wasPressedThisFrame || keyboard.numpadEnterKey.wasPressedThisFrame))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        if (GetInput(out NetworkInputData data))
        {
            //data.direction = moveDir.normalized;
            _cc.Move(5 * data.direction * Runner.DeltaTime);

            
            // compute pressed/released state
            var pressed = data.buttons.GetPressed(ButtonsPrevious);
            var released = data.buttons.GetReleased(ButtonsPrevious);

            // store latest input as 'previous' state we had
            ButtonsPrevious = data.buttons;

            // movement (check for down)
            var vector = default(Vector3);
            if (data.buttons.IsSet(MyButtons.Forward)) { vector.z += 1; }
            if (data.buttons.IsSet(MyButtons.Backward)) { vector.z -= 1; }

            if (data.buttons.IsSet(MyButtons.Left)) { vector.x -= 1; }
            if (data.buttons.IsSet(MyButtons.Right)) { vector.x += 1; }
            Debug.Log(vector);
            DoMove(vector.normalized * 0.1f);
            

            velocity = (lastPos - transform.localPosition).magnitude;
            AnimFloat?.Invoke("Speed", velocity);

            currentDir = new Vector3(Mathf.Lerp(currentDir.x, moveDir.x, 0.1f), currentDir.y, Mathf.Lerp(currentDir.z, moveDir.z, 0.1f));

            AnimFloat?.Invoke("MoveForward", currentDir.z);
            AnimFloat?.Invoke("MoveRight", currentDir.x);

            lastPos = transform.localPosition;
        }
    }

    public void DoMove(Vector3 direction)
    {
        transform.position += direction;
    }

    public override void ScreenRotate(Vector2 mouseDelta)
    {
        var mouse = Mouse.current;
        if (mouse != null)
        {
            var lookRotationDelta = new Vector2(-mouseDelta.y, mouseDelta.x);
            lookRotationDelta *= 10 / 60f;
            _lookRotationAccumulator.Accumulate(lookRotationDelta);
        }
    }

    private void OnInput(NetworkRunner runner, NetworkInput networkInput)
    {
        // Mouse movement (delta values) is aligned to engine update.
        // To get perfectly smooth interpolated look, we need to align the mouse input with Fusion ticks.
        //_accumulatedInput.lookRotationDelta = _lookRotationAccumulator.ConsumeTickAligned(runner);

        // Fusion polls accumulated input. This callback can be executed multiple times in a row if there is a performance spike.
        //networkInput.Set(_accumulatedInput);
    }

}