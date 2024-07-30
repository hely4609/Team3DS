using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : Manager
{
    private List< ResourceEnum.Prefab> waveMonsterList; //�� ���̺꿡 � ���Ͱ�.
    private int monsterNumber = 3; // ������ ��


    protected WaveInfo[] allWaveInfo;
    protected int currentWaveIndex; //���� ���̺�� ���° ���̺��ΰ�.
    protected List<Monster> monsterList; // ���� ���� ��. 

    protected int currentMonsterIndex = 0; //���� ���̺꿡�� ���° �����ΰ�.
    private float monsterInterval = 5; // ���Ͱ��� ����(�ʿ��Ѱ�?)
    private float waveInterval = 30; // ���� ���̺������ �ð�
    protected float nowMonsterTime = 0; // ���� ���� ���� �ð�
    protected float nowWaveTime = 0; // ���� ���̺� ���� �ð�

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
        Debug.Log("��ȣ");
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
