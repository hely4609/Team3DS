using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractTest : MonoBehaviour ,IInteraction
{
    bool IInteraction.InteractionEnd()
    {
        throw new System.NotImplementedException();
    }

    Interaction IInteraction.InteractionStart(Player player)
    {
        throw new System.NotImplementedException();
    }

    bool IInteraction.InteractionUpdate(float deltaTime, Interaction interaction)
    {
        throw new System.NotImplementedException();
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
