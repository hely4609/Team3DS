using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script to emit particles for muzzle flash effect when firing blasters

public class BL_MuzzleFlash : MonoBehaviour {

    // Private variables
    private ParticleSystem _particleSystem;

    public void Flash()
    {
        // Publically available method to emit flash particles from particle system component
        if (_particleSystem == null) _particleSystem = gameObject.GetComponent<ParticleSystem>();

        // Emit 1 particle from the particle system         
        _particleSystem.Emit(1);
    }
}
