using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuildingEnum
{
    Tower,
    Pylon,
    Bridge
}

public abstract class Building : MonoBehaviour
{
    protected BuildingEnum type; // Ÿ�� ����
    protected bool isNeedLine; // ������ �ʿ��Ѱ�?

    protected float buildingTimeMax; // ���ۿ� �󸶳� �ɸ���
    protected float buildingTimeCurrent; // �󸶳� �����߳�
    protected float completePercent; //(0~1) ������ �ۼ�Ʈ
    public float CompletePercent {get; set; } 

    protected bool isBuildable; // �� ��ҿ� �Ǽ��� �� �ֳ�
    protected Vector2Int tiledBuildingPositionCurrent; // �Ǽ��ϰ���� ���� ��ġ. 
    protected Vector2Int tiledBuildingPositionLast; // �Ǽ��ϰ����ϴ� ������ ��ġ.

    protected Vector2Int startPos; // ���۵� ������. �ǹ��� ���� �Ʒ� ����
    protected Vector2Int size; // ������. �ǹ��� xy ũ��

    public virtual bool CheckBuild() { return default; } // buildPos�� �Ǽ��ϴ� Ÿ���� ���ʾƷ�
    public virtual bool FixPlace() { return default; } // ��ġ�� ������.
}
