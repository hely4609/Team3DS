using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bridge : Building
{
    public override bool CheckBuild() { return default; } // buildPos�� �Ǽ��ϴ� Ÿ���� ���ʾƷ�

    protected override void Initialize()
    {
    }
}
