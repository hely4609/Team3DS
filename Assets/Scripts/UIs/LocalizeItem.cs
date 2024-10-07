using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Components;

public class LocalizeItem : MonoBehaviour
{
    public void LocalizeTextString(string tableName, InteractableBuilding building)
    {
        GetComponent<LocalizeStringEvent>().StringReference
            .SetReference(tableName, building.ObjectName);
    }
}
