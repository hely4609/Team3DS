using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : Manager
{
    private List< ResourceEnum.Prefab> waveMonsterList = new List<ResourceEnum.Prefab>(); //몇 웨이브에 어떤 몬스터가.
    private int monsterNumber = 3; // 몬스터의 수


    protected WaveInfo[] allWaveInfo;
    protected int currentWaveIndex = 0; //현재 웨이브는 몇번째 웨이브인가.
    protected List<Monster> monsterList = new List<Monster>(); // 남은 몬스터 수. 

    protected int currentMonsterIndex = 0; //현재 웨이브에서 몇번째 몬스터인가.
    private float monsterInterval = 2; // 몬스터간의 간격(필요한가?)
    private float waveInterval = 30; // 다음 웨이브까지의 시간
    protected float nowMonsterTime = 0; // 현재 몬스터 생성 시간
    protected float nowWaveTime = 0; // 현재 웨이브 진행 시간

    protected void MonsterInstantiate()
    {
        //if(GameManager.Instance.NetworkManager.Runner.)
        List<Vector2> roadData = GameManager.Instance.BuildingManager.roadData;
        
        //monsterList.Add(GameManager.Instance.NetworkManager.Runner.Spawn(ResourceManager.Get(ResourceEnum.Prefab.EnemyTest), new Vector3(roadData[roadData.Count-1].x, 2.5f, roadData[roadData.Count-1].y)).GetComponent<Monster>());
    }
    public override IEnumerator Initiate()
    {
        waveMonsterList.Add(ResourceEnum.Prefab.EnemyTest);
        yield return base.Initiate();
    }
    public override void ManagerUpdate(float deltaTime)
    {
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
