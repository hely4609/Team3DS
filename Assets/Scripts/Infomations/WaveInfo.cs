using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveInfo
{
    // 여기에 몬스터가 어떻게 나올지 적어야함.
    // 모든 데이터를 일단 다 적어두고 할까


    public Queue<Queue<ResourceEnum.Prefab>> waveOrder;  // 전체 웨이브

    public void Initialize()
    {
        waveOrder = new Queue<Queue<ResourceEnum.Prefab>>();
        if(GameManager.Instance.NetworkManager.Runner.IsServer)
        {
            for (int i = 0; i < 3; i++)
            {
                MonsterQueue();
            }

        }
    }

    // 웨이브 딕셔너리에 몬스터를 넣는다.
    // A B C 몬스터를 각각 10마리씩 넣고싶음.

    // 소환할 몬스터 종류 배열, 각 몬스터가 한번에 몇마리 나올지 숫자 배열, 이것의 반복 회수
    public void AddMonsterQueue(ResourceEnum.Prefab[] monsterArray, int[] monsterNumber, int iterator)
    {
        Queue<ResourceEnum.Prefab> nextQueue = new Queue<ResourceEnum.Prefab>();
        for (int i = 0; i < iterator; i++)
        {
            for (int j = 0; j < monsterArray.Length; j++)
            {
                for (int k = 0; k < monsterNumber[j]; k++)
                {
                    nextQueue.Enqueue(monsterArray[j]);
                }
            }
        }
        waveOrder.Enqueue(nextQueue);
    }

    public void MonsterQueue()
    {

        ResourceEnum.Prefab[] a = new ResourceEnum.Prefab[] { ResourceEnum.Prefab.Slime_Leaf, ResourceEnum.Prefab.Slime_King };
        int[] b = new int[] { 1, 2 };
        AddMonsterQueue(a, b, 3);
    }

}
