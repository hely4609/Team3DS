using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WaveManager : Manager
{
    private List<ResourceEnum.Prefab> waveMonsterList = new List<ResourceEnum.Prefab>(); //�� ���̺꿡 � ���Ͱ�.

    protected WaveInfo waveInfo;
    protected int currentWaveIndex = 0; //���� ���̺�� ���° ���̺��ΰ�.
    protected List<Monster> monsterList = new List<Monster>(); // ���� ���� ��. 

    protected int currentMonsterIndex = 0; //���� ���̺꿡�� ���° �����ΰ�.
    private float monsterInterval = 2; // ���Ͱ��� ����(�ʿ��Ѱ�?)
    protected float nowMonsterTime = 0; // ���� ���� ���� �ð�
    protected float nowWaveTime = 0; // ���� ���̺� ���� �ð�
    protected int spawnLoc = 4; // ���̺� ���� ��ġ. ���� ���Ƿ� �����.

    public int SpawnLoc { get { return spawnLoc; } protected set { spawnLoc = value; } }
    List<Vector2> roadData;

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
                monsterList.Add(GameManager.Instance.NetworkManager.Runner.Spawn(ResourceManager.Get((ResourceEnum.Prefab)number), new Vector3(roadData[spawnLoc].x, 0, roadData[spawnLoc].y)).GetComponent<Monster>());
                GameManager.Instance.BuildingManager.generator.MonsterCount++;
                Debug.Log($"monsterCount : {GameManager.Instance.BuildingManager.generator.MonsterCount}");
            }
        }

    }

    public override IEnumerator Initiate()
    {
        //waveMonsterList.Add(ResourceEnum.Prefab.EnemyTest);
        waveInfo = new WaveInfo();
        waveInfo.Initialize();

        roadData = GameManager.Instance.BuildingManager.roadData;

        if (GameManager.Instance.NetworkManager.Runner.LocalPlayer == GameManager.Instance.NetworkManager.LocalController.myAuthority && GameManager.Instance.BuildingManager.generator.IsWaveStart) WaveStart();


        yield return base.Initiate();
    }

    void FindWaveInfoUI()
    {
        Debug.Log($"{GameManager.Instance.NetworkManager.Runner.LocalPlayer}, {GameManager.Instance.NetworkManager.LocalController.myAuthority}");
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
        if (GameManager.Instance.NetworkManager.Runner.IsServer)
        {
            GameManager.Instance.BuildingManager.generator.IsWaveStart = true;
            GameManager.Instance.BuildingManager.generator.WaveInterval = waveInfo.waveOrder.Peek().Count * monsterInterval;
        }
        if (waveInfo == null) FindWaveInfoUI();
        if (GameManager.Instance.NetworkManager.Runner.LocalPlayer == GameManager.Instance.NetworkManager.LocalController.myAuthority)
        {
            if (waveInfoUI != null) waveInfoUI.SetActive(true);
        }
    }

    public override void ManagerUpdate(float deltaTime)
    {
        if (waveInfoUI == null) FindWaveInfoUI();
        else if(waveInfo.waveOrder.Count > 1)
        {
            nextWaveTimeText.text = $"{(int)(GameManager.Instance.BuildingManager.generator.WaveInterval - nowWaveTime) / 60:00} : {(int)(GameManager.Instance.BuildingManager.generator.WaveInterval - nowWaveTime) % 60:00}";
            monsterCountText.text = $"Current Monsters : {GameManager.Instance.BuildingManager.generator.MonsterCount}";
        }
        else
        {
            nextWaveTimeText.text = "";
            monsterCountText.text = $"Current Monsters : {GameManager.Instance.BuildingManager.generator.MonsterCount}";
        }

        if (GameManager.Instance.NetworkManager.Runner.IsServer && GameManager.IsGameStart)
        {
            if(GameManager.Instance.BuildingManager.generator.IsWaveStart)
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
                    if (nowWaveTime >= GameManager.Instance.BuildingManager.generator.WaveInterval)
                    {
                        nowWaveTime = 0;
                        currentMonsterIndex = 0;
                        waveInfo.waveOrder.Dequeue();
                        Debug.Log($"{currentWaveIndex}��° ���̺� ��");
                        if(waveInfo.waveOrder.Count > 0) GameManager.Instance.BuildingManager.generator.WaveInterval = waveInfo.waveOrder.Peek().Count * monsterInterval;
                        currentWaveIndex++;
                        if (currentWaveIndex % 1 == 0) // ���̺긶�� ���� ��Ұ� �����.
                        {
                            if (SpawnLoc <= roadData.Count - 1) // �� ���� �ƴ϶�� +1
                                SpawnLoc++;
                        }
                    }
                    else
                    {
                        nowWaveTime += deltaTime;
                    }

                }
                else
                {
                    GameManager.Instance.BuildingManager.generator.IsWaveStart = false;
                }

            }
            else if(waveInfo.waveOrder.Count == 0 && GameManager.Instance.BuildingManager.generator.MonsterCount == 0)
            {
                Debug.Log("Ŭ����!");
            }
        }
    }
}
