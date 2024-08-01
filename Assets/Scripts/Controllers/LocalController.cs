using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LocalController : ControllerBase
{
    
    public override void FixedUpdateNetwork()
    {
        // KeyCode.Return�� Enter��
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (Cursor.lockState == CursorLockMode.Locked) Cursor.lockState = CursorLockMode.None;
            else Cursor.lockState = CursorLockMode.Locked;
        }

        if (GetInput(out NetworkInputData data))
        {
            OnMove(data.moveDirection);
            OnScreenRotate(data.lookRotationDelta);
            OnInteraction(data.buttons.IsSet(MyButtons.Interaction));

            if (HasStateAuthority)
            {
                //HoldingDesign();
                OnDesignBuilding(data.selectedBuildingIndex);
            }
            //if (data.buttons.IsSet(MyButtons.Build)) DoBuild();
        }
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.B)) 
        { 
            DoBuild();
        }
    }

    protected void OnMove(Vector3 direaction)
    {
        DoMove?.Invoke(direaction);
    }

    protected void OnScreenRotate(Vector2 mouseDelta)
    {
        DoScreenRotate?.Invoke(mouseDelta);
    }

    protected void OnPickUp() { }
    protected void OnPutDown() { }
    protected void OnDesignBuilding(int index) 
    {
        if (ControlledPlayer?.buildingSeletUI.activeInHierarchy == false) return;
        
        DoDesignBuilding?.Invoke(index);

        // � �ǹ��� ������ UI�� ����ش�.
        // �׸��� �� ��ư�� ������ �ű⼭ �÷��̾��� �ǹ����⸦ �õ��Ѵ�.
        // DoDesignBuilding?.Invoke(ResourceEnum.Prefab.Turret1a);
    }
    protected void OnBuild() 
    {
        DoBuild?.Invoke();
    }

    protected void OnMouseWheel(Vector2 scrollDelta)
    {
        DoMouseWheel?.Invoke(scrollDelta);
    }
    protected void OnRepair() { }
    ////////////////////////////////////////////
    protected void OnInteraction(bool isPressed) 
    {
        if (GameManager.Instance.InteractionManager.InteractionObject == null) return;

        if (isPressed)
        {
            // �÷��̾ ���� �ڱⰡ �ϰ��ִ� ��ȣ�ۿ��� ���� �˾ƾ���.
            // ������Ʈ �Լ��� ����ؼ� �������� ����
            DoInteractionStart?.Invoke(GameManager.Instance.InteractionManager.InteractionObject);
        }
        else
        {
            DoInteractionEnd?.Invoke();
        }
    }
}
