using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractTest : MonoBehaviour ,IInteraction
{
    bool IInteraction.InteractionEnd()
    {
        return default;
    }

    Interaction IInteraction.InteractionStart(Player player)
    {
        return default;

    }

    bool IInteraction.InteractionUpdate(float deltaTime, Interaction interaction)
    {
        return default;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
