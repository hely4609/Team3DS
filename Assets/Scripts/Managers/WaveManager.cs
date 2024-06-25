using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : Manager
{
    protected WaveInfo[] allWaveInfo;
    protected int currentWaveIndex; //현재 웨이브는 몇번째 웨이브인가.
    protected int currentMonsterIndex; //현재 웨이브에서 몇번째 몬스터인가.
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
