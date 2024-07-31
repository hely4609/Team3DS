using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : Manager
{
    private List< ResourceEnum.Prefab> waveMonsterList = new List<ResourceEnum.Prefab>(); //�� ���̺꿡 � ���Ͱ�.
    private int monsterNumber = 3; // ������ ��


    protected WaveInfo[] allWaveInfo;
    protected int currentWaveIndex = 0; //���� ���̺�� ���° ���̺��ΰ�.
    protected List<Monster> monsterList = new List<Monster>(); // ���� ���� ��. 

    protected int currentMonsterIndex = 0; //���� ���̺꿡�� ���° �����ΰ�.
    private float monsterInterval = 2; // ���Ͱ��� ����(�ʿ��Ѱ�?)
    private float waveInterval = 30; // ���� ���̺������ �ð�
    protected float nowMonsterTime = 0; // ���� ���� ���� �ð�
    protected float nowWaveTime = 0; // ���� ���̺� ���� �ð�

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
