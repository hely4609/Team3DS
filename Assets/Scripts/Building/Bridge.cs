using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bridge : Building
{
    public override bool CheckBuild() { return default; } // buildPos는 건설하는 타워의 왼쪽아래

    protected override void Initialize()
    {
    }
}
