using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : Manager
{
    protected WaveInfo[] allWaveInfo;
    protected int currentWaveIndex; //���� ���̺�� ���° ���̺��ΰ�.
    protected int currentMonsterIndex; //���� ���̺꿡�� ���° �����ΰ�.
    protected float spawnCoolTime;

    public override IEnumerator Initiate()
    {
        yield return base.Initiate();
    }

    public override void ManagerUpdate(float deltaTime)
    {
        base.ManagerUpdate(deltaTime);
    }
}
