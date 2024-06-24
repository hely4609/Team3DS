using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapManager : Manager
{
    //2CB 참고.
    //Layer설정으로관리
    //MiniMapCamera의 CullingMask로 보고싶은 Layer조정가능
    //RenderTexture필요
    public override IEnumerator Initiate() { yield return null; }
}
