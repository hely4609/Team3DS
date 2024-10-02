using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Demo script that alters between different blasters
public class BL_Demo : MonoBehaviour {

    // Array of predefined rates of fire
    public int[] altRateOfFire = new int[] { 60, 120, 240, 480, 960 };    
    public int currentRateOfFire = 2;

    // Array of predefined bullet velocities
    public float[] altVelocity = new float[] { 50, 100, 200, 400, 800, 1600 };
    public int currentVelocity = 3 ;

    // Array of bullet types (prefabs)
    public GameObject[] altBulletType;
    public int currentBulletType = 0;

    // Array of impact types (prefabs)
    public GameObject[] altImpactType;
    public int currentImpactType;
    
    // Array of muzzle flash typues (prefab )
    public GameObject[] altMuzzleFlashType;
    public int currentMuzzleFlashType;

    // Array of sound effects
    public AudioClip[] altSFX;
    public int currentSFX = 0;

    // Array of camera positions
    public Transform[] altCameraPosition;
    public int currentCameraPosition = 0;

    // Fire blasters on turret sequentially (true) or simultaneously (false)
    public bool sequential = true;

    // Lock turret Y axis when aiming?
    public bool lockY = true;

    // Reference to UI text objects to update based on setting
    public Text uiRateOfFire;
    public Text uiVelocity;
    public Text uiSFX;
    public Text uiSequential;
    public Text uiLockY;

    // Private - reference to turrets in scene
    private BL_Turret[] _turrets;

    // Use this for initialization
    void Start () {        
        // Find all the turrets in the scene
        _turrets = GameObject.FindObjectsOfType<BL_Turret>();

        // Update the turret settings based on settings in this script and update UI
        Refresh(true);
    }
		
	void Update () {
        if (Input.GetKeyDown(KeyCode.W))
        {
            // Pressing W key increases the rate of fire (select next rate from the array of predefined rates)
            currentRateOfFire++;
            if (currentRateOfFire >= altRateOfFire.Length) currentRateOfFire = altRateOfFire.Length - 1;                        
            Refresh();
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            // Pressing Q key decreases the rate of fire (select previous rate from the array of predefined rates)
            currentRateOfFire--;
            if (currentRateOfFire < 0) currentRateOfFire = 0;            
            Refresh();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            // Pressing S key increases the bullet velocity (select next velocity from the array of predefined velocities)
            currentVelocity++;
            if (currentVelocity >= altVelocity.Length) currentVelocity = altVelocity.Length - 1;
            Refresh();
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            // Pressing A key decreases the bullet velocity (select previous velocity from the array of predefined velocities)
            currentVelocity--;
            if (currentVelocity < 0) currentVelocity = 0;
            Refresh();
        }

        if (Input.GetKeyDown(KeyCode.PageDown) || Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            // Mouse wheel down or Page Down selects the next sound effect from the array
            currentSFX++;
            if (currentSFX >= altSFX.Length) currentSFX = altSFX.Length - 1;
            // Bullet type is also changed
            currentBulletType++;            
            if (currentBulletType >= altBulletType.Length) currentBulletType = 0;
            // ...and so is Impact Type
            currentImpactType++;            
            if (currentImpactType >= altImpactType.Length) currentImpactType = 0;
            // ..and Muzzle Flash type
            currentMuzzleFlashType++;
            if (currentMuzzleFlashType >= altMuzzleFlashType.Length) currentMuzzleFlashType = 0;
            // Refresh the turrets with the new settings and update the UI
            Refresh(true);
        }
        if (Input.GetKeyDown(KeyCode.PageUp) || Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            // Mouse wheel up or Page Up selects the previous sound effect from the array
            currentSFX--;
            if (currentSFX < 0) currentSFX = 0;
            // Bullet type is also changed
            currentBulletType--;
            if (currentBulletType < 0) currentBulletType = altBulletType.Length - 1;
            // ...and so is Impact Type
            currentImpactType--;
            if (currentImpactType < 0) currentImpactType = altImpactType.Length -1;
            // ..and Muzzle Flash type
            currentMuzzleFlashType--;
            if (currentMuzzleFlashType < 0) currentMuzzleFlashType = altMuzzleFlashType.Length - 1;
            // Refresh the turrets with the new settings and update the UI
            Refresh(true);
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            // Pressing C rotates camera views from the array
            currentCameraPosition++;
            if (currentCameraPosition >= altCameraPosition.Length) currentCameraPosition = 0;
            Refresh();
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            // Pressing Z enables or disables sequential fire
            if (sequential)
            {
                sequential = false;
            }
            else
            {
                sequential = true;
            }
            // Update all turrets with the  with the new sequential settings
            foreach (BL_Turret _t in _turrets)
            {
                _t.fireSequential = sequential;
            }
            Refresh();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            // Pressing X toggles locking of Y Axis of turret
            if (lockY)
            {
                lockY = false;
            }
            else
            {
                lockY = true;
            }
            // Update all turrets with the  with the new lock Y axis setting
            foreach (BL_Turret _t in _turrets)
            {
                _t.lockYAxis = lockY;
            }
            Refresh();
        }

    }

    void Refresh(bool _repoolBullets = false)
    {
        // Refresh all turrets with new settings
        foreach (BL_Turret _t in _turrets)
        {
            // Set rate of fire to a new value from the array of predefined rates of fire
            _t.rateOfFire = altRateOfFire[currentRateOfFire];
            // Set the new bullet velocity
            _t.bulletVelocity = altVelocity[currentVelocity];
            // Repool all the bullets
            if (_repoolBullets)
            {
                // Set the new bullet type
                _t.SetBullet(altBulletType[currentBulletType]);
                // Set a new muzzle flash type
                _t.SetMuzzleFlash(altMuzzleFlashType[currentMuzzleFlashType]);
                // Set new impact type
                _t.SetImpact(altImpactType[currentImpactType]);
            }
            // Set the new sound effect (this only sets the first effect in the array as we are not using round robin in this demo
            if (_t.sfxFire.Length > 0) _t.sfxFire[0] = altSFX[currentSFX];
        }
        
        // Update UI with new settings
        uiRateOfFire.text = "Q/W RATE OF FIRE: " + altRateOfFire[currentRateOfFire];
        uiVelocity.text = "A/S VELOCITY: " + altVelocity[currentVelocity];
        uiSequential.text = "Z SEQUENTIAL: " + sequential.ToString().ToUpper();
        uiLockY.text = "X LOCK Y-AXIS: " + lockY.ToString().ToUpper();
        uiSFX.text = "MOUSE WHEEL SFX: " + altSFX[currentSFX].name.ToUpper();

        // Set the camera position and rotation
        Camera.main.transform.position = altCameraPosition[currentCameraPosition].position;
        Camera.main.transform.rotation = altCameraPosition[currentCameraPosition].rotation;
    }

}
