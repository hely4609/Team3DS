using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WaveManager : Manager
{
    private List<ResourceEnum.Prefab> waveMonsterList = new List<ResourceEnum.Prefab>(); //몇 웨이브에 어떤 몬스터가.

    protected WaveInfo waveInfo;
    protected int currentWaveIndex = 0; //현재 웨이브는 몇번째 웨이브인가.
    protected List<Monster> monsterList = new List<Monster>(); // 남은 몬스터 수. 

    protected int currentMonsterIndex = 0; //현재 웨이브에서 몇번째 몬스터인가.
    private float monsterInterval = 2; // 몬스터간의 간격(필요한가?)
    protected float nowMonsterTime = 0; // 현재 몬스터 생성 시간
    protected float nowWaveTime = 0; // 현재 웨이브 진행 시간
    protected int spawnLoc = 4; // 웨이브 시작 위치. 현재 임의로 적어둠.

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
                    Debug.Log($"{waveInfo.waveOrder.Peek().Count}마리 남음");
                }
                else if(waveInfo.waveOrder.Count > 0)
                {
                    if (nowWaveTime >= GameManager.Instance.BuildingManager.generator.WaveInterval)
                    {
                        nowWaveTime = 0;
                        currentMonsterIndex = 0;
                        waveInfo.waveOrder.Dequeue();
                        Debug.Log($"{currentWaveIndex}번째 웨이브 끝");
                        if(waveInfo.waveOrder.Count > 0) GameManager.Instance.BuildingManager.generator.WaveInterval = waveInfo.waveOrder.Peek().Count * monsterInterval;
                        currentWaveIndex++;
                        if (currentWaveIndex % 1 == 0) // 웨이브마다 스폰 장소가 변경됨.
                        {
                            if (SpawnLoc <= roadData.Count - 1) // 길 끝이 아니라면 +1
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
                Debug.Log("클리어!");
            }
        }
    }
}
