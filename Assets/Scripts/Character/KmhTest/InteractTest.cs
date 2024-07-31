using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractTest : MonoBehaviour ,IInteraction
{
    Mesh mesh;

    public void Start()
    {
        //GameManager.Instance.InteractionManager.AddInteractionObject(this);
    }

    public Bounds GetInteractionBounds()
    {
        if (mesh == null)
        {
            mesh = GetComponent<Mesh>();
        }
        return mesh.bounds;
    }

    public Collider[] GetInteractionColliders()
    {
        //if (colliders == null)
        //{
        //    colliders = GetComponentsInChildren<Collider>();
        //}
        return default;
        //return colliders;
    }

    public string GetName()
    {
        return "InteractableObject";
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
