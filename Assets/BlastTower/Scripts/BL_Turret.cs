using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Main script for turrets to aim and fire
public class BL_Turret : MonoBehaviour {
    [Header("AUDIO")]

    // Fire Sound effect is a public array. Drag and drop sounds to this array to play when firing a blaster.
    // This is an array and if it contains multiple sound effects it will perform a "round robin" cycle to alter the
    // sound effect each time the blaster is fired.
    [Tooltip("1 or more sound effects (multiple sound effects will be alternated with each firing using round robin method).")]
    public AudioClip[] sfxFire;
    // Volume of fire sound effect (0.0 - 1.0 loudest)
    public float volumeFire = 1.0f;

    // Impact sound effect. This is an array and if it contains multiple sounds, the sound will be played in a "round robin" fashion.
    [Tooltip("1 or more sound effects (multiple sound effects will be alternated with each impact using round robin method).")]
    public AudioClip[] sfxImpact;
    // Volume of impact sound effect (0.0 - 1.0 loudest)
    public float volumeImpact = 1.0f;

    [Header("TURRET PROPERTIES")]

    // Rate of fire (rounds per minute)
    public float rateOfFire = 300f;

    // Fire sequentially (one blaster after another in the children blasters to this turret) or simultaneously (false)
    public bool fireSequential;

    // Rotation speed of the turret, lower is slower, higher value is faster
    public float rotationSpeed = 10.0f;

    // Lock Y yes/no (can turret aim up/down?)
    public bool lockYAxis = true;

    // Reference to the prefab used for muzzle flashes (must contain an appropriate particle system.
    // Duplicate and modify existing muzzle flash prefabs if you want to make your own
    public GameObject muzzleFlashPrefab;

    [Header("BULLET PROPERTIES")]

    // Reference to the bullet prefab to be fired out of the blasters of this turret
    public GameObject bulletPrefab;

    // Bullet velocity of the bullet prefabs fired out of this turret
    public float bulletVelocity = 100f;

    // Bullet life, time until the bullet is deactivated, hidden, and returned to the pool
    public float bulletLife = 2.0f;    

    // Reference to the impact prefab to be shown when a bullet hits an object
    public GameObject impactPrefab;

    // Size of the impact pool (number of impacts that can be shown simultaneously)
    int impactPoolSize = 8;

    // PRIVATE VARIABLES

    // List of pooled bullets gameobjects
    private List<GameObject> _bullets = new List<GameObject>();
    // List of pooled impact gameobjects
    private List<GameObject> _impacts = new List<GameObject>();
    
    // Blasters that are children of this turret
    private BL_Blaster[] _blasters;

    // Private variable of bullet pool size, pool size is adjusted based on rate of fire and and bullet life
    // It will also grow the pool if necessary.
    private int _bulletPoolSize = 120;

    // Timer used to control rate of fire
    private float _timerFire;

    // Counter to keep track of which blaster in sequence to fire
    private int _counter;
    
    // Flag used to fire the appropriate blaster in the Fire() method
    private bool _fire;

    // Reference to the local transform of the turret
    private Transform _transform;

    // Target is the aim point of the turret
    private Vector3 _target;

    // Counters for the round robin sequencing of fire and impact sound arrays
    private int _sfxRoundRobinFire;
    private int _sfxRoundRobinImpact;

    void Awake () {
        // Find the child blasters of this turret and store it in _blasters array
        GetBlasters();

        // Set the active muzzle flash prefab (instantiates the prefab at the muzzle)
        SetMuzzleFlash(muzzleFlashPrefab);        

        // Aim straight forward to begin with
        _target = Vector3.forward;

        // Pool (instantiate) the bullets and deactivate them (this is much faster than instantiating 
        // and destroying bullets during runtime all the time.
        PoolBullets();

        // Pool (instantiate) the impact effects and deactivat them (for the same reason as with the bullets above)
        PoolImpacts();
    }
	
	void Update () {        
        // Set the looking position to the target minus the position of the turret        
        Vector3 _lookPos = _target - transform.position;
        
        // If lock Y axis is set, reset the y axis to 0
        if (lockYAxis) _lookPos.y = 0;

        // Use LookRotation to get the direction to aim at the target position _lookPos
        Quaternion _rotation = Quaternion.LookRotation(_lookPos);

        // Interpolate the rotation of the turrent based on the rotation speed variable
        transform.rotation = Quaternion.Slerp(transform.rotation, _rotation, Time.deltaTime * rotationSpeed);
    }

    // public method to aim the turret at a target position
    public void Aim(Vector3 _newTarget)
    {
        // Set target to the new target position
        _target = _newTarget;
    }

    // Public method to fire the turret (call this from external scripts like Players or AI)
    public void Fire(bool _useRoundRobin = true)
    {
        // If blasters are to be fired sequentially... 
        if (fireSequential)
        {
            // If more time has passed than 60/rateOfFire (seconds between each bullet) divided by the number of blasters on the turret...
            if (Time.time > _timerFire + ((60f / rateOfFire) / _blasters.Length))
                
            {
                // Increase the counter to fire the next blaster in sequence
                _counter++;
                if (_counter >= _blasters.Length) _counter = 0;                

                // Remember the timestamp when the blaster was fired
                _timerFire = Time.time;

                // Set the fire flag to true (used later on to actually fire the blaster)
                _fire = true;
            }
        }
            else
        {
            // If blasters are NOT fired sequentially (in otherwords, fire all blasters on the turret at the same time...

            // If more time has passed than 60/rateOfFire (seconds between each bullet)...
            if (Time.time > _timerFire + (60f / rateOfFire))
            {
                // Remember the timestamp when the blaster was fired
                _timerFire = Time.time;
                // Set the fire flag to true (used later on to actually fire the blaster)
                _fire = true;
            }
        }
        foreach(BL_Blaster _b in _blasters)
        {    
            // Loop through all the child blasters of this turret

            // If blaster is firing....
            if (_fire)
            {
                // If the blasters are not fired in sequence OR if we are currently iterrating though the next blaster in line to be fired...
                if (!fireSequential || (fireSequential && _b == _blasters[_counter]))
                {                    
                    // Fire the blaster using the public method Fire() of the Blaster. The rate of fire is used as an argument for the turret recoil animation timer
                    _b.Fire(rateOfFire);

                    //  Check if the array of sound effects to fire the blaster contains any sounds
                    if (sfxFire.Length > 0)
                    {
                        // If the sound effect is not null, play the clip at the point of the muzzle position of the blaster (audio source is created and destroyed for the purpose)
                        if (sfxFire[_sfxRoundRobinFire] != null) AudioSource.PlayClipAtPoint(sfxFire[_sfxRoundRobinFire], _b.muzzle.position, volumeFire);
                    }

                    // We now find a bullet in the bullet pool that is not active
                    GameObject _activeBullet = null;
                    for (int _i = 0; _i < _bullets.Count; _i++)
                    {
                        // Loop through the list of pooled bullets
                        if (!_bullets[_i].activeInHierarchy)
                        {
                            // When a bullet gameobject is not active in the hierarchy it means it's available for use, remember the active bullet reference
                            _activeBullet = _bullets[_i];

                            // Exit the for loops, we don't need to keep looking for any more bullets
                            break;
                        }
                    }

                    // If the active bullet is not null (i.e. we found a bullet in the pool)...
                    if (_activeBullet == null)
                    {
                        // Check that we have a bullet prefab configured for the blaster...
                        if (bulletPrefab != null)
                        {
                            // Instantiate a new bullet since the pool wasn't big enough
                            _activeBullet = (GameObject)Instantiate(bulletPrefab);                        

                            // Ad d the bullet to the list of pooled bullets
                            _bullets.Add(_activeBullet);
                        }
                    }

                    // If we have successfully referenced a new bullet, and muzzle object has been defined...
                    if (_activeBullet != null && _b.muzzle != null)
                    {
                        // Move the bullet to the position of the muzzle (remember, we take this from the pool, we don't instantiate a new one)
                        _activeBullet.transform.position = _b.muzzle.position;
                        // Align the bullet with the nozzle 
                        _activeBullet.transform.rotation = _b.muzzle.rotation;
                        // Set the bullet gameobject to active (show it)
                        _activeBullet.SetActive(true);
                        // Get the reference to the BL_Bullet script of the bullet (ache this for better performance if necessary)
                        BL_Bullet _bullet = _activeBullet.GetComponent<BL_Bullet>();
                        // Set the public turrentParent variable to this turret (when a bullet hits we want to set the impact effect that this turret has)
                        _bullet.turretParent = this;
                        // Set the velocity of the bullet
                        _bullet.velocity = bulletVelocity;
                        // Set the time to live of the bullet
                        _bullet.life = bulletLife;
                    }

                    // If round robin is used, step to the next sound
                    if (_useRoundRobin) _sfxRoundRobinFire++; 
                    if (_sfxRoundRobinFire >= sfxFire.Length) _sfxRoundRobinFire = 0;
                }
            }
        }

        // Reset the fire flag, we don't want to fire again until it's time
        _fire = false;
    }

    // Public Method to get the current sound effect name, used by the Demo - can be removed if not needed
    public string GetCurrentSFXName()
    {
        // If there is no sound effect, return an empty string
        if (sfxFire.Length <= 0) return "";
        // Return the name in upper case of the sound effect most recently played in the round robin array
        return sfxFire[_sfxRoundRobinFire].name.ToUpper();
    }

    // Internal method to find the child blasters of this turret
    void GetBlasters()
    {
        // Find all child blasters by looking for the component BL_Blaster script in children of the turret
        _blasters = gameObject.GetComponentsInChildren<BL_Blaster>();
    }


    // Pool bullets - it is much faster to pool bullets by instantiating them once and them activating/deactivating them
    public void PoolBullets()
    {
        // Calculate the size of the bullet pool required (we know the rate of fire and the bullet life time so we know how
        // many can live at the same time.
        _bulletPoolSize = Mathf.RoundToInt((rateOfFire / 60f) * bulletLife * 2) + 1;

        // Delete any previous pooled bullets
        for (int _i = 0; _i < _bullets.Count; _i++)
        {
            // Destroy the bullet
            Destroy(_bullets[_i], bulletLife);
        }
        // Clear the bullet list
        _bullets.Clear();

        // Create new bullet list
        _bullets = new List<GameObject>();

        // Instantiate and inactivate pool of bullets (much faster than instantiating bullets on demand)
        for (int _i = 0; _i < _bulletPoolSize; _i++)
        {
            // If there is a bullet prefab configured in the inspector...
            if (bulletPrefab != null) 
            {
                // Instantiate the new bullet and remember the gameobject reference
                GameObject _go = (GameObject)Instantiate(bulletPrefab);
                _go.GetComponent<BL_Bullet>().life = bulletLife;

                // Set a reference to the parent turret (neeeded to spawn impacts)
                _go.GetComponent<BL_Bullet>().turretParent = this;

                // Deactivate (hide) it
                _go.SetActive(false);

                // Add the bullet to the list of pooled bullets
                _bullets.Add(_go);
            }
        }

    }

    // Pool impacts - it's faster to enable/disable impacts rather than instantiating and destroying them each time
    public void PoolImpacts()
    {        

        // Delete previous pooled impacts
        for (int _i = 0; _i < _impacts.Count; _i++)
        {
            // Destroy the gameobject
            Destroy(_impacts[_i]);
        }

        // Clear the list of pooled impacts
        _impacts.Clear();

        // Create new list
        _impacts = new List<GameObject>();

        // Instantiate and inactivate pool of impact (much faster than instantiating impacts on demand)
        for (int _i = 0; _i < impactPoolSize; _i++)
        {
            // If an impact prefab has been configured in the inspector...
            if (impactPrefab != null)
            {
                // Instantiate a new impact
                GameObject _go = (GameObject)Instantiate(impactPrefab);

                // Deactivate (hide) it
                _go.SetActive(false);

                // Add the impact to the list of pooled impacts
                _impacts.Add(_go);
            }            
        }
    }


    // Impact effect
    public void Impact(Vector3 _position, Vector3 _eulerAngles)
    {
        // If the array of impact sounds has any impact sound in it...
        if (sfxImpact.Length > 0)
        {
            // Cycle to the next impact effect in the round robin sequence
            _sfxRoundRobinImpact++;
            if (_sfxRoundRobinImpact >= sfxImpact.Length) _sfxRoundRobinImpact = 0;

            // Play the impact sound effect at the position supplied as an argument by the bullet where it hit
            // The audio source is created and destroyed automatically
            AudioSource.PlayClipAtPoint(sfxImpact[_sfxRoundRobinImpact], _position, volumeImpact);
        }

        // If there are no pooled impacts, exit this method, we can't instantiate the impact effect in that case
        if (_impacts.Count == 0) return;

        // Loop through the pooled impacts
        for (int _i = 0; _i < impactPoolSize; _i++)
        {
            // If we find a pooled impact that is not currently in use...
            if (!_impacts[_i].activeInHierarchy)
            {
                // Move the impact effect to the position of the impact (supplied as an argument by the bullet script)
                _impacts[_i].transform.position = _position;

                // Set the rotation of the impact, this is the reflected(!) angle of the normal of the surface (as if the bullet
                // is bouncing off the surface when it hits like this: in \   / out
                //                                                         \ /
                //                                                          X impact
                _impacts[_i].transform.forward = _eulerAngles;

                // Set the gameobject to active (the particle effect of the impact must be set to play on awake)
                _impacts[_i].SetActive(true);

                // Exit this for loop, we don't need to look for more impacts to activate
                break;
            }
        }
    }

    // Change the impact effect prefab during runtime
    public void SetImpact(GameObject _impactPrefab)
    {
        // Set the new impact prefab
        impactPrefab = _impactPrefab;

        // Repool the impacts
        PoolImpacts();
    }

    // Change the bullet prefab during runtime
    public void SetBullet(GameObject _bulletPrefab)
    {
        // Set the new bullet prefab
        bulletPrefab = _bulletPrefab;
        // Repool the bullets
        PoolBullets();
    }

    // Change the muzzle flash prefab during runtime
    public void SetMuzzleFlash(GameObject _muzzleFlashPrefab)
    {
        // Set the new muzzle flash prefab
        muzzleFlashPrefab = _muzzleFlashPrefab;

        // If there are no blasters on this turret - return already (what's the point in continuing? =)
        if (_blasters.Length == 0) return;

        // Loop through all the child blasters of this turret
        foreach (BL_Blaster _b in _blasters)
        {
            // If there is a muzzle position configured in the inspector (all blasters should have a muzzle designated child object to indicate muzzle position...
            if (_b.muzzle != null)
            {
                // Destroy the first child of the muzzle transform, that should be the muzzle effect
                if (_b.muzzle.transform.childCount > 0) Destroy(_b.muzzle.transform.GetChild(0).gameObject);

                // If the new muzzle prefab object is a proper gameobject (it should be)...
                if (muzzleFlashPrefab != null)
                {
                    // Instantiate the muzzle prefab at the blaster transform position (we will soon move it)
                    GameObject _go = GameObject.Instantiate(muzzleFlashPrefab, _b.transform);

                    // Set the parent of the muzzle effect to the muzzle of the blaster
                    _go.transform.parent = _b.muzzle;

                    // Set the position of the muzzle effect to the position of the muzzle object
                    _go.transform.position = _b.muzzle.position;

                    // Set the rotation of the muzzle effect to align with the muzzle object (it should also align with blaster)
                    _go.transform.rotation = _b.muzzle.rotation;

                    // Refresh the muzzle flash effect of the blaster using the BL_Blaster script method
                    _b.RefreshMuzzleFlash();
                }
            }
        }
    }
}
