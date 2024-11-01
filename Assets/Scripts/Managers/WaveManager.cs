using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class WaveManager : Manager
{
    [Networked] public bool IsWaveStart { get; set;}


    private List<ResourceEnum.Prefab> waveMonsterList = new List<ResourceEnum.Prefab>(); //몇 웨이브에 어떤 몬스터가.


    protected WaveInfo waveInfo;
    protected int currentWaveIndex = 0; //현재 웨이브는 몇번째 웨이브인가.
    protected List<Monster> monsterList = new List<Monster>(); // 남은 몬스터 수. 
    public int monsterCount = 0;

    protected int currentMonsterIndex = 0; //현재 웨이브에서 몇번째 몬스터인가.
    private float monsterInterval = 2; // 몬스터간의 간격(필요한가?)
    private float waveInterval = 10; // 다음 웨이브까지의 시간
    protected float nowMonsterTime = 0; // 현재 몬스터 생성 시간
    protected float nowWaveTime = 0; // 현재 웨이브 진행 시간

    protected void MonsterInstantiate()
    {
        if (GameManager.Instance.NetworkManager.Runner.IsServer)
        {
            if (waveInfo.waveOrder.Peek().Count > 0)
            {
                //int number = Random.Range((int)ResourceEnum.Prefab.Slime_Leaf, (int)ResourceEnum.Prefab.Slime_King + 1);
                int number = (int)waveInfo.waveOrder.Peek().Dequeue();
                List<Vector2> roadData = GameManager.Instance.BuildingManager.roadData;
                //Debug.Log($"roadData : {roadData.Count}");
                monsterList.Add(GameManager.Instance.NetworkManager.Runner.Spawn(ResourceManager.Get((ResourceEnum.Prefab)number), new Vector3(roadData[roadData.Count - 1].x, 0, roadData[roadData.Count - 1].y)).GetComponent<Monster>());
                monsterCount++;
                Debug.Log($"monsterCount : {monsterCount}");
            }
        }

    }

    public override IEnumerator Initiate()
    {
        //waveMonsterList.Add(ResourceEnum.Prefab.EnemyTest);
        waveInfo = new WaveInfo();
        waveInfo.Initialize();
        yield return base.Initiate();
    }

    public override void ManagerUpdate(float deltaTime)
    {
        if (GameManager.Instance.NetworkManager.Runner.IsServer && GameManager.IsGameStart)
        {
            if(IsWaveStart)
            {
                nowMonsterTime += deltaTime;
            
                if (nowMonsterTime >= monsterInterval && waveInfo.waveOrder.Peek().Count > 0) 
                {
                        MonsterInstantiate();
                        nowMonsterTime = 0;
                    Debug.Log($"{waveInfo.waveOrder.Peek().Count}마리 남음");
                }
                else if(waveInfo.waveOrder.Count > 0)
                {
                    if (nowWaveTime >= waveInterval && waveInfo.waveOrder.Peek().Count <= 0)
                    {
                        nowWaveTime = 0;
                        currentMonsterIndex = 0;
                        waveInfo.waveOrder.Dequeue();
                        Debug.Log($"{currentWaveIndex}번째 웨이브 끝");
                        currentWaveIndex++;
                    }
                    else
                    {
                        nowWaveTime += deltaTime;
                    }

                }
                else
                {
                    IsWaveStart = false;
                }

            }
            else if(waveInfo.waveOrder.Count == 0 && monsterCount == 0)
            {
                Debug.Log("클리어!");
            }
        }
    }
}
