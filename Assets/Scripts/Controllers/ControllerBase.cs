using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public delegate void MoveDelegate(Vector3 dir);
public delegate void ScreenRotateDelegate(Vector2 mouseDelta);
//public delegate bool DesignBuildingDelegate(BuildingEnum wantBuilding);
public delegate bool DesignBuildingDelegate(ResourceEnum.Prefab wantBuilding);
public delegate bool BuildDelegate();


public class ControllerBase : MyComponent
{
    public MoveDelegate             DoMove;
    public ScreenRotateDelegate     DoScreenRotate;
    public DesignBuildingDelegate   DoDesignBuilding;
    public BuildDelegate            DoBuild;

    protected Player controlledPlayer;
    public Player ControlledPlayer => controlledPlayer;

    protected override void OnEnable()
    {
        GameManager.ControllerStarts += MyStart;
        GameManager.ControllerUpdates += MyUpdate;
    }

    protected override void OnDisable()
    {
        GameManager.ControllerDestroies -= MyDestroy;
        GameManager.ControllerDestroies += MyDestroy;
        GameManager.ControllerUpdates -= MyUpdate;
        GameManager.ControllerStarts -= MyStart;
    }

    protected override void MyStart()
    {
        // �׽�Ʈ  
        Spawn(0, 0, 0);
    }

    public void Spawn(float dst_x, float dst_y, float dst_z)
    {
        if (controlledPlayer)
        {
            //����� �̵� ��Ű��
            controlledPlayer.transform.position = new Vector3(dst_x, dst_y, dst_z);
        }
        else //������ 
        {
            //�����!
            GameObject inst = GameManager.Instance.PoolManager.Instantiate(ResourceEnum.Prefab.Player, new Vector3(dst_x, dst_y, dst_z));
            controlledPlayer = inst.GetComponent<Player>();
            //�� ģ���� �� ���� �����̷���, ���Ǹ� �ؾ� �ؿ�!
            controlledPlayer.Possession(this);
        };
    }

    public virtual void OnUnPossessionComplete(Player target) { }
    public virtual void OnPossessionComplete(Player target) { }
    public virtual void OnPossessionFailed(Player target) { }
}
