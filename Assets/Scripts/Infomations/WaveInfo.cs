using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveInfo : MonoBehaviour
{
    private Dictionary<MonsterEnum, int> waveDictionary; //�׽�Ʈ��
    private Queue<MonsterEnum> waveOrder;
    private float monsterInterval;
    private float waveInterval;

    public void SetInfo(string waveInfo) { }
}
