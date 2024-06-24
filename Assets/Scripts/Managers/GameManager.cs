using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    protected static GameManager instance;
    public static GameManager Instance => instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    protected ControllerManager controllerManager;
    public ControllerManager ControllerManager => controllerManager;

    protected BuildingManager buildingManager;
    public BuildingManager BuildingManager => buildingManager;

    protected MiniMapManager miniMapManager;
    public MiniMapManager MiniMapManager => miniMapManager;

    protected OptionManager optionManager;
    public OptionManager OptionManager => optionManager;

    protected PoolManager poolManager;
    public PoolManager PoolManager => poolManager;

    protected ResourceManager resourceManager;
    public ResourceManager ResourceManager => resourceManager;

    protected SaveManager saveManager;
    public SaveManager SaveManager => saveManager;

    protected SoundManager soundManager;
    public SoundManager SoundManager => soundManager;

    protected UIManager uiManager;
    public UIManager UIManager => uiManager;

    protected WaveManager waveManager;
    public WaveManager WaveManager => waveManager;

    //protected NetworkManager networkManager;
    //public NetworkManager NetworkManager => networkManager;


    IEnumerator Start()
    {
        return default;
    }

    
    void Update()
    {
        
    }
}
