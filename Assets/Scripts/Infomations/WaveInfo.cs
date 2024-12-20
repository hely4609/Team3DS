using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveInfo
{
    // ���⿡ ���Ͱ� ��� ������ �������.
    // ��� �����͸� �ϴ� �� ����ΰ� �ұ�


    public Queue<Queue<ResourceEnum.Prefab>> waveOrder;  // ��ü ���̺�

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

    // ���̺� ��ųʸ��� ���͸� �ִ´�.
    // A B C ���͸� ���� 10������ �ְ����.

    // ��ȯ�� ���� ���� �迭, �� ���Ͱ� �ѹ��� ��� ������ ���� �迭, �̰��� �ݺ� ȸ��
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
