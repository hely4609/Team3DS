using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Script for explosions to apply force to nearby objects
public class BL_Explosion : MonoBehaviour {
    public float radius;
    public float power;
    public float upModifier;
	
    void OnEnable()
    {
        // When enabled...

        // Get the transform position    
        Vector3 _explosionPos = transform.position;

        // Find all colliders within <radius> of the explosion
        Collider[] _colliders = Physics.OverlapSphere(_explosionPos, radius);

        // Loop through all the colliders that are affected by the blast
        foreach (Collider _hit in _colliders)
        {
            // Get a reference to the rigidbody of the nearby objects
            Rigidbody _rb = _hit.GetComponent<Rigidbody>();
            
            if (_rb != null)
                // If the object has a rigidbody, apply explosion force <power> as an impulse force
                // The explosion force will affect nearby objects more than distant object since we're using the position
                _rb.AddExplosionForce(power, _explosionPos, radius, upModifier, ForceMode.Impulse);
        }
    }
}
