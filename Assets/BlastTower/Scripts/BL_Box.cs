using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// This is just a demo script to get the boxes (virtual enemies) to move towards a spot in front of the demo turret
public class BL_Box : MonoBehaviour {
    
    // Set the explosion prefab of this object
    public GameObject prefabExplosion;    

    // Number of hits to take before being destroyed
    public int hp = 10;

    // The location to which the boxes will move
    public Vector3 targetLocation = new Vector3(0, 0, 30);
    
    // The force applied to the object to move it towards the designated spot
    public float force;

    // Max velocity will prevent the boxes from moving too fast
    public float maxVelocity;
    
    // Private variables
    private Rigidbody _rigidbody;
    private Transform _transform;
    private int _orgHp;

	// Use this for initialization
	void Start () {
        // Grab a reference to the Rigidbody and Transform components
        _rigidbody = gameObject.GetComponent<Rigidbody>();
        _transform = gameObject.transform;

        // Remember the original HP for when respawnning
        _orgHp = hp;
	}
	

    void FixedUpdate()
    {        
        if (_rigidbody.velocity.magnitude < maxVelocity)
        {
            // If the box is moving at a lower speed than max velocity, add force to make it move towards the target spot
            _rigidbody.AddForce((targetLocation - _transform.position).normalized * force, ForceMode.Force);
        }        
    }

    public void Hit()
    {
        // The Hit() Method is automatically called by SendMessage in the BL_Bullet.cs script - when a bullet hits, deduct one HP (hitpoint)
        // You can add more stuff in the Hit() Method if you supply arguments when calling using SendMessage in the bullet script, e.g. you
        // May want to ensure that it's a hostile bullet, or a a damage value for stronger/weaker blasters, etc.
        hp--;
        
        if (hp <= 0)
        {
            // If HP is 0 or less, call the Destroy method
            Destroy();
        }
    }

    void Destroy()
    {
        // Instantiate an explosion at the location of the transform. If you have many explosions you would probably want to pool explosions
        // instead like we do with bullets and impact effects.
        Instantiate(prefabExplosion, _transform.position, Quaternion.identity);
        
        // Instead of destroying the gameobject we reset the hitpoint and move the box to a new random location, e.g. "respawning" it somewhere
        hp = _orgHp;
        _rigidbody.velocity = Vector3.zero;
        _transform.position = new Vector3(Random.Range(-100, 100), 40, 150);
    }
    
}
