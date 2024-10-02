using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This scripts allows a "player" to fire and aim turrets
public class BL_PlayerDemo : MonoBehaviour {

    // Use a custom texture for aiming instead of the normal pointer arrow
    public Texture2D cursorTexture;
    public CursorMode cursorMode = CursorMode.Auto;

    // Set the hot spot 8 pixels in and down (center of the 16 x 16 pixel curos included)
    public Vector2 hotSpot = new Vector2(8f, 8f);

    // Array of player controlled turrets
    public BL_Turret[] playerControlledTurrets;

    // Auto find all turrets in scene true/false
    public bool autoControlAllTurrets = true;

	void Start () {
        // If enabled, find all Turrets in scene
        if (autoControlAllTurrets)
        {
            // We locate the turrets based on object type, i.e. objects using the BL_Turret script
            playerControlledTurrets = GameObject.FindObjectsOfType<BL_Turret>();
        }

        // Set the custom cursor
        Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
    }
	
	void Update () {
        if (Input.GetButton("Fire1"))
        {
            // When fire button is pressed, loop through all player controlled turrets
            foreach (BL_Turret _t in playerControlledTurrets)
            {
                if (!Input.GetKey(KeyCode.LeftShift))
                {
                    // Fire with round robin enabled (step through the array of sounds for the blaster fire to allow alterations)
                    _t.Fire();
                }
                else
                {
                    // Fire with round robin disabled (play ony one sound repeat even if array contains multiple sounds)
                    _t.Fire(false);
                }
                
            }
        }

        // Perform a raycast hit for aiming - use the mouse position of the screen as a target to the ray cast from the camera
        RaycastHit _hit;      
        Ray _ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(_ray, out _hit, 4000))
        {
            // If we detect a hit, loop through all turrets
            foreach (BL_Turret _t in playerControlledTurrets)
            {
                // Use the public Aim() method of the turret to aim it at the raycast hit point
                // Note: if this doesn't work for you, it's likely because there is no object within the defined distance of the raycast
                // In the demo there is a distant verticle wall to provide a raycast hit. You can also use hidden colliders.
                _t.Aim(_hit.point);
            }            
        }

    }

    void OnMouseEnter()
    {
        // On mouse enter, show the custom mouse cursor
        Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
    }
    void OnMouseExit()
    {
        // On mouse exit, use the normal mouse cursor/pointer
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}
