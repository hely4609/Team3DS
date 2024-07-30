using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LocalController : ControllerBase
{
    protected void OnMove(InputValue value)
    {
        DoMove?.Invoke(value.Get<Vector3>());
    }

    protected void OnScreenRotate(InputValue value)
    {
        DoScreenRotate?.Invoke(value.Get<Vector2>());
    }

    protected void OnPickUp() { }
    protected void OnPutDown() { }
    protected void OnDesignBuilding(InputValue value) 
    {
        if (ControlledPlayer.buildingSeletUI.activeInHierarchy == false) return;

        Vector3 result = value.Get<Vector3>();

        if (result.magnitude == 0f) return;

        else if (result == Vector3.up)
        {
            DoDesignBuilding?.Invoke(0);
        }
        else if (result == Vector3.down)
        {
            DoDesignBuilding?.Invoke(1);
        }
        else if (result == Vector3.left)
        {
            DoDesignBuilding?.Invoke(2);
        }
        else if (result == Vector3.right)
        {
            DoDesignBuilding?.Invoke(3);
        }
        else if (result == Vector3.forward)
        {
            DoDesignBuilding?.Invoke(4);
        }

        // � �ǹ��� ������ UI�� ����ش�.
        // �׸��� �� ��ư�� ������ �ű⼭ �÷��̾��� �ǹ����⸦ �õ��Ѵ�.
        // DoDesignBuilding?.Invoke(ResourceEnum.Prefab.Turret1a);
    }
    protected void OnBuild() 
    {
        DoBuild?.Invoke();
    }

    protected void OnMouseWheel(InputValue value)
    {
        DoMouseWheel?.Invoke(value.Get<Vector2>());
    }
    protected void OnRepair() { }
    ////////////////////////////////////////////
    protected void OnInteraction(InputValue value) 
    {
        if (ControlledPlayer.InteractionObject == null) return;

        if (value.isPressed)
        {
            // �÷��̾ ���� �ڱⰡ �ϰ��ִ� ��ȣ�ۿ��� ���� �˾ƾ���.
            // ������Ʈ �Լ��� ����ؼ� �������� ����
            DoInteractionStart?.Invoke();
        }
        else
        {
            DoInteractionEnd?.Invoke();
        }
    }
}
