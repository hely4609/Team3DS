using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractTest : MonoBehaviour ,IInteraction
{
    Collider[] colliders;
    public Collider[] GetInteractionColliders()
    {
        if (colliders == null)
        {
            colliders = GetComponentsInChildren<Collider>();
        }

        return colliders;
    }

    bool IInteraction.InteractionEnd()
    {
        return default;
    }

    Interaction IInteraction.InteractionStart(Player player)
    {
        return default;

    }

    float IInteraction.InteractionUpdate(float deltaTime, Interaction interaction)
    {
        return default;
    }
}
