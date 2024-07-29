using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveInfo : MonoBehaviour
{
    // 여기에 몬스터가 어떻게 나올지 적어야함.
    // 모든 데이터를 일단 다 적어두고 할까?
    private Dictionary<MonsterEnum, int> waveDictionary; //어떤 몬스터가, 얼마만큼.
    private Queue<MonsterEnum> waveOrder; 
    private float monsterInterval;
    private float waveInterval;

    public void SetInfo(string waveInfo) { }


    
}
