using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WaveManager : Manager
{
    [Networked] public bool IsWaveStart { get; set;}


    private List<ResourceEnum.Prefab> waveMonsterList = new List<ResourceEnum.Prefab>(); //�� ���̺꿡 � ���Ͱ�.


    protected WaveInfo waveInfo;
    protected int currentWaveIndex = 0; //���� ���̺�� ���° ���̺��ΰ�.
    protected List<Monster> monsterList = new List<Monster>(); // ���� ���� ��. 
    [Networked] public int MonsterCount { get; set; } = 0; // ���� �ʵ忡 �ִ� ���� ��.

    protected int currentMonsterIndex = 0; //���� ���̺꿡�� ���° �����ΰ�.
    private float monsterInterval = 2; // ���Ͱ��� ����(�ʿ��Ѱ�?)
    [Networked]private float WaveInterval { get; set; } = 10; // ���� ���̺������ �ð�
    protected float nowMonsterTime = 0; // ���� ���� ���� �ð�
    protected float nowWaveTime = 0; // ���� ���̺� ���� �ð�

    GameObject waveInfoUI;
    TextMeshProUGUI nextWaveTimeText;
    TextMeshProUGUI monsterCountText;

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
                MonsterCount++;
                Debug.Log($"monsterCount : {MonsterCount}");
            }
        }

    }

    public override IEnumerator Initiate()
    {
        //waveMonsterList.Add(ResourceEnum.Prefab.EnemyTest);
        waveInfo = new WaveInfo();
        waveInfo.Initialize();

        if(GameManager.IsGameStart) FindWaveInfoUI();

        yield return base.Initiate();
    }

    void FindWaveInfoUI()
    {
        if (GameManager.Instance.NetworkManager.Runner.LocalPlayer == GameManager.Instance.NetworkManager.LocalController.myAuthority)
        {
            waveInfoUI = GameObject.FindGameObjectWithTag("WaveInfoText");
            if(waveInfoUI != null)
            {
                nextWaveTimeText = waveInfoUI.GetComponentsInChildren<TextMeshProUGUI>()[0];
                monsterCountText = waveInfoUI.GetComponentsInChildren<TextMeshProUGUI>()[1];
                waveInfoUI.SetActive(false);
            }
        }

    }
    public void WaveStart()
    {
        IsWaveStart = true;
        if (GameManager.Instance.NetworkManager.Runner.IsServer) WaveInterval = waveInfo.waveOrder.Peek().Count * monsterInterval;
        if (waveInfo == null) FindWaveInfoUI();
        if (waveInfoUI != null) waveInfoUI.SetActive(true);
    }

    public override void ManagerUpdate(float deltaTime)
    {
        if (waveInfoUI == null) FindWaveInfoUI();
        else if(waveInfo.waveOrder.Count > 1)
        {
            nextWaveTimeText.text = $"{(int)(WaveInterval - nowWaveTime) / 60:00} : {(int)(WaveInterval - nowWaveTime) % 60:00}";
            monsterCountText.text = $"Current Monsters : {MonsterCount}";
        }
        else
        {
            nextWaveTimeText.text = "";
            monsterCountText.text = $"Current Monsters : {MonsterCount}";
        }

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
                    if (nowWaveTime >= WaveInterval)
                    {
                        nowWaveTime = 0;
                        currentMonsterIndex = 0;
                        waveInfo.waveOrder.Dequeue();
                        Debug.Log($"{currentWaveIndex}��° ���̺� ��");
                        if(waveInfo.waveOrder.Count > 0) WaveInterval = waveInfo.waveOrder.Peek().Count * monsterInterval;
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
            else if(waveInfo.waveOrder.Count == 0 && MonsterCount == 0)
            {
                Debug.Log("Ŭ����!");
            }
        }
    }
}
