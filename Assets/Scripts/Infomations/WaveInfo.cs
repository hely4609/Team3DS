using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveInfo : MonoBehaviour
{
    private Dictionary<MonsterEnum, int> waveDictionary; //테스트용
    private Queue<MonsterEnum> waveOrder;
    private float monsterInterval;
    private float waveInterval;

    public void SetInfo(string waveInfo) { }
}
