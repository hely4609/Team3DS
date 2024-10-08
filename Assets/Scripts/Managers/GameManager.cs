using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

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
    public static UpdateFunction SoundUpdates;

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
            Destroy(gameObject);
        }
    }

    #region Managers
    protected ControllerManager controllerManager;
    public ControllerManager ControllerManager => controllerManager;

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

    protected NetworkManager networkManager;
    public NetworkManager NetworkManager => networkManager;

    protected WaveManager waveManager;
    public WaveManager WaveManager => waveManager;

    protected LocaleManager localeManager;
    public LocaleManager LocaleManager => localeManager;
    #endregion

    [SerializeField] bool isGameStart;
    public static bool IsGameStart => instance && instance.isGameStart;
    
    [SerializeField] bool isDefeated;
    public void Defeat() { isDefeated = true; }

    [Networked,SerializeField] public static float PlayTime { get; set; }
    [Networked,SerializeField] public static int KillCount { get; set; }

    public IEnumerator GameStart()
    {
        // 게임 시작후 Initiate할 매니저들
        if (poolManager == null)
        {
            poolManager = new PoolManager();
            yield return poolManager.Initiate();
        }

        if (buildingManager == null)
        {
            buildingManager = new BuildingManager();
        }
        yield return BuildingManager.Initiate();

        if (cameraManager == null)
        {
            cameraManager = new CameraManager();
            yield return CameraManager.Initiate();
            ManagerUpdates += CameraManager.ManagerUpdate;
        }

        if (waveManager == null)
        {
            waveManager = new WaveManager();
            yield return WaveManager.Initiate();
            ManagerUpdates += WaveManager.ManagerUpdate;
        }
        isGameStart = true; 
    }

    

    public void GameOver() 
    { 
        isGameStart = false;
        if (isDefeated) uiManager.GetUI(UIEnum.GameOverCanvas).GetComponent<GameOverCanvas>().SetResultText();
        PlayTime = 0;
        KillCount = 0;
        Cursor.lockState = CursorLockMode.None;
        
        NetworkManager.LocalController = null;

        ManagerStarts -= BuildingManager.ManagerStart;

        ManagerUpdates -= CameraManager.ManagerUpdate;
        ManagerUpdates -= WaveManager.ManagerUpdate;
        
        waveManager = null;
        cameraManager = null;
        buildingManager = null;
        poolManager = null;

        StartCoroutine(SeverInitiate());

        //networkManager = new NetworkManager();
        //networkManager.Initiate();

    }

    public void GoTitle()
    {
        networkManager.Runner.Shutdown();
    }

    public IEnumerator SeverInitiate()
    {
        // 게임 시작후 Initiate할 매니저들
        poolManager = new PoolManager();
        yield return poolManager.Initiate();
        buildingManager = new BuildingManager();
        //yield return BuildingManager.Initiate();
        cameraManager = new CameraManager();
        yield return CameraManager.Initiate();
        waveManager = new WaveManager();
        yield return WaveManager.Initiate();

        ManagerUpdates += CameraManager.ManagerUpdate;
        ManagerUpdates += WaveManager.ManagerUpdate;
    }

    LoadingCanvas loadingCanvas;


    IEnumerator Start()
    {
        loadingCanvas = GetComponentInChildren<LoadingCanvas>();
        localeManager = new LocaleManager();
        yield return localeManager.Initiate();

        resourceManager = new ResourceManager();
        yield return resourceManager.Initiate();
        soundManager = new SoundManager();
        yield return soundManager.Initiate();
        saveManager = new SaveManager();
        yield return saveManager.Initiate();
        //optionManager = new OptionManager();
        //yield return optionManager.Initiate();
        // 옵션 매니저는 UI를 끌어다 놓으려고 인스펙터창에 넣어 놓기로 했다.
        optionManager = GetComponent<OptionManager>();
        controllerManager = new ControllerManager();
        yield return controllerManager.Initiate();        
        uiManager = new UIManager();
        yield return uiManager.Initiate();
        networkManager = new NetworkManager();
        yield return networkManager.Initiate();

        //cameraManager = new CameraManager();
        //yield return cameraManager.Initiate();

        SoundUpdates += SoundManager.ManagerUpdate;
        ManagerUpdates += UIManager.ManagerUpdate;
        ManagerUpdates += ControllerManager.ManagerUpdate;


        CloseLoadInfo();
        SoundManager.Play(ResourceEnum.BGM.Silent_Partner__Whistling_Down_the_Road);
        SoundManager.Play(ResourceEnum.SFX.Wind, transform.position, true, out AudioSource temp);
        temp.spatialBlend = 0;

    }

    void FixedUpdate()
    {
        NetworkUpdates?.Invoke(Time.fixedDeltaTime);
        SoundUpdates?.Invoke(Time.fixedDeltaTime);
        if (!isGameStart) return;

        if (networkManager.Runner.IsServer) PlayTime += Time.fixedUnscaledDeltaTime;

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

        if (Input.GetKeyDown(KeyCode.R))
        {
            networkManager.Runner.Shutdown();
        }
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
