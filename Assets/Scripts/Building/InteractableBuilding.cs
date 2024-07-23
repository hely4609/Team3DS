using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableBuilding : Building, IInteraction
{ 
    protected override void Initialize()
    {
    }
    // 위치를 고정함.


    // 상호작용을 시작함.
    // 상호작용을 할 분류를 받아와서 어떤 걸 할 지 저장.
    // 플레이어가 

    // 스타트와 엔드는 단발성.
    // 플레이어는 상호작용 키 만 누를거임. 내용은 모름.
    // 뭘 할지는 얘가 플레이어한테 알려줄거임.
    // 플레이어는 건설, 수리이면 망치를 휘두를거고, onoff 면 손만 나와서 누를거고, 납품이면 손에 있는게 사라질거임.
    // 지속 = 업데이트.

    // 아무것도 없는 깡통건물. 세우기만 하고 상호작용 없음. ex) 육교
    public virtual Interaction InteractionStart(Player player)
    {
        // 완성이 아직 안됨.
        if (completePercent < 1)
        {
            return Interaction.Build;
        }
        else
        {
            return Interaction.None;
        }
    }
    public virtual bool InteractionUpdate(float deltaTime, Interaction interaction) // 상호작용시 적용할 함수. 제작하라는 명령이 들어오면 제작을 진행함.
    {
        if (interaction == Interaction.Build)
        {
            BuildBuilding(deltaTime);
        }
        return true;
    }

    public bool InteractionEnd()
    {

        Debug.Log("끝");
        return true;
    }

}
