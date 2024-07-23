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
    protected void OnDesignBuiling() 
    {
        // � �ǹ��� ������ UI�� ����ش�.
        // �׸��� �� ��ư�� ������ �ű⼭ �÷��̾��� �ǹ����⸦ �õ��Ѵ�.
        DoDesignBuilding?.Invoke(ResourceEnum.Prefab.Turret1a);
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
    protected void OnInteraction() 
    {
        DoInteraction?.Invoke(ControlledPlayer.InteractionObject);
    }

    protected void OnTest(InputValue value)
    {
        if (ControlledPlayer.InteractionObject == null) return;

        if (value.isPressed)
        {
            //Debug.Log("������");
            // �÷��̾ ���� �ڱⰡ �ϰ��ִ� ��ȣ�ۿ��� ���� �˾ƾ���.
            ControlledPlayer.InteractionObject.InteractionStart(controlledPlayer);
            ControlledPlayer.isInteracting = true;
            // ������Ʈ �Լ��� ����ؼ� �������� ����
        }
        else
        {
            //Debug.Log("�´�");
            ControlledPlayer.InteractionObject.InteractionEnd();
            ControlledPlayer.isInteracting= false;
        }    
    }
}
