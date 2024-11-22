using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using TMPro;
using UnityEngine.UI;

public delegate void MoveDelegate(Vector3 dir);
public delegate void ScreenRotateDelegate(Vector2 mouseDelta);
public delegate bool DesignBuildingDelegate(int index);
public delegate bool BuildDelegate();
public delegate bool InteractionStartDelegate();
public delegate bool InteractionEndDelegate();
public delegate void WheelDelegate(Vector2 scrollDelta);
public delegate void CancelDelegate();
public delegate void FarmingDelegate(bool isFarming);
public delegate void KeyGuideDelegate();


public class ControllerBase : MyComponent
{
    public MoveDelegate             DoMove;
    public ScreenRotateDelegate     DoScreenRotate;
    public DesignBuildingDelegate   DoDesignBuilding;
    public BuildDelegate            DoBuild;
    public InteractionStartDelegate DoInteractionStart;
    public InteractionEndDelegate   DoInteractionEnd;
    public WheelDelegate            DoMouseWheel;
    public CancelDelegate           DoCancel;
    public FarmingDelegate          DoFarming;
    public KeyGuideDelegate         DoKeyGuide;

    [SerializeField]protected Player controlledPlayer;
    public Player ControlledPlayer => controlledPlayer;

    public PlayerRef myAuthority;
    [Networked] public int MyNumber { get; set; }

    protected override void OnEnable()
    {
        GameManager.ControllerStarts += MyStart;
        GameManager.ControllerUpdates += MyUpdate;
        
    }

    protected override void OnDisable()
    {
        GameManager.ControllerDestroies -= MyDestroy;
        GameManager.ControllerDestroies += MyDestroy;
        GameManager.ControllerUpdates -= MyUpdate;
        GameManager.ControllerStarts -= MyStart;
    }

    public override void Spawned()
    {
        if (GameManager.Instance.NetworkManager.LocalController == null)
        {
            LocalController[] controllers = GameObject.FindObjectsByType<LocalController>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            GameManager.Instance.NetworkManager.LocalController = System.Array.Find(controllers, target => target.GetComponent<NetworkObject>().HasInputAuthority == true);
            Debug.Log(GameManager.Instance.NetworkManager.LocalController.GetComponent<NetworkObject>().InputAuthority);
        }
        myAuthority = GetComponent<NetworkObject>().InputAuthority;
        // 테스트  
        if(HasInputAuthority)
        {
            GameObject characterUICanvas = GameManager.Instance.UIManager.GetUI(UIEnum.CharacterUICanvas);


            Button button = GameObject.FindGameObjectWithTag("WaveStartButton").GetComponent<Button>();
            if (HasStateAuthority)
            {
                button.onClick.AddListener(() => GameManager.Instance.WaveStart());
                button.onClick.AddListener(() => button.gameObject.SetActive(false));
            }
            else
            {
                button.gameObject.SetActive(false);
            }

            GameObject sessionName = GameObject.FindGameObjectWithTag("SessionIDText");
            sessionName.GetComponentInChildren<TextMeshProUGUI>().text = $"Session ID : {Runner.SessionInfo.Name}";
            Button sessionNameCopy = sessionName.GetComponentInChildren<Button>();
            GameObject checkMark = sessionNameCopy.GetComponentsInChildren<Image>()[1].gameObject;
            sessionNameCopy.onClick.AddListener(() => { 
                GUIUtility.systemCopyBuffer = Runner.SessionInfo.Name;
                checkMark.SetActive(true);
                StartCoroutine(TrashCode(checkMark));
            });
            checkMark.SetActive(false);

            GameManager.Instance.UIManager.GetUI(UIEnum.Minimap).GetComponentInChildren<Camera>();
        }
        Spawn(0, 0, 0);
    }

    IEnumerator TrashCode(GameObject wantObject)
    {
        yield return new WaitForSeconds(1);
        wantObject.SetActive(false);
    }

    public void Spawn(float dst_x, float dst_y, float dst_z)
    {
        if (controlledPlayer)
        {
            //여기로 이동 시키고
            //controlledPlayer.transform.position = new Vector3(dst_x, dst_y, dst_z);
        }
        else //없으면 
        {

            //만들기!
            if (HasStateAuthority)
            {
                NetworkObject inst = GameManager.Instance.NetworkManager.Runner.Spawn(ResourceManager.Get(ResourceEnum.Prefab.Player), new Vector3(dst_x, dst_y, dst_z), Quaternion.identity, myAuthority);
                //var spawndCharacter = FindAnyObjectByType<NetworkPhotonCallbacks>().SpawnedCharacter;
                //GameObject inst = GameManager.Instance.PoolManager.Instantiate(ResourceEnum.Prefab.Player, new Vector3(dst_x, dst_y, dst_z));
               
                //GameManager.Instance.InteractionManager.ControlledPlayer = controlledPlayer;
                //이 친구의 손 발을 움직이려면, 빙의를 해야 해요!
                //controlledPlayer.Possession(this);
            }
            
            Player[] players = GameObject.FindObjectsByType<Player>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            controlledPlayer = System.Array.Find(players, target => target.GetComponent<NetworkObject>().InputAuthority == myAuthority);
            Debug.Log($"myauth : {GameManager.Instance.NetworkManager.LocalController.myAuthority}");
            foreach (Player p in players) 
            {
                Debug.Log($"Player : {p.GetComponent<NetworkObject>().InputAuthority}");
            }
            controlledPlayer.Possession(this);
            
        }
    }

    public virtual void OnUnPossessionComplete(Player target) { }
    public virtual void OnPossessionComplete(Player target) { }
    public virtual void OnPossessionFailed(Player target) { }
}
