using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script for impacts
public class BL_Impact : MonoBehaviour {

    // Impact has a delay life to allow the particle effect and sound to play before it's removed
    public float life = 0.5f;

    // Private variables
    private bool _poolFlag;

    void OnEnable()
    {
        // When object is enabled (i.e. instantiated)
        if (!_poolFlag)
        {
            // If we are pooling this object we don't want to invoke the delayed disable
            // Set the pool flag to true now that we are likely pooling the impacts
            _poolFlag = true;
            // Return, we don't want to run the rest of this method during the pooling process
            return;
        }
        // Do the Disable() method after <life> seconds. Basically, sets this gameobject to inactive which
        // effectively returns it to the pool of impacts after a delay (let's the particle effects and sound play out first)
        Invoke("Disable", life);
    }

    void Disable()
    {
        // Disable the gameobject which hides it and returns it to the pool to be used later on
        gameObject.SetActive(false);
    }
}
