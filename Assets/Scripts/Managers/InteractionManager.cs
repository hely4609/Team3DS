using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class InteractionManager : Manager
{
    protected List<IInteraction> interactions;
    //protected Dictionary<IInteraction, GameObject> interactionDictionary;

    public override IEnumerator Initiate()
    {
        if ( interactions == null ) interactions = new List<IInteraction>();
        //if ( interactionDictionary == null) interactionDictionary = new Dictionary<IInteraction, GameObject>();
        yield return null;
    }

    public void AddInteractionObject(IInteraction target)
    {
        interactions.Add(target);
        //interactionDictionary.Add(target, targetObj);
    }

    public void RemoveInteractionObject(IInteraction target) 
    {
        interactions.Remove(target);
        //interactionDictionary.Remove(target);
    }

    public List<IInteraction> CheckInteractionObjInRange(Vector3 center, float range)
    {
        if (center == null) return null;
        if (range <= 0f) return null;
        Debug.Log(interactions.Count);

        List<IInteraction> insts = new List<IInteraction>();
        foreach (var inst in interactions)
        {
            if (Vector3.Distance(center, inst.GetPosition()) <= range)
            {
                insts.Add(inst);
            }
        }

        return insts;
    }
}
