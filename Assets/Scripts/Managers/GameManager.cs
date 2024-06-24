using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void StartFunction();
public delegate void UpdateFunction(float deltaTime);
public delegate void DestroyFunction();

public class GameManager : MonoBehaviour
{
    protected static GameManager instance;
    public static GameManager Instance => instance;

    public static StartFunction ManagerStarts;
    public static StartFunction CharacterStarts;
    public static StartFunction BuildingStarts;
    public static StartFunction ControllerStarts;

    public static UpdateFunction ManagerUpdates;
    public static UpdateFunction CharacterUpdates;
    public static UpdateFunction BuildingUpdates;
    public static UpdateFunction ControllerUpdates;

    public static DestroyFunction ManagerDestroies;
    public static DestroyFunction CharacterDestroies;
    public static DestroyFunction BuildingDestroies;
    public static DestroyFunction ControllerDestroies;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
    }

    protected ControllerManager controllerManager;
    public ControllerManager ControllerManager => controllerManager;

    protected MiniMapManager miniMapManager;
    public MiniMapManager MiniMapManager => miniMapManager;

    protected OptionManager optionManager;
    public OptionManager OptionManager => optionManager;

    protected ResourceManager resourceManager;
    public ResourceManager ResourceManager => resourceManager;

    protected SaveManager saveManager;
    public SaveManager SaveManager => saveManager;

    protected SoundManager soundManager;
    public SoundManager SoundManager => soundManager;

    protected UIManager uiManager;
    public UIManager UIManager => uiManager;


    //protected NetworkManager networkManager;
    //public NetworkManager NetworkManager => networkManager;

    bool isGameStart;
    public static bool IsGameStart => instance && instance.isGameStart;

    IEnumerator Start()
    {
        resourceManager = new ResourceManager();
        yield return resourceManager.Initiate();
        soundManager = new SoundManager();
        yield return soundManager.Initiate();
        saveManager = new SaveManager();
        yield return saveManager.Initiate();
        optionManager = new OptionManager();
        yield return optionManager.Initiate();
        controllerManager = new ControllerManager();
        yield return controllerManager.Initiate();
        uiManager = new UIManager();
        yield return uiManager.Initiate();
        miniMapManager = new MiniMapManager();
        yield return miniMapManager.Initiate();

        ManagerUpdates += SoundManager.ManagerUpdate;
        ManagerUpdates += UIManager.ManagerUpdate;
        ManagerUpdates += MiniMapManager.ManagerUpdate;
        ManagerUpdates += ControllerManager.ManagerUpdate;

        isGameStart = true;
    }

    
    void Update()
    {
        if (!isGameStart) return;

        // 매니저 먼저
        if(ManagerStarts != null)
        {
            ManagerStarts.Invoke();
            ManagerStarts = null;
        }
        else
        {
            BuildingStarts?.Invoke();
            BuildingStarts = null;
            CharacterStarts?.Invoke();
            CharacterStarts = null;
            ControllerStarts?.Invoke();
            ControllerStarts = null;

            ManagerUpdates?.Invoke(Time.deltaTime);
            ControllerUpdates?.Invoke(Time.deltaTime);
            BuildingUpdates?.Invoke(Time.deltaTime);
            CharacterUpdates?.Invoke(Time.deltaTime);
            
        }

        ControllerDestroies?.Invoke();
        ControllerDestroies = null;
        CharacterDestroies?.Invoke();
        CharacterDestroies = null;
        BuildingDestroies?.Invoke();
        BuildingDestroies = null;
        ManagerDestroies?.Invoke();
        ManagerDestroies = null;
    }
}
