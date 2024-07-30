using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveInfo : MonoBehaviour
{
    // 여기에 몬스터가 어떻게 나올지 적어야함.
    // 모든 데이터를 일단 다 적어두고 할까?
    private Dictionary<int, MonsterEnum> waveDictionary; //몇 웨이브에 어떤 몬스터가.
    //private Queue<MonsterEnum> waveOrder; 
    private float monsterNumber; // 몬스터의 수
    private float monsterInterval; // 몬스터간의 간격(필요한가?)
    private float waveInterval; // 다음 웨이브까지의 시간
    public void SetInfo(string waveInfo) { }


    
}
