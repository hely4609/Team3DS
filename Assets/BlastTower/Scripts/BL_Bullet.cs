using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BL_Bullet : MonoBehaviour {
    // Velocity and life time in second of bullet (these are overridden by Turret when firing)
    public float velocity = 100f;
    public float life = 2.0f;
    // Impact force magnitude to be applied as an impulse force to target that's hit by the bullet
    public float impactImpulseForce = 50f;

    // We need a reference to the turretParent to spawn the pooled impact prefab effect on impect.
    [HideInInspector]
    public BL_Turret turretParent;

    // Private variables - should not need any changing, for internal use
    private TrailRenderer _trailRenderer;
    private ParticleSystem _particleSystem;
    private Transform _transform;
    private AudioSource _audioSource;
    private bool _hasHit;

    void Awake () {
        // Get a reference to the bullet trail render component
        _trailRenderer = gameObject.GetComponent<TrailRenderer>();

        // Get a reference to the bullet particle system component
        _particleSystem = gameObject.GetComponent<ParticleSystem>();

        // Reference to the bullet transform itself
        _transform = gameObject.GetComponent<Transform>();

        // Audio source for bullet (if flying sound is desired, e.g. when bullet whooshes past)
        _audioSource = gameObject.GetComponent<AudioSource>();
	}
	
    void OnEnable()
    {
        // Invoke the Destroy method to execute after <life> seconds
        Invoke("Destroy", life);

        // Clear the trail render and particle system (if there are any) since we want a fresh bullet        
        if (_trailRenderer != null) _trailRenderer.Clear();
        if (_particleSystem != null) _particleSystem.Clear();

        // Start playing the bullet particle system
        _particleSystem.Play();

        // Reset the hit flag to false
        _hasHit = false;
        
        // Play the flying sound if enabled (change the sound and proerties of the AudioSource on the bullet prefab itself).
        if (_audioSource != null) _audioSource.Play();
    }

    void OnDisable()
    {
        // Cancel the delaed execution to destroy the bullet if it is being disabled
        CancelInvoke();
    }

    void Destroy()
    {     
        // We don't really destroy the gameobject since it's a pool of objects. We just set it to be inactive and then we
        // activate it again when it's being fired. This will perform much better than instantiating and destroying bullets all the time.
        gameObject.SetActive(false);
    }

	// Update is called once per frame
	void Update () {
        // If the bullet has hit already we don't need to go through the update loop any more - we're waiting for the bullet to expire
        if (_hasHit) return;

        // Move the bullet forward according to the velocity of the bullet
        transform.Translate(Vector3.forward * velocity * Time.deltaTime);

        // Check for a collider hit using Raycast
        RaycastHit _hit;

        // HINT! You can tweak the formula  (4th argument) to suit your need so bullets don't go through objects or hit too early.
        // You will need to play around with the velocity and multipliers to find something that suits your game. Values depend on
        // the velocity, size of colliders, angle of your game, etc.        
        if (Physics.Raycast(_transform.position, _transform.forward, out _hit, 2 + (velocity * 0.02f)))
        {
            // Get the target (hit) collider
            Collider _target = _hit.collider;

            // Calculate the incoming vector (hit point minus bullets position since we are not quite yet at the target, only the raycast
            // has detected the hit.
            Vector3 _incomingVec = _hit.point - _transform.position; 
            // Calculate the reflected vector against the normal (angle) of the surface we hit - this is used to align the bullet impact particle effect.
            Vector3 _reflectVec = Vector3.Reflect(_incomingVec, _hit.normal);

            // Identify the target gameobject that the bullet just hit
            GameObject _targetGameObject = _hit.collider.gameObject; 

            // Get the direction between the target object and the bullet position
            Vector3 _direction = _targetGameObject.transform.position - _transform.position;

            // See if the target has a rigidbody component
            Rigidbody _rt = _targetGameObject.GetComponent<Rigidbody>();

            // If target object has a rigid body, apply an impulse force from the bullet
            if (_rt != null)
            {
                _rt.AddForceAtPosition(_direction.normalized * impactImpulseForce, _hit.point, ForceMode.Impulse);
            }

            // Set the bullet position to be the same as the hit point (remember, it's still just the raycast that detected the hit)
            _transform.position = _hit.point;

            // Stop the bullet's velocity
            velocity = 0f;

            // Send a "Hit" message to the target object that was hit this doesn't require the target object to listen.
            // If the target object has a public Hit method, e.g. public void Hit(), it will be called when it was hit.
            // You may want to add the ability to send damage or information of a team that you belong to etc. using the
            // second argument which is currently set to null.
            _targetGameObject.SendMessage("Hit", null, SendMessageOptions.DontRequireReceiver);

            // The bullet has hit so we should stop the particle system and trail renderers from emitting any more particles.
            _particleSystem.Stop();            
            _trailRenderer.Clear();

            // If the bullet has a flying sound, we stop it now since we've impacted.
            if (_audioSource != null) _audioSource.Stop();

            // Invoke a delayed destroy method to deactiate the bullet from the hierarchy and return it to the pool. 
            // We do this with a delay to let the particles and trail fade out and not stop abruptly.
            Invoke("Destroy", 1f);

            // Set the hit flag to true so we don't need to go through the update process any more for this bullet as
            // it would result in multiple hits by the same bullets. And LOTS of them =)
            _hasHit = true;

            // Enable a pooled impact effect at hit position
            if (turretParent != null) turretParent.Impact(_hit.point, _reflectVec);

            // Uncomment these debug calls if you want to see the bullet direction and impact reflection in the Scene view.
            //Debug.DrawLine(turretParent.transform.position, _hit.point, Color.red);
            //Debug.DrawRay(_hit.point, _reflectVec, Color.green);
        }
    }



}
