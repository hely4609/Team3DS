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
            //여기로 이동 시키고
            controlledPlayer.transform.position = new Vector3(dst_x, dst_y, dst_z);
        }
        else //없으면 
        {
            //만들기!
            GameObject inst = GameManager.Instance.PoolManager.Instantiate(ResourceEnum.Prefab.Character, new Vector3(dst_x, dst_y, dst_z));
            controlledPlayer = inst.GetComponent<Player>();
            //이 친구의 손 발을 움직이려면, 빙의를 해야 해요!
            controlledPlayer.Possession(this);
        };
    }

    public virtual void OnUnPossessionComplete(Player target) { }
    public virtual void OnPossessionComplete(Player target) { }
    public virtual void OnPossessionFailed(Player target) { }
}
