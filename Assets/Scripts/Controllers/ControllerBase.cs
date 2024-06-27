using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public delegate void MoveDelegate(Vector3 dir);
public delegate void ScreenRotateDelegate(Vector2 mouseDelta);

public class ControllerBase : MonoBehaviour
{
    public MoveDelegate         DoMove;
    public ScreenRotateDelegate DoScreenRotate;

    protected Player controlledPlayer;
    public Player ControlledPlayer => controlledPlayer;

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
            GameObject inst = GameManager.Instance.PoolManager.Instantiate(ResourceEnum.Prefab.Character, new Vector3(dst_x, dst_y, dst_z));
            controlledPlayer = inst.GetComponent<Player>();
            //�� ģ���� �� ���� �����̷���, ���Ǹ� �ؾ� �ؿ�!
            controlledPlayer.Possession(this);
        };
    }

    public virtual void OnUnPossessionComplete(Player target) { }
    public virtual void OnPossessionComplete(Player target) { }
    public virtual void OnPossessionFailed(Player target) { }
}
