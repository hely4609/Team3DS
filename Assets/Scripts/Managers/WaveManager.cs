using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class WaveManager : Manager
{
    [Networked] public bool IsWaveStart { get; set;}


    private List<ResourceEnum.Prefab> waveMonsterList = new List<ResourceEnum.Prefab>(); //�� ���̺꿡 � ���Ͱ�.


    protected WaveInfo waveInfo;
    protected int currentWaveIndex = 0; //���� ���̺�� ���° ���̺��ΰ�.
    protected List<Monster> monsterList = new List<Monster>(); // ���� ���� ��. 
    public int monsterCount = 0;

    protected int currentMonsterIndex = 0; //���� ���̺꿡�� ���° �����ΰ�.
    private float monsterInterval = 2; // ���Ͱ��� ����(�ʿ��Ѱ�?)
    private float waveInterval = 10; // ���� ���̺������ �ð�
    protected float nowMonsterTime = 0; // ���� ���� ���� �ð�
    protected float nowWaveTime = 0; // ���� ���̺� ���� �ð�

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
                    Debug.Log($"{waveInfo.waveOrder.Peek().Count}���� ����");
                }
                else if(waveInfo.waveOrder.Count > 0)
                {
                    if (nowWaveTime >= waveInterval && waveInfo.waveOrder.Peek().Count <= 0)
                    {
                        nowWaveTime = 0;
                        currentMonsterIndex = 0;
                        waveInfo.waveOrder.Dequeue();
                        Debug.Log($"{currentWaveIndex}��° ���̺� ��");
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
                Debug.Log("Ŭ����!");
            }
        }
    }
}
