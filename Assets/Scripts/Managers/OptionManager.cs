using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OptionManager : MonoBehaviour
{
    public static int cancelable = 0;
    // 마우스 감도.
    private float mouseSensitivity;

    //public override IEnumerator Initiate() { yield return null; }
    IEnumerator Start()
    {
        yield return null;
    }
    [SerializeField] GameObject settingsUICanvas;
    protected void OnESC(InputValue value)
    {
        if (settingsUICanvas != null && cancelable == 0)
        {
            settingsUICanvas.SetActive(!settingsUICanvas.activeInHierarchy);
        }
    }

    [SerializeField] Toggle masterToggle;
    [SerializeField] Toggle bgmToggle;
    [SerializeField] Toggle sfxToggle;
    [SerializeField] Slider masterSlider;
    [SerializeField] Slider bgmSlider;
    [SerializeField] Slider sfxSlider;
    
    
    public void ToggleMaster()
    {
        masterSlider.interactable = masterToggle.isOn;
        bgmToggle.interactable = masterToggle.isOn;
        sfxToggle.interactable = masterToggle.isOn;
        bgmSlider.interactable = masterToggle.isOn;
        sfxSlider.interactable = masterToggle.isOn;
        GameManager.Instance.SoundManager.ToggleAudioMixerGroup(SoundManager.AudioMixerGroupType.Master, masterToggle.isOn, masterSlider.value);
    }
    public void ToggleBGM()
    {
        bgmSlider.interactable = bgmToggle.isOn;
        GameManager.Instance.SoundManager.ToggleAudioMixerGroup(SoundManager.AudioMixerGroupType.BGM, bgmToggle.isOn, bgmSlider.value);
    }
    public void ToggleSFX()
    {
        sfxSlider.interactable = sfxToggle.isOn;
        GameManager.Instance.SoundManager.ToggleAudioMixerGroup(SoundManager.AudioMixerGroupType.SFX, sfxToggle.isOn, sfxSlider.value);
    }

    public void GameQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void GoTitle()
    {
        if (settingsUICanvas != null && cancelable == 0)
        {
            settingsUICanvas.SetActive(false);
        }

        GameManager.Instance.GoTitle();
        
        if (GameManager.Instance.IsDefeated)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
