using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : Manager
{
    private List< ResourceEnum.Prefab> waveMonsterList; //몇 웨이브에 어떤 몬스터가.
    private int monsterNumber = 3; // 몬스터의 수


    protected WaveInfo[] allWaveInfo;
    protected int currentWaveIndex; //현재 웨이브는 몇번째 웨이브인가.
    protected List<Monster> monsterList; // 남은 몬스터 수. 

    protected int currentMonsterIndex = 0; //현재 웨이브에서 몇번째 몬스터인가.
    private float monsterInterval = 5; // 몬스터간의 간격(필요한가?)
    private float waveInterval = 30; // 다음 웨이브까지의 시간
    protected float nowMonsterTime = 0; // 현재 몬스터 생성 시간
    protected float nowWaveTime = 0; // 현재 웨이브 진행 시간

    protected void MonsterInstantiate()
    {
        monsterList.Add(GameManager.Instance.PoolManager.Instantiate(waveMonsterList[currentWaveIndex]).GetComponent<Monster>());
    }
    public override IEnumerator Initiate()
    {
        waveMonsterList.Add(ResourceEnum.Prefab.MonsterTest);
        yield return base.Initiate();
    }
    public override void ManagerUpdate(float deltaTime)
    {
        Debug.Log("야호");
        nowMonsterTime += deltaTime;
        nowWaveTime+= deltaTime;
        if (currentMonsterIndex < monsterNumber)
        {
            if (nowMonsterTime >= monsterInterval)
            {
                MonsterInstantiate();
                currentMonsterIndex++;
                nowMonsterTime= 0;
            }
        }
        if(nowWaveTime >= waveInterval)
        {
            nowWaveTime = 0;
            currentMonsterIndex = 0;
        }
    }
}
