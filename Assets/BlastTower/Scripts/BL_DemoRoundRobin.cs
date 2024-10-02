using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Demo to show the effect of using Round Robin sounds with slight variations to avoid "machine gun" effect of artificial repeat
public class BL_DemoRoundRobin : MonoBehaviour {

    // Reference to UI text containing current sound effect
    public Text _sfxPlaying;
    
    // 5 arrays of different round robin examples to cycle through
    public AudioClip[] sfxRoundRobin1;
    public AudioClip[] sfxRoundRobin2;
    public AudioClip[] sfxRoundRobin3;
    public AudioClip[] sfxRoundRobin4;
    public AudioClip[] sfxRoundRobin5;

    // The current selection of array for round robin sounds
    public int currentSFX = 0;

    // Private variables
    private BL_Turret _turret;


    void Start () {
        // Upon start, find all turrets in scenes
        _turret = GameObject.FindObjectOfType<BL_Turret>();
        // Update the sound effects and UI
        Refresh();
	}
	
	// Update is called once per frame
	void Update () {
        // Update the UI with the current sound effect name of the blaster fire
        _sfxPlaying.text = _turret.GetCurrentSFXName();

        if (Input.GetKeyDown(KeyCode.PageDown) || Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            // Scroll wheel down or Page Down key selects next array of Round Robin sounds
            currentSFX++;
            if (currentSFX > 4) currentSFX = 0;

            // Refresh the turret with new array of sounds
            Refresh();
        }
        if (Input.GetKeyDown(KeyCode.PageUp) || Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            // Scroll wheel up or Page Up key selects previous array of Round Robin sounds
            currentSFX--;
            if (currentSFX < 0) currentSFX = 4;

            // Refresh the turret with new array of sounds
            Refresh();
        }
    }

    void Refresh()
    {
        // Set the selected array of round robin sound effects of the turret
        switch (currentSFX)
        {
            case 0:
                _turret.sfxFire = sfxRoundRobin1;
                break;
            case 1:
                _turret.sfxFire = sfxRoundRobin2;
                break;
            case 2:
                _turret.sfxFire = sfxRoundRobin3;
                break;
            case 3:
                _turret.sfxFire = sfxRoundRobin4;
                break;
            case 4:
                _turret.sfxFire = sfxRoundRobin5;
                break;
        }
    }
}
