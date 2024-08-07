using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void StartFunction();
public delegate void UpdateFunction(float deltaTime);
public delegate void DestroyFunction();

public class GameManager : MonoBehaviour
{
    [SerializeField]protected static GameManager instance;
    public static GameManager Instance => instance;

    public static StartFunction ManagerStarts;
    public static StartFunction ObjectStarts;
    public static StartFunction ControllerStarts;

    public static UpdateFunction ManagerUpdates;
    public static UpdateFunction ObjectUpdates;
    public static UpdateFunction ControllerUpdates;
    public static UpdateFunction NetworkUpdates;

    public static DestroyFunction ManagerDestroies;
    public static DestroyFunction ObjectDestroies;
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

    #region Managers
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

    protected PoolManager poolManager;
    public PoolManager PoolManager => poolManager;

    protected BuildingManager buildingManager;
    public BuildingManager BuildingManager => buildingManager;

    protected CameraManager cameraManager;
    public CameraManager CameraManager => cameraManager;

    protected InteractionManager interactionManager;
    public InteractionManager InteractionManager => interactionManager;

    protected NetworkManager networkManager;
    public NetworkManager NetworkManager => networkManager;

    protected WaveManager waveManager;
    public WaveManager WaveManager => waveManager;
    #endregion

    bool isGameStart;
    public static bool IsGameStart => instance && instance.isGameStart;
    public void GameStart() { isGameStart = true; }

    LoadingCanvas loadingCanvas;


    IEnumerator Start()
    {
        loadingCanvas = GetComponentInChildren<LoadingCanvas>();

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
        poolManager = new PoolManager();
        yield return poolManager.Initiate();
        uiManager = new UIManager();
        yield return uiManager.Initiate();
        miniMapManager = new MiniMapManager();
        yield return miniMapManager.Initiate();
        buildingManager = new BuildingManager();
        yield return buildingManager.Initiate();
        cameraManager = new CameraManager();
        yield return cameraManager.Initiate();
        interactionManager = new InteractionManager();
        yield return interactionManager.Initiate();
        networkManager = new NetworkManager();
        yield return networkManager.Initiate();
        waveManager = new WaveManager();
        yield return waveManager.Initiate();


        cameraManager = new CameraManager();
        yield return cameraManager.Initiate();

        ManagerUpdates += SoundManager.ManagerUpdate;
        ManagerUpdates += UIManager.ManagerUpdate;
        ManagerUpdates += MiniMapManager.ManagerUpdate;
        ManagerUpdates += ControllerManager.ManagerUpdate;
        ManagerUpdates += WaveManager.ManagerUpdate;

        ManagerUpdates += CameraManager.ManagerUpdate;
        ManagerUpdates += InteractionManager.ManagerUpdate;

        CloseLoadInfo();

    }

    void FixedUpdate()
    {
        NetworkUpdates?.Invoke(Time.fixedDeltaTime);
        if (!isGameStart) return;

        // 매니저 먼저
        if(ManagerStarts != null)
        {
            ManagerStarts.Invoke();
            ManagerStarts = null;
        }
        else
        {
            ObjectStarts?.Invoke();
            ObjectStarts = null;
            ControllerStarts?.Invoke();
            ControllerStarts = null;

            ManagerUpdates?.Invoke(Time.fixedDeltaTime);
            ControllerUpdates?.Invoke(Time.fixedDeltaTime);
            ObjectUpdates?.Invoke(Time.fixedDeltaTime);
            
        }

        ControllerDestroies?.Invoke();
        ControllerDestroies = null;
        ObjectDestroies?.Invoke();
        ObjectDestroies = null;
        ManagerDestroies?.Invoke();
        ManagerDestroies = null;
    }

    public static void ClaimLoadInfo(string info, int numerator = 0, int denominator = 1)
    {
        if(instance && instance.loadingCanvas)
        {
            instance.loadingCanvas.SetLoadInfo(info, numerator, denominator);
            instance.loadingCanvas.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("There is no GameManager or loadingCanvas");
        }
    }

    public static void CloseLoadInfo()
    {
        if (instance && instance.loadingCanvas)
        {
            instance.loadingCanvas.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("There is no GameManager or loadingCanvas");
        }

    }
}
