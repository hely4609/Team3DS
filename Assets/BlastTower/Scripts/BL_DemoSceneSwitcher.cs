using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Script to switch between the demo scenes
public class BL_DemoSceneSwitcher : MonoBehaviour
{
    // Private variables
    private bool _hideUI;

    void Start()
    {
        // Make this game object persistent (don't destroy it when switching scenes)
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {

        // Keys 1-5 switches between the different scenes
        if (Input.GetKey(KeyCode.Alpha1))
        {
            SceneManager.LoadScene("BL_Scene_Demo", LoadSceneMode.Single);
        }
        if (Input.GetKey(KeyCode.Alpha2))
        {
            SceneManager.LoadScene("BL_Scene_Demo_CrossFire", LoadSceneMode.Single);
        }
        if (Input.GetKey(KeyCode.Alpha3))
        {
            SceneManager.LoadScene("BL_Scene_Demo_Multiple", LoadSceneMode.Single);
        }
        if (Input.GetKey(KeyCode.Alpha4))
        {
            SceneManager.LoadScene("BL_Scene_Demo_RoundRobin", LoadSceneMode.Single);
        }
        if (Input.GetKey(KeyCode.Alpha5))
        {
            SceneManager.LoadScene("BL_Scene_Demo_Screenshots", LoadSceneMode.Single);
        }

        // Tab key hides/shows UI
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            // Find all canvases in the scene
            Canvas[] _canvases = GameObject.FindObjectsOfType<Canvas>();

            if (_hideUI)
            {               
                // If UI is hidden, loop through all canvases and enable them (show them)
                foreach (Canvas _c in _canvases)
                {
                    _c.enabled = true;
                }
                // Set the Hide UI to false (i.e. the UI is not hidden)
                _hideUI = false;
            }
            else
            {
                // If UI is not hidden, loop through all canvases and disable them (hide them)
                foreach (Canvas _c in _canvases)
                {
                    _c.enabled = false;
                }
                // Set the Hide UI to true (i.e. the UI is hidden)
                _hideUI = true;
            }
        }
    }
}
