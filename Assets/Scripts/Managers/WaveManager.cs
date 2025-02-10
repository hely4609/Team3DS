using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.SmartFormat.Extensions;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

public class WaveManager : Manager
{
    private List<ResourceEnum.Prefab> waveMonsterList = new List<ResourceEnum.Prefab>(); //�� ���̺꿡 � ���Ͱ�.

    protected WaveInfo waveInfo;
    protected int currentWaveIndex = 0; //���� ���̺�� ���° ���̺��ΰ�.
    public int CurrentWaveIndex { get { return currentWaveIndex; } }
    protected List<Monster> monsterList = new List<Monster>(); // ���� ���� ��. 
    public List<Monster> MonsterList { get { return monsterList; } }

    protected int currentMonsterIndex = 0; //���� ���̺꿡�� ���° �����ΰ�.
    private float monsterInterval = 2; // ���Ͱ��� ����(�ʿ��Ѱ�?)
    protected float nowMonsterTime = 0; // ���� ���� ���� �ð�
    protected float waveInterval; // ���� ���̺������ �ð�
    protected float nowWaveTime = 0; // ���� ���̺� ���� �ð�
    protected int spawnLoc = 3; // ���̺� ���� ��ġ. ���� ���Ƿ� �����.
    IntVariable monsterNumber; // ���� ���̺� ���� ���� ���� ȭ�鿡 ���� ���� Localize����.

    public int SpawnLoc { get { return spawnLoc; } protected set { spawnLoc = value; } }
    List<Vector2> roadData;

    public GameObject waveInfoUI;
    TextMeshProUGUI nextWaveTimeText;
    TextMeshProUGUI monsterCountText;

    // �÷��̾� ������ �����ϴ� ����
    public Vector3 BoundLeftUp { get; private set; }
    public Vector3 BoundRightDown { get; private set; }
    private int nowArea = 0;
    public int NowArea { get { return nowArea; } private set { if(value<AreaBounds.Length) value = nowArea; } }
    public GameObject walls;
    private static Vector3[] zeroArea = new Vector3[2] { new Vector3(-57, 0, 30), new Vector3(20, 0, -57) };
    private static Vector3[] firstArea  = new Vector3[2] { new Vector3(-57, 0, 30), new Vector3(57, 0, -57) };
    private static Vector3[] secondArea  = new Vector3[2] { new Vector3(-57, 0, 60), new Vector3(57, 0, -57) };
    private static Vector3[] thirdArea  = new Vector3[2] { new Vector3(-90, 0, 60), new Vector3(57, 0, -57) };
    private static Vector3[] fourthArea  = new Vector3[2] { new Vector3(-90, 0, 60), new Vector3(57, 0, -90) };
    private static Vector3[] fifthArea  = new Vector3[2] { new Vector3(-90, 0, 60), new Vector3(90, 0, -90) };
    private static Vector3[] sixthArea  = new Vector3[2] { new Vector3(-90, 0, 90), new Vector3(90, 0, -90) };

    public Vector3[][] AreaBounds = new Vector3[][]{ zeroArea, firstArea, secondArea, thirdArea, fourthArea, fifthArea, sixthArea };
    //public void DrawBound(Vector3 leftUp, Vector3 rightDown)
    //{
    //    BoundLeftUp = leftUp;
    //    BoundRightDown = rightDown;
    //}
    //
    protected GameObject CreateBox(Vector3 leftUp, Vector3 rightDown)
    {
        Vector3 upMid = new Vector3((leftUp.x + rightDown.x) / 2, 2, leftUp.z);
        Vector3 downMid = new Vector3((leftUp.x + rightDown.x) / 2, 2, rightDown.z);
        Vector3 rightMid = new Vector3(rightDown.x, 2, (leftUp.z + rightDown.z) / 2);
        Vector3 leftMid = new Vector3(leftUp.x, 2, (leftUp.z + rightDown.z) / 2);
        float upLength = rightDown.x - leftUp.x;
        float rightLength = leftUp.z - rightDown.z;
        GameObject objParent = new GameObject("wall");
        GameObject upObj = GameManager.Instance.PoolManager.Instantiate(ResourceEnum.Prefab.NoEnterWall, upMid);
        GameObject downObj = GameManager.Instance.PoolManager.Instantiate(ResourceEnum.Prefab.NoEnterWall, downMid);
        GameObject rightObj = GameManager.Instance.PoolManager.Instantiate(ResourceEnum.Prefab.NoEnterWall, rightMid);
        GameObject leftObj = GameManager.Instance.PoolManager.Instantiate(ResourceEnum.Prefab.NoEnterWall, leftMid);
        upObj.transform.SetParent(objParent.transform);
        upObj.transform.localScale = new Vector3(upLength, upObj.transform.localScale.y, upObj.transform.localScale.z);
        downObj.transform.SetParent(objParent.transform);
        downObj.transform.localScale = new Vector3(upLength, downObj.transform.localScale.y, downObj.transform.localScale.z);
        rightObj.transform.SetParent(objParent.transform);
        rightObj.transform.localScale = new Vector3(rightObj.transform.localScale.x, rightObj.transform.localScale.y, rightLength);
        leftObj.transform.SetParent(objParent.transform);
        leftObj.transform.localScale = new Vector3(leftObj.transform.localScale.x, leftObj.transform.localScale.y, rightLength);


        return objParent;
    }
    protected void MonsterInstantiate()
    {
        if (GameManager.Instance.NetworkManager.Runner.IsServer)
        {
            if (waveInfo.waveOrder.Peek().Count > 0)
            {
                //int number = Random.Range((int)ResourceEnum.Prefab.Slime_Leaf, (int)ResourceEnum.Prefab.Slime_King + 1);
                int number = (int)waveInfo.waveOrder.Peek().Dequeue();
                List<Vector2> roadData = GameManager.Instance.BuildingManager.roadData;
                monsterList.Add(GameManager.Instance.NetworkManager.Runner.Spawn(ResourceManager.Get((ResourceEnum.Prefab)number), new Vector3(roadData[spawnLoc].x, 0, roadData[spawnLoc].y)).GetComponent<Monster>());
                GameManager.Instance.BuildingManager.generator.MonsterCount++;
            }
        }

    }

    public override IEnumerator Initiate()
    {
        //waveMonsterList.Add(ResourceEnum.Prefab.EnemyTest);
        waveInfo = new WaveInfo();
        waveInfo.Initialize();

        roadData = GameManager.Instance.BuildingManager.roadData;

        walls = CreateBox(AreaBounds[nowArea][0], AreaBounds[nowArea][1]);

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
                var source = LocalizationSettings.StringDatabase.SmartFormatter.GetSourceExtension<PersistentVariablesSource>();
                monsterNumber = source["ResultGroup"]["CurrentMonsters"] as IntVariable;
                Debug.Log($"{monsterNumber.Value} = ���� ���� ��");
                waveInfoUI.SetActive(false);
            }
        }

    }
    public void WaveStart()
    {
        if (GameManager.Instance.NetworkManager.Runner.IsServer)
        {
            GameManager.Instance.BuildingManager.generator.IsWaveStart = true;
            GameManager.Instance.BuildingManager.generator.IsWaveLeft = true;
            waveInterval = waveInfo.waveOrder.Peek().Count * monsterInterval;
        }
        if (waveInfoUI == null) FindWaveInfoUI();
        if (GameManager.Instance.NetworkManager.Runner.LocalPlayer == GameManager.Instance.NetworkManager.LocalController.myAuthority)
        {
            if (waveInfoUI != null) waveInfoUI.SetActive(true);
        }
    }

    public override void ManagerStart()
    {
        if (GameManager.Instance.NetworkManager.Runner.LocalPlayer == GameManager.Instance.NetworkManager.LocalController.myAuthority && GameManager.Instance.BuildingManager.generator.IsWaveStart) WaveStart();
    }

    public override void ManagerUpdate(float deltaTime)
    {
        if (waveInfoUI == null) FindWaveInfoUI();
        else if(waveInfo.waveOrder.Count > 1 || GameManager.Instance.BuildingManager.generator.IsWaveLeft)
        {
            nextWaveTimeText.text = $"{(int)(GameManager.Instance.BuildingManager.generator.PlayTime) / 60:00} : {(int)(GameManager.Instance.BuildingManager.generator.PlayTime) % 60:00}";
            Debug.Log($"{GameManager.Instance.BuildingManager.generator.MonsterCount}");
            monsterNumber.Value = GameManager.Instance.BuildingManager.generator.MonsterCount;
            //monsterCountText.text = $"Current Monsters : {GameManager.Instance.BuildingManager.generator.MonsterCount}";
        }
        else
        {
            nextWaveTimeText.text = "";
            //monsterCountText.text = $"Current Monsters : {GameManager.Instance.BuildingManager.generator.MonsterCount}";
        }

        if (GameManager.Instance.NetworkManager.Runner.IsServer && GameManager.IsGameStart)
        {
            if(GameManager.Instance.BuildingManager.generator.IsWaveStart)
            {
                nowMonsterTime += deltaTime;
            
                if (nowMonsterTime >= monsterInterval && waveInfo.waveOrder.Count > 0 && waveInfo.waveOrder.Peek().Count > 0) 
                {
                    MonsterInstantiate();
                    nowMonsterTime = 0;
                    //Debug.Log($"{waveInfo.waveOrder.Peek().Count}���� ����");
                }
                else if(waveInfo.waveOrder.Count > 0)
                {
                    Debug.Log($"{nowWaveTime}/{waveInterval}   {monsterList.Count}����");
                    //if (nowWaveTime >= waveInterval)
                    if(nowWaveTime >= waveInterval && monsterList.Count <= 0)
                    {
                        nowWaveTime = 0;
                        currentMonsterIndex = 0;
                        waveInfo.waveOrder.Dequeue();
                        Debug.Log($"{currentWaveIndex}��° ���̺� ��");
                        if(waveInfo.waveOrder.Count > 0)
                        {
                            waveInterval = waveInfo.waveOrder.Peek().Count * monsterInterval;
                            GameManager.Instance.BuildingManager.generator.IsWaveLeft = true;
                        }
                        else GameManager.Instance.BuildingManager.generator.IsWaveLeft = false;
                        currentWaveIndex++;
                        if (currentWaveIndex % 1 == 0) // ���̺긶�� ���� ��Ұ� �����.
                        {
                            if (SpawnLoc < roadData.Count - 1) // �� ���� �ƴ϶�� +1
                            {
                                SpawnLoc++;
                                nowArea++;
                                GameObject nextWall= CreateBox(AreaBounds[nowArea][0], AreaBounds[nowArea][1]);
                                GameObject.Destroy(walls);
                                walls = nextWall;
                                Debug.Log($"���� ���� : {BoundLeftUp}, {BoundRightDown}");
                            }


                        }
                    }
                    else
                    {
                        nowWaveTime += deltaTime;
                        //GameManager.Instance.BuildingManager.generator.WaveLeftTime = waveInterval - nowWaveTime;
                    }

                }
                else
                {
                    GameManager.Instance.BuildingManager.generator.IsWaveLeft = false;
                    GameManager.Instance.BuildingManager.generator.IsWaveStart = false;
                }

            }
            else if(waveInfo.waveOrder.Count == 0 && GameManager.Instance.BuildingManager.generator.MonsterCount == 0)
            {
                GameManager.Instance.GameClear();
                GameManager.Instance.GameOver();
            }
        }
    }
}
