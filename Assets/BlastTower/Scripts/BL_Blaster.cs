using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BL_Blaster : MonoBehaviour {
    public enum State { IDLE, RECOIL, RETURN };

    [Header("BLASTER PARAMETERS")]
    // Must be referencing the muzzle position transform in the inspector
    public Transform muzzle;

    // Distance how far the blaster/barrel should recoil (set to 0 for no recoil)
    public float recoilDistance;
    
    // Be careful to change these, you may break the Rounds Per Minute calculations.
    // You can change the durations but it is recommended that you make sure they add up to 1.0 together.
    // recoil duration is how long of a portion of the time the blaster should kick back and the
    // return duration is how long of a portion of the time it should return to its original place.
    private float _recoilDuration = 0.1f;
    private float _returnDuration = 0.9f;    
    private float _recoilSpeed = 1.0f;

    // Private variables - should not need any alteration
    private float _timerRecoil;
    private Transform _transform;
    private Vector3 _orgLocalPosition;
    private Vector3 _recoilLocalPosition;
    private State _state;
    private BL_MuzzleFlash[] _muzzleFlashes;

	void Awake () {
        // reference the local transform
        _transform = gameObject.transform;

        // remember the original position (used for the interpolation LERP of recoil animation)
        _orgLocalPosition = _transform.localPosition;

        // Calculate what the recoild position is (used for the interpolation LERP of recoil animation)
        _recoilLocalPosition = _orgLocalPosition - new Vector3(0, 0, recoilDistance);

        // Set references to muzzle flashes children objects
        RefreshMuzzleFlash();
    }
	

    // Update is called once per frame
    void Update () {
        float _t  = 0;
        switch (_state)
        {            
            case State.RECOIL:                
                // The blaster is currently recoiling after firing - it interpolations with LERP between original position and recoil position
                _t = (Time.time - _timerRecoil) / (_recoilDuration/_recoilSpeed) ;
                _transform.localPosition = Vector3.Lerp(_orgLocalPosition, _recoilLocalPosition, _t / _recoilDuration);
                if (_t >= 1.0f)
                {
                    _state = State.RETURN;
                    _timerRecoil = Time.time;
                }
                break;

            case State.RETURN:
                // The blaster is currently returning to original position after firing - interpolating with LERP back to original position
                _t = (Time.time - _timerRecoil) / (_returnDuration/_recoilSpeed);
                _transform.localPosition = Vector3.Lerp(_recoilLocalPosition, _orgLocalPosition, _t / _returnDuration);
                if (_t >= 1.0f)
                {
                    _state = State.IDLE;
                    _timerRecoil = 0;
                }
                break;
        }
        
	}

    // Fire the blaster, start recoil animation and flash (the bullet firing is done in BL_Turret.cs)
    public void Fire(float _rateOfFire)
    {
        // The blaster is firing, initiate the recoil states
        _recoilSpeed = _rateOfFire / 60f;
        _state = State.RECOIL;
        _timerRecoil = Time.time;


        // If there are no muzzle flash references - make sure to refresh and look for children
        // This is done because the muzzle flashes can be replaced during runtime if dresired.
        if (_muzzleFlashes.Length == 0) RefreshMuzzleFlash();

        // Loop through each muzzle flash of this blaster and flash it as we're firing.
        foreach (BL_MuzzleFlash _mf in _muzzleFlashes)
        {
            if (_mf != null) _mf.Flash();            
        }
    }

    // Refresh muzzle flash children 
    public void RefreshMuzzleFlash()
    {      
        _muzzleFlashes = gameObject.GetComponentsInChildren<BL_MuzzleFlash>();
    }
}
