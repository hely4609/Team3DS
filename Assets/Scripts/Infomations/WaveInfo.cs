using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveInfo : MonoBehaviour
{
    // ���⿡ ���Ͱ� ��� ������ �������.
    // ��� �����͸� �ϴ� �� ����ΰ� �ұ�?
    private Dictionary<MonsterEnum, int> waveDictionary; //� ���Ͱ�, �󸶸�ŭ.
    private Queue<MonsterEnum> waveOrder; 
    private float monsterInterval;
    private float waveInterval;

    public void SetInfo(string waveInfo) { }


    
}
