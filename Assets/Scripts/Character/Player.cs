using Fusion;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

[Serializable]
public class InteractionButtonInfo
{
    public GameObject button;
    public IInteraction interactionObject;
    public Interaction interactionType;

    public InteractionButtonInfo(GameObject button, IInteraction interactionObject, Interaction interactionType)
    {
        this.button = button;
        this.interactionObject = interactionObject;
        this.interactionType = interactionType;
    }
}

public partial class Player : Character
{
    [SerializeField] protected ControllerBase possessionController;
    public ControllerBase PossesionController { get { return possessionController; } }
    [SerializeField] bool TPS_Mode;
    [SerializeField] protected Transform cameraOffset_FPS;
    [SerializeField] protected Transform cameraOffset_TPS;
    public Transform CameraOffset => TPS_Mode ? cameraOffset_TPS : cameraOffset_FPS;

    private ChangeDetector _changeDetector;
    /////////////////////////////interaction 변수들
    [SerializeField] protected Transform interactionUI; // 상호작용 UI위치
    [SerializeField] protected RectTransform interactionContent; // 상호작용대상 UI띄워줄 컨텐츠의 위치
    [SerializeField] protected GameObject interactionUpdateUI; // 상호작용 진행중 UI
    [SerializeField] public GameObject buildingSelectUI; // 빌딩 선택 UI
    public GameObject[] buildingSelectUIBuildingImages;
    [SerializeField] public GameObject buildingConfirmUI; // 가건물 들고있을때 설치 취소키 보여주는 UI
    public GameObject ropeMaxDistanceSignUI; // 로프 길이가 최대일 때 보여줄 경고 UI
    protected Animator onScreenKeyGuideUIAnim;

    protected ImgsFillDynamic interactionUpdateProgress; // 상호작용 진행중 UI 채울 정도
    protected GameObject mouseLeftImage; // 마우스좌클릭 Image
    protected TextMeshProUGUI buttonText; // 버튼에 띄워줄 text
    protected TextMeshProUGUI oreAmountText; // 가지고 있는 광물양을 보여줄 UI
    protected Image buttonImage;
    public TextMeshProUGUI pageIndexText;
    public TextMeshProUGUI leftRopeLengthText;
    public TextMeshProUGUI guidelineText;
    GameObject directPowerSupply;

    protected IInteraction interactionObject = null; // 내가 선택한 상호작용 대상
    public IInteraction InteractionObject => interactionObject;
    [SerializeField, Networked] protected int interactionIndex { get; set; } = -1; // 내가 선택한 상호작용 대상이 리스트에서 몇번째 인지
    protected List<IInteraction> interactionObjectList = new List<IInteraction>(); // 범위내 상호작용 가능한 대상들의 리스트
    protected Dictionary<IInteraction, GameObject> interactionObjectDictionary = new(); // 상호작용 가능한 대상들의 리스트와 버튼UI오브젝트를 1:1대응시켜줄 Dictionary 
    
    // test
    [SerializeField]protected List<InteractionButtonInfo> interactionButtonInfos = new List<InteractionButtonInfo>();
     
    [SerializeField, Networked] protected bool isInteracting { get; set; } // 나는 지금 상호작용 중인가?
    //public bool IsInteracting => isInteracting;
    [SerializeField, Networked] protected Interaction interactionType { get; set; } // 나는 어떤 상호작용을 하고있는가?

    [Networked] public int OreAmount { get; set; }

    /////////////////////////////
    [SerializeField] protected GameObject myMarker;
    [SerializeField] protected GameObject otherMarker;

    protected GameObject bePicked;
    public GameObject BePicked => bePicked;
    // protected bool isHandFree;
    protected ResourceEnum.Prefab[,] buildableEnumArray;
    public ResourceEnum.Prefab[,] BuildableEnumArray => buildableEnumArray;
    [Networked] public int BuildableEnumPageIndex { get; set; }
    [Networked] public Building DesigningBuilding { get; set; }
    [Networked] public bool IsThisPlayerCharacterUICanvasActivated { get; set; } = false;
    [Networked] public bool IsBuildingComfirmUIOpen { get; set; } = false;
    [Networked] public InteractableBuilding ropeBuilding { get; set; }
    [SerializeField, Networked] public bool CanSetRope { get; set; } = true;

    protected float rotate_x; // 마우스 이동에 따른 시점 회전 x값
    protected float rotate_y; // 마우스 이동에 따른 시점 회전 y값
    protected float mouseDelta_y; // 마우스 이동 변화량 y값

    [Networked] public Vector3 MoveDir { get; set; }
    protected Vector3 currentDir = Vector3.zero;
    Vector2 lastPos;
    //[Networked] bool AngleCheck { get; set; } = false;
    bool angleCheck;
    [Networked] public Vector3 angleCheckVector { get; set; }

    [Networked] public Vector3 PreviousPosition { get; set; }
    [Networked] public Quaternion PreviousRotation { get; set; }

    //public Vector3 interpolatedPosition;
    ////방향키 방향을 토대로 월드 방향을 구해봤어요!
    //Vector3 prefferedMoveDirection;
    ////평지 기준으로 움직이는 벡터입니다!
    //public Vector3 planeMovementVector;
    ////바닥 노말을 기준으로 움직이는 벡터입니다!
    //public Vector3 groundMovementVelocity;


    public bool TryPossession() => possessionController == null;

    protected void RegistrationFunctions(ControllerBase targetController)
    {
        targetController.DoMove -= Move;
        targetController.DoScreenRotate -= ScreenRotate;
        targetController.DoDesignBuilding -= DesignBuilding;
        targetController.DoBuild -= Build;
        targetController.DoInteractionStart -= InteractionStart;
        targetController.DoInteractionEnd -= InteractionEnd;
        targetController.DoMouseWheel -= MouseWheel;
        targetController.DoCancel -= Cancel;
        targetController.DoFarming -= Farming;
        targetController.DoKeyGuide -= SetKeyGuideUI;
        targetController.DoGreeting -= Greeting;
        targetController.DoChangeView -= ChangeView;

        targetController.DoMove += Move;
        targetController.DoScreenRotate += ScreenRotate;
        targetController.DoDesignBuilding += DesignBuilding;
        targetController.DoBuild += Build;
        targetController.DoInteractionStart += InteractionStart;
        targetController.DoInteractionEnd += InteractionEnd;
        targetController.DoMouseWheel += MouseWheel;
        targetController.DoCancel += Cancel;
        targetController.DoFarming += Farming;
        targetController.DoKeyGuide += SetKeyGuideUI;
        targetController.DoGreeting += Greeting;
        targetController.DoChangeView += ChangeView;
    }

    protected void UnRegistrationFunction(ControllerBase targetController)
    {
        targetController.DoMove -= Move;
        targetController.DoScreenRotate -= ScreenRotate;
        targetController.DoDesignBuilding -= DesignBuilding;
        targetController.DoBuild -= Build;
        targetController.DoInteractionStart -= InteractionStart;
        targetController.DoInteractionEnd -= InteractionEnd;
        targetController.DoMouseWheel -= MouseWheel;
        targetController.DoCancel -= Cancel;
        targetController.DoFarming -= Farming;
        targetController.DoKeyGuide -= SetKeyGuideUI;
        targetController.DoGreeting -= Greeting;
        targetController.DoChangeView -= ChangeView;
    }

    public virtual void Possession(ControllerBase targetController)
    {
        if (TryPossession() == false)
        {
            targetController.OnPossessionFailed(this);
            return;
        }

        possessionController = targetController;

        //함수를 전달만 하면 되는 거예요! -> 컨트롤러가 일하고 싶을 때
        //내 함수를 대신 실행해줘!
        RegistrationFunctions(possessionController);
        targetController.OnPossessionComplete(this);


    }
    public virtual void UnPossess()
    {
        if (possessionController == null) return;
        UnRegistrationFunction(possessionController);
        possessionController.OnUnPossessionComplete(this);
        possessionController = null;
    }

    protected override void MyStart()
    {
        CanSetRope = true;
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }

        if (cameraOffset_FPS == null)
        {
            cameraOffset_FPS = transform.Find("CameraOffset");
        }

        int xSize = (ResourceEnum.Prefab.buildingEnd - ResourceEnum.Prefab.buildingStart - 2) / 5 + 1;
        
        buildableEnumArray = new ResourceEnum.Prefab[xSize , 5];
        
        for (ResourceEnum.Prefab index = ResourceEnum.Prefab.buildingStart + 1; index < ResourceEnum.Prefab.buildingEnd; index++)
        {
            int y = index - (ResourceEnum.Prefab.buildingStart + 1);
            int x = y / 5;
            y %= 5;
            buildableEnumArray[x, y] = index;
        }


        if (HasInputAuthority) otherMarker.SetActive(false);
        else myMarker.SetActive(false);

        if (possessionController != null && HasInputAuthority)
        {
            interactionUI = GameObject.FindGameObjectWithTag("InteractionScrollView").transform;
            interactionContent = GameObject.FindGameObjectWithTag("InteractionContent").GetComponent<RectTransform>();
            interactionUpdateUI = GameObject.FindGameObjectWithTag("InteractionUpdateUI");
            buildingSelectUI = GameObject.FindGameObjectWithTag("BuildingSelectUI");
            buildingSelectUIBuildingImages = GameObject.FindGameObjectsWithTag("BuildingSelectUIBuildingImage");
            buildingConfirmUI = GameObject.FindGameObjectWithTag("BuildingConfirmUI");
            ropeMaxDistanceSignUI = GameObject.FindGameObjectWithTag("RopeMaxDistanceSignUI");
            oreAmountText = GameObject.FindGameObjectWithTag("OreText").GetComponent<TextMeshProUGUI>();
            onScreenKeyGuideUIAnim = GameObject.FindGameObjectWithTag("OnScreenKeyGuideUI").GetComponent<Animator>();
            pageIndexText = GameObject.FindGameObjectWithTag("PageIndexText").GetComponent<TextMeshProUGUI>();
            leftRopeLengthText = GameObject.FindGameObjectWithTag("LeftRopeLengthText").GetComponent<TextMeshProUGUI>();
            guidelineText = GameObject.FindGameObjectWithTag("GuidelineText").GetComponent<TextMeshProUGUI>();
            directPowerSupply = GameObject.FindGameObjectWithTag("DirectPowerSupply");

            interactionUpdateUI.SetActive(false);
            buildingSelectUI.SetActive(false);
            buildingConfirmUI.SetActive(false);
            ropeMaxDistanceSignUI.SetActive(false);
            leftRopeLengthText.gameObject.SetActive(false);
            if(!Runner.IsSinglePlayer) guidelineText.gameObject.SetActive(false);
            directPowerSupply.SetActive(false);

            GameManager.CloseLoadInfo();
        }
    }

    public override void Spawned()
    {
        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        if (footstepAudioSource != null) SoundManager.StopSFX(footstepAudioSource);
        if (cleanerAudioSource != null) SoundManager.StopSFX(cleanerAudioSource);
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            PreviousPosition = data.currentPosition;
            PreviousRotation = data.currentRotation;
        }
    }

    [Networked] bool NetworkIsWalking { get; set; } = false;
    [SerializeField] AudioSource footstepAudioSource;
    [Networked, SerializeField] ResourceEnum.SFX FootstepSFX { get; set; }
    ResourceEnum.SFX currentFootstep = ResourceEnum.SFX.None;
    [Networked] public bool NetworkIsFarmingPressed { get; set; } = false;
    bool isFarmingKeyAlreadyPressed = false;
    [SerializeField] AudioSource cleanerAudioSource;
    bool wasPlayingCleanerEnd;
    AudioSource ropeSource;

    private void Update()
    {
        if(directPowerSupply != null && directPowerSupply.activeSelf)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(GameManager.Instance.BuildingManager.supply2.transform.position);
            bool isOnScreen = screenPos.z > 0 && screenPos.x >= 0 && screenPos.x <= Screen.width && screenPos.y >= 0 && screenPos.y <= Screen.height;

            if(isOnScreen)
            {
                directPowerSupply.transform.SetPositionAndRotation(Camera.main.WorldToScreenPoint(GameManager.Instance.BuildingManager.supply2.transform.position), Quaternion.Euler(0, 0, 0));
            }
            else
            {
                if (screenPos.z < 0) screenPos *= -1;
                float clampedX = Mathf.Clamp(screenPos.x, 50, Screen.width - 50);
                float clampedY = Mathf.Clamp(screenPos.y, 50, Screen.height - 50);

                directPowerSupply.transform.position = new Vector3(clampedX, clampedY, 0); 
                Vector2 direction = (Camera.main.WorldToScreenPoint(GameManager.Instance.BuildingManager.supply2.transform.position) - Camera.main.transform.position).normalized;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                directPowerSupply.transform.rotation = Quaternion.Euler(0, 0, angle - 90);
            }
        }
    }
    protected override void MyUpdate(float deltaTime)
    {
        //이동방향이 있을 시 해당 방향으로 움직임. + 애니메이션 설정
        
        currentDir = new Vector3(Mathf.Lerp(currentDir.x, MoveDir.x, deltaTime), currentDir.y, Mathf.Lerp(currentDir.z, MoveDir.z, deltaTime));

        AnimFloat?.Invoke("Speed", MoveDir.magnitude);
        AnimFloat?.Invoke("MoveForward", currentDir.z);
        AnimFloat?.Invoke("MoveRight", currentDir.x);
        
        // 상호작용
        if (isInteracting && interactionObject != null)
        {
            // 건설 게이지 차오르기
            float progress = interactionObject.InteractionUpdate(deltaTime, Interaction.Build);

            if (possessionController != null && HasInputAuthority && interactionUpdateProgress != null)
                interactionUpdateProgress.SetValue(progress, true);

            if (progress >= 1f)
            {
                InteractionEnd();
            }
            // 건설 끝
        }

        ////////////////////////////
        if (DesigningBuilding != null && HasStateAuthority)
        {
            if (transform.rotation.eulerAngles.y < 45f || transform.rotation.eulerAngles.y >= 315f)
            {
                DesigningBuilding.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            }
            else if (transform.rotation.eulerAngles.y < 135f)
            {
                DesigningBuilding.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
            }
            else if (transform.rotation.eulerAngles.y < 225f)
            {
                DesigningBuilding.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            }
            else if (transform.rotation.eulerAngles.y < 315f)
            {
                DesigningBuilding.transform.rotation = Quaternion.Euler(0f, 270f, 0f);
            }

            Vector3 pickPos = transform.position + transform.forward * Mathf.Lerp(3.5f, 10f, Mathf.InverseLerp(30f + 13f / Mathf.Abs(GroundNormal.y), 0f, Mathf.Clamp(cameraOffset_FPS.localEulerAngles.x > 300f ? 0f : cameraOffset_FPS.localEulerAngles.x, 0f, 30f + 13f / Mathf.Abs(GroundNormal.y))));
            int x = (int)pickPos.x;
            int z = (int)pickPos.z;
            Vector2Int currentPos = new Vector2Int(x, z);

            // 건물위치에 변화가 생겼을 때 건물을 지을 수 있는 상태인지 체크함.
            if (DesigningBuilding.TiledBuildingPos != currentPos)
            {
                DesigningBuilding.transform.position = new Vector3(currentPos.x, DesigningBuilding.gameObject.transform.position.y, currentPos.y);
                DesigningBuilding.TiledBuildingPos = new Vector2Int((int)DesigningBuilding.transform.position.x, (int)DesigningBuilding.transform.position.z);
                DesigningBuilding.CheckBuild();
            }
        }
        if (ropeBuilding != null)
        {
            // 로프 끌기
            Vector3 ropePos = transform.position;
            int playerID = possessionController.MyNumber;
            int x = (int)Mathf.Round(ropePos.x);
            float y = ropePos.y;
            int z = (int)Mathf.Round(ropePos.z);
            Vector2 currentPos = new Vector2(x, z);
            Vector3 playerIntPos = new Vector3(x, y, z);
            if (currentPos != lastPos)
            {
                if(HasStateAuthority)CanSetRope = ropeBuilding.CheckRopeLength(playerIntPos, playerID);
                //if (CanSetRope)
                {
                    ropeBuilding.OnRopeSet(playerIntPos, playerID);
                    lastPos = currentPos;
                }
            }
        }

        //

        // 발소리
        if (NetworkIsWalking)
        {

            if (footstepAudioSource == null)
            {
                // 오디오 소스가 없는경우
                SoundManager.Play(FootstepSFX, transform.position, true, out footstepAudioSource);
                currentFootstep = FootstepSFX;
            }
            else if (FootstepSFX != currentFootstep)
            {
                // 오디오 소스는 있는데 현재 재생되고있는 바닥재질과 재생하고싶은 바닥재질이 다른경우
                SoundManager.StopSFX(footstepAudioSource);
                footstepAudioSource = null;
                SoundManager.Play(FootstepSFX, transform.position, true, out footstepAudioSource);
                currentFootstep = FootstepSFX;
            }
            else
            {
                footstepAudioSource.transform.position = transform.position;
            }

        }
        else if (footstepAudioSource != null)
        {
            // 안걷고있는경우
            SoundManager.StopSFX(footstepAudioSource);
            footstepAudioSource = null;
        }
        // 청소기 소리
        if (NetworkIsFarmingPressed)
        {
            if (!isFarmingKeyAlreadyPressed)
            {
                if (cleanerAudioSource != null)
                {
                    SoundManager.StopSFX(cleanerAudioSource);
                    wasPlayingCleanerEnd = false;
                }
                SoundManager.Play(ResourceEnum.SFX.cleaner_start, transform.position, false, out cleanerAudioSource);
                isFarmingKeyAlreadyPressed = true;
            }
        }
        else
        {
            if (isFarmingKeyAlreadyPressed)
            {
                if (cleanerAudioSource != null)
                {
                    SoundManager.StopSFX(cleanerAudioSource);
                    SoundManager.Play(ResourceEnum.SFX.cleaner_end, transform.position, false, out cleanerAudioSource);
                    wasPlayingCleanerEnd = true;
                }
                isFarmingKeyAlreadyPressed = false;

            }
        }
        if (cleanerAudioSource != null)
        {
            cleanerAudioSource.transform.position = transform.position;

            if (!cleanerAudioSource.isPlaying)
            {
                if (wasPlayingCleanerEnd)
                {
                    cleanerAudioSource = null;
                    wasPlayingCleanerEnd = false;
                }
                else
                {
                    SoundManager.Play(ResourceEnum.SFX.cleaner_loop, transform.position, true, out cleanerAudioSource);

                }
            }
        }
    }

    public Vector3 MoveDirCalculrate(Vector3 input)
    {
        return (transform.forward * input.z + transform.right * input.x).normalized * moveSpeed;
    }

    // 키보드 입력으로 플레이어 이동방향을 결정하는 함수.
    public override void Move(Vector3 direction)
    {
       
        MoveDir = direction.normalized;

        if (possessionController != null && HasInputAuthority)
        {
            
            Vector3 wantMoveDir = transform.forward * MoveDir.z + transform.right * MoveDir.x;
            Vector3 velocity = CalculrateNextFrameGroundAngle() < 85f ? wantMoveDir : Vector3.zero;
            Vector3 gravity = IsGround ? Vector3.zero : Vector3.down * Mathf.Abs(rb.velocity.y);
            Vector3 boundaryLeftUp = GameManager.Instance.WaveManager.BoundLeftUp;
            Vector3 boundaryRightDown = GameManager.Instance.WaveManager.BoundRightDown;

            
            if (IsGround)
            {
                velocity = Vector3.ProjectOnPlane(wantMoveDir, GroundNormal);

                if (direction.magnitude > 0)
                {
                    NetworkIsWalking = true;
                }
                else
                {
                    NetworkIsWalking = false;
                }

            }

            if (!IsGround) velocity *= 0.1f;

            if (CanSetRope)
            {
                rb.velocity = velocity * moveSpeed + gravity;
            }
            else 
            {
                angleCheck = Vector3.Angle(velocity, angleCheckVector - transform.position) < 45;

                // CanSetRope가 false이면 로프 회수만 가능하게
                //if(HasStateAuthority)
                //{
                //    if(ropeBuilding is Pylon)
                //    {
                //        Pylon ropePylon = ropeBuilding as Pylon;
                //        AngleCheck = Vector3.Angle(velocity, ropePylon.MultiTabList[possessionController.MyNumber].ropePositions[ropePylon.MultiTabList[possessionController.MyNumber].ropePositions.Count - 2] - transform.position) < 45.0f;
                //    }
                //    else if (ropeBuilding != null)
                //    {
                //        AngleCheck = Vector3.Angle(velocity, ropeBuilding.RopeStruct.ropePositions[ropeBuilding.RopeStruct.ropePositions.Count - 2] - transform.position) < 45.0f;
                //    }

                //}

                if (angleCheck)
                    rb.velocity = velocity * moveSpeed + gravity;
                else
                    rb.velocity = Vector3.zero;
            }
        }
        else
        {
            rb.position = PreviousPosition;
        }
    }

    public virtual void ScreenRotate(Vector2 mouseDelta)
    {
        if (HasInputAuthority)
        {
            //좌우회전은 캐릭터를 회전
            rotate_y = transform.eulerAngles.y + mouseDelta.x * 0.02f * 10f;
            transform.localEulerAngles = new Vector3(0f, rotate_y, 0f);

            mouseDelta_y = -mouseDelta.y * 0.02f * 10f;
            rotate_x += mouseDelta_y;
            rotate_x = Mathf.Clamp(rotate_x, -45f, 30f + 13f / Mathf.Abs(GroundNormal.y));

            
            // 상하회전은 카메라만 회전
            cameraOffset_FPS.localEulerAngles = new Vector3(rotate_x, 0f, 0f);
        }
    }

    

    public bool PickUp(GameObject target) { return default; }
    public bool PutDown() { return default; }

    // 건설한 건물을 반투명(가건물) 상태로 만드는 함수
    public bool DesignBuilding(int index)
    {
        //if (index < 0 || buildableEnumArray[buildableEnumPageIndex, index] == 0) return false;

        //designingBuilding = GameManager.Instance.PoolManager.Instantiate(buildableEnumArray[buildableEnumPageIndex, index]).GetComponent<Building>();
        //buildingSelectUI.SetActive(false);
        //return true;

        if (index < 0 || buildableEnumArray[BuildableEnumPageIndex, index] == 0) return false;

        if (ResourceManager.Get(buildableEnumArray[BuildableEnumPageIndex, index]).GetComponent<Building>() is BlastTower)
        {
            return false;
        }

        if (ResourceManager.Get(buildableEnumArray[BuildableEnumPageIndex, index]).GetComponent<Building>().Cost > GameManager.Instance.BuildingManager.supply.TotalOreAmount)
        {
            Debug.Log("건설에 필요한 광물이 부족합니다.");
            return false;
        } 
        

        NetworkObject building = GameManager.Instance.NetworkManager.Runner.Spawn(ResourceManager.Get(buildableEnumArray[BuildableEnumPageIndex, index]));
        DesigningBuilding = building.GetComponent<Building>();
        GameManager.Instance.BuildingManager.supply.TotalOreAmount -= DesigningBuilding.Cost;
        IsBuildingComfirmUIOpen = true;

        return true;

    }

    public bool Build()
    {
        if (DesigningBuilding == null)
        {
            IsThisPlayerCharacterUICanvasActivated = !IsThisPlayerCharacterUICanvasActivated;
            return true;
        }
        return false;
    }

    public bool Repair(EnergyBarrierGenerator target) { return default; }

    public void Cancel()
    {
        if (HasInputAuthority && buildingSelectUI.activeSelf)
        {
            buildingSelectUI.SetActive(false);
        }
        IsThisPlayerCharacterUICanvasActivated = false;

        if (DesigningBuilding != null)
        {
            IsBuildingComfirmUIOpen = false;
            GameManager.Instance.BuildingManager.supply.TotalOreAmount += DesigningBuilding.Cost;
            Runner.Despawn(DesigningBuilding.GetComponent<NetworkObject>());
        }

        if (ropeBuilding != null)
        {
            ropeBuilding.ResetRope(this, possessionController.MyNumber);
            ropeBuilding = null;
        }

        if(ropeMaxDistanceSignUI != null)
        {
            ropeMaxDistanceSignUI.SetActive(false);
        }

    }

    public void Farming(bool isFarming)
    {
        AnimIK?.Invoke(isFarming);
    }

    public bool InteractionStart()
    {
        if (DesigningBuilding != null)
        {
            if (DesigningBuilding.FixPlace())
            {
                IsBuildingComfirmUIOpen = false;
                DesigningBuilding = null;
                if (Runner.IsSinglePlayer) guidelineText.text = "Click (hold) the temporary building to complete the building.";
                return true;
            }

            return default;
        }
        if (interactionObject == null) return false;

        interactionObject.InteractionStart(this, interactionType);

        switch (interactionType)
        {
            default: InteractionEnd(); break;
            case Interaction.OnOff:
                if (Runner.IsSinglePlayer) guidelineText.text = "If everything is ready, press \"Wave Start\" button to start the game.";
                break;
            case Interaction.Build:
                if (HasStateAuthority) isInteracting = true;
                AnimBool?.Invoke("isBuild", true);

                if (HasInputAuthority)
                {
                    interactionUI.gameObject.SetActive(false);
                    interactionUpdateUI.SetActive(true);
                    interactionUpdateProgress = interactionUpdateUI.GetComponentInChildren<ImgsFillDynamic>();
                    buttonText = interactionUpdateUI.GetComponentInChildren<TextMeshProUGUI>();
                    buttonText.GetComponentInChildren<LocalizeStringEvent>().StringReference.SetReference("ChangeableTable", "NowBuilding");

                }

                //GameManager.Instance.PoolManager.Instantiate(ResourceEnum.Prefab.Hammer, sockets.FindSocket("RightHand").gameObject.transform);
                break;
        }

        return default;
    }

    public bool InteractionEnd()
    {
        if (HasStateAuthority) isInteracting = false;
        
        switch (interactionType)
        {
            default:
                //interactionType = Interaction.None;

                break;
            case Interaction.Demolish:
                //RenewalInteractionUI(interactionObject, false);
                break;
            case Interaction.OnOff:
                break;
            case Interaction.AttachRope:
                if (Runner.IsSinglePlayer)
                {
                    guidelineText.text = "Click on the tower to turn on the tower.";
                    directPowerSupply.SetActive(false);
                }
                break;
            case Interaction.Upgrade:
                //RenewalInteractionUI(interactionObject);
                break;
            case Interaction.Deliver:
                OreAmount = 0;
                break;
            case Interaction.Build:
                AnimBool?.Invoke("isBuild", false);
                if (HasInputAuthority)
                {
                    interactionUI.gameObject.SetActive(true);
                    interactionUpdateUI.SetActive(false);
                    interactionUpdateProgress = null;
                    if (Runner.IsSinglePlayer)
                    {
                        guidelineText.text = "Connect the wires to supply power from power supply.\r\nIf the wire is short, try building an pylon(5).";
                        directPowerSupply.SetActive(true);
                    }
                }
                //GameManager.Instance.PoolManager.Destroy(sockets.FindSocket("RightHand").gameObject.GetComponentInChildren<PoolingInfo>());
                break;

        }

        //interactionObject = null;

        if (interactionObject == null) return false;
        bool result = interactionObject.InteractionEnd(this, interactionType);

        return result;
    }

    // 상호작용 가능한 대상이 감지되었을 때 처리
    private void OnTriggerEnter(Collider other)
    {
        IInteraction target;

        if (!other.TryGetComponent(out target))
        {
            target = other.GetComponentInParent<IInteraction>();
            if (target == null) return;
        }

        // 이미 있다면 추가하지않음
        //if (interactionObjectList.Exists(inst => inst == target)) return;
        if (interactionButtonInfos.Exists(inst => inst.interactionObject == target)) return;

        if (System.Array.Find(target.GetInteractionColliders(), col => other == col))
        {
            List<Interaction> interactions = target.GetInteractions(this);

            foreach (var interaction in interactions)
            {
                GameObject button = GameManager.Instance.PoolManager.Instantiate(ResourceEnum.Prefab.InteractableObjButton, interactionContent);
                GameObject resource = button.transform.GetChild(1).gameObject;
                buttonText = resource.GetComponentInChildren<TextMeshProUGUI>();
                Image[] images = resource.GetComponentsInChildren<Image>();
                buttonImage = images[1];

                switch (interaction)
                {
                    case Interaction.Demolish:
                        
                        int cost = 0;
                        if (target as Tower)
                        {
                            Tower tower = (Tower)target;
                            if (tower.CompletePercent < 1) cost = tower.Cost;
                            else cost = (int)((tower.Cost + tower.TotalUpgradeCost) * 0.7f + 0.5f);
                        }
                        else
                        {
                            cost = ((Building)target).Cost;
                        }
                        
                        buttonText.text = "+" + $" {cost}";
                        buttonText.color = Color.green;

                        buttonImage.sprite = ResourceManager.Get(ResourceEnum.Sprite.Ore);
                        break;

                    case Interaction.Upgrade:
                        
                        cost = 0;
                        if (target as PowerSupply) cost = ((PowerSupply)target).ExpMax;
                        else if (target as Tower) cost = ((Tower)target).UpgradeRequire;

                        buttonText.text = "-" + $" {cost}";
                        buttonText.color = Color.red;

                        buttonImage.sprite = ResourceManager.Get(ResourceEnum.Sprite.Ore);
                        break;

                    case Interaction.OnOff:

                        if (target as Tower)
                        {
                            Tower tower = (Tower)target;
                            cost = tower.powerConsumption;

                            buttonText.text = tower.OnOff? "+" + $" {cost}" : "-" + $" {cost}";
                            buttonText.color = tower.OnOff? Color.green : Color.red;
                        }

                        buttonImage.sprite = ResourceManager.Get(ResourceEnum.Sprite.Battery);
                        break;

                    default:
                        resource.SetActive(false);
                        break;
                }
                

                button.transform.SetSiblingIndex(9999);  // SiblingIndex - 나는 부모의 자식중에 몇번째 Index에 있는가
                buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
                //button.GetComponentInChildren<LocalizeStringEvent>().StringReference.SetReference("ChangeableTable", target.GetName());
                buttonText.text = $"{GameManager.Instance.LocaleManager.LocaleNameSet(target.GetName())}({GameManager.Instance.LocaleManager.LocaleNameSet(interaction.ToString())})";
                //interactionObjectDictionary.Add(target, button);

                interactionButtonInfos.Add(new InteractionButtonInfo(button, target, interaction));

                if (interactionButtonInfos.Count == 1)
                {
                    if (HasStateAuthority)
                    {
                        interactionIndex = 0;
                        interactionType = interaction;
                    } 
                    interactionObject = target;
                    
                    //if (HasInputAuthority)
                    //{
                    //    mouseLeftImage = GameManager.Instance.PoolManager.Instantiate(ResourceEnum.Prefab.MouseLeftUI, interactionUI);
                    //    //Canvas.ForceUpdateCanvases();
                    //}
                }

                //UpdateInteractionUI(interactionIndex);
            }

            //interactionObjectList.Add(target);
            

            //GameObject button = Instantiate(ResourceManager.Get(ResourceEnum.Prefab.InteractableObjButton), interactionContent);
            
        }

    }

    // 상호작용 가능한 대상 리스트에 있는 대상이 감지범위에서 나갔을 때 처리
    private void OnTriggerExit(Collider other)
    {
        IInteraction target;

        if (!other.TryGetComponent(out target))
        {
            target = other.GetComponentInParent<IInteraction>();
            if (target == null) return;
        }

        if (System.Array.Find(target.GetInteractionColliders(), col => other == col))
        {
            //interactionObjectList.Remove(target);

            List<InteractionButtonInfo> removeInfos = interactionButtonInfos.FindAll(obj => obj.interactionObject == target);

            foreach (InteractionButtonInfo info in removeInfos)
            {
                info.button.transform.GetChild(1).gameObject.SetActive(true);
                GameManager.Instance.PoolManager.Destroy(info.button);
                interactionButtonInfos.Remove(info);
            }

            
            //if (interactionObjectDictionary.TryGetValue(target, out GameObject result))
            //{
            //    GameManager.Instance.PoolManager.Destroy(result);
            //    interactionObjectDictionary.Remove(target);
            //}

            if (interactionButtonInfos.Count == 0)
            {
                if(HasStateAuthority)interactionIndex = -1;
                if (isInteracting)
                {
                    InteractionEnd();
                }
                interactionObject = null;
                if (HasStateAuthority) interactionType = Interaction.None;

                if (HasInputAuthority)
                {
                    GameManager.Instance.PoolManager.Destroy(mouseLeftImage);
                }
            }
            else
            {
                if (HasStateAuthority) interactionIndex = Mathf.Min(interactionIndex, interactionButtonInfos.Count - 1);
                if (isInteracting && target == interactionObject)
                {
                    InteractionEnd();
                }
                interactionObject = interactionButtonInfos[interactionIndex].interactionObject;
                if (HasStateAuthority) interactionType = interactionButtonInfos[interactionIndex].interactionType;

            }

            //UpdateInteractionUI(interactionIndex);
        }

    }

    // 마우스 휠을 굴려서 상호작용할 대상을 정함.
    public void MouseWheel(Vector2 scrollDelta)
    {
        //if (interactionContent == null || interactionContent.gameObject.activeInHierarchy == false) return;
        if (interactionButtonInfos.Count == 0) return;

        int index = interactionIndex;

        if (scrollDelta.y == 0f) return;
        // 휠을 위로 굴렸을 때
        else if (scrollDelta.y > 0)
        {
            if (HasStateAuthority)
            {
                index--;
                interactionIndex = Mathf.Max(index, 0);
            }
            Debug.Log(interactionIndex);
            if(interactionIndex > -1) interactionObject = interactionButtonInfos[interactionIndex].interactionObject;
            if (HasStateAuthority) interactionType = interactionButtonInfos[interactionIndex].interactionType;

            if (interactionIndex < interactionButtonInfos.Count - 4 && HasInputAuthority)
            {
                interactionContent.anchoredPosition -= new Vector2(0, 50f);
                interactionContent.anchoredPosition = new Vector2(0, Mathf.Clamp(interactionContent.anchoredPosition.y, 0, (interactionButtonInfos.Count - 6) * 50f));
            }
        }
        // 휠을 아래로 굴렸을 때
        else if (scrollDelta.y < 0)
        {
            if (HasStateAuthority)
            {
                index++;
                interactionIndex = Mathf.Min(interactionButtonInfos.Count - 1, index);
            }
            Debug.Log(interactionIndex);
            if (interactionIndex > -1 && interactionIndex < interactionButtonInfos.Count) interactionObject = interactionButtonInfos[interactionIndex].interactionObject;
            if (HasStateAuthority) interactionType = interactionButtonInfos[interactionIndex].interactionType;

            if (interactionIndex > 4 && HasInputAuthority)
            {
                interactionContent.anchoredPosition += new Vector2(0, 50f);
                interactionContent.anchoredPosition = new Vector2(0, Mathf.Clamp(interactionContent.anchoredPosition.y, 0, (interactionButtonInfos.Count - 6) * 50f));
            }
        }
        //UpdateInteractionUI(interactionIndex);
    }

    // 상호작용 UI를 최신화하는 함수
    public void UpdateInteractionUI(int targetIndex)
    {
        Canvas.ForceUpdateCanvases();

        for (int i = 0; i < interactionButtonInfos.Count; i++)
        {
            GameObject button = interactionButtonInfos[i].button;
            Image buttonImage = button.GetComponentInChildren<Image>();
            if (targetIndex == i)
            {
                buttonImage.color = Color.blue;
                if (HasInputAuthority)
                {
                    mouseLeftImage.transform.position = button.transform.position;
                }
            }
            else buttonImage.color = Color.white;
        }

        Canvas.ForceUpdateCanvases();
    }

    public void RenewalInteractionUI(IInteraction target, bool isRenewal = true)
    {
        int index = interactionIndex;

        if (isRenewal)
        {
            //if (isInteracting)
            //{
            //    InteractionEnd();
            //    return;
            //} 

            List<InteractionButtonInfo> removeInfos = interactionButtonInfos.FindAll(obj => obj.interactionObject == target);
            if (removeInfos.Count == 0) return;

            foreach (InteractionButtonInfo info in removeInfos)
            {
                info.button.transform.GetChild(1).gameObject.SetActive(true);
                interactionButtonInfos.Remove(info);
                GameManager.Instance.PoolManager.Destroy(info.button);
            }

            List<Interaction> interactions = target.GetInteractions(this);

            foreach (var interaction in interactions)
            {
                GameObject button = GameManager.Instance.PoolManager.Instantiate(ResourceEnum.Prefab.InteractableObjButton, interactionContent);
                GameObject resource = button.transform.GetChild(1).gameObject;
                buttonText = resource.GetComponentInChildren<TextMeshProUGUI>();
                Image[] images = resource.GetComponentsInChildren<Image>();
                buttonImage = images[1];

                switch (interaction)
                {
                    case Interaction.Demolish:

                        int cost = 0;
                        if (target as Tower)
                        {
                            Tower tower = (Tower)target;
                            if (tower.CompletePercent < 1) cost = tower.Cost;
                            else cost = (int)((tower.Cost + tower.TotalUpgradeCost) * 0.7f + 0.5f);
                        }
                        else
                        {
                            cost = ((Building)target).Cost;
                        }

                        buttonText.text = "+" + $" {cost}";
                        buttonText.color = Color.green;

                        buttonImage.sprite = ResourceManager.Get(ResourceEnum.Sprite.Ore);
                        break;

                    case Interaction.Upgrade:

                        cost = 0;
                        if (target as PowerSupply) cost = ((PowerSupply)target).ExpMax;
                        else if (target as Tower) cost = ((Tower)target).UpgradeRequire;

                        buttonText.text = "-" + $" {cost}";
                        buttonText.color = Color.red;

                        buttonImage.sprite = ResourceManager.Get(ResourceEnum.Sprite.Ore);
                        break;

                    case Interaction.OnOff:

                        if (target as Tower)
                        {
                            Tower tower = (Tower)target;
                            cost = tower.powerConsumption;

                            buttonText.text = tower.OnOff ? "+" + $" {cost}" : "-" + $" {cost}";
                            buttonText.color = tower.OnOff ? Color.green : Color.red;
                        }

                        buttonImage.sprite = ResourceManager.Get(ResourceEnum.Sprite.Battery);
                        break;

                    default:
                        resource.SetActive(false);
                        break;
                }

                button.transform.SetSiblingIndex(9999);  // SiblingIndex - 나는 부모의 자식중에 몇번째 Index에 있는가
                buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
                //button.GetComponentInChildren<LocalizeStringEvent>().StringReference.SetReference("ChangeableTable", interactionObject.GetName());
                buttonText.text = $"{GameManager.Instance.LocaleManager.LocaleNameSet(target.GetName())}({GameManager.Instance.LocaleManager.LocaleNameSet(interaction.ToString())})";
                //interactionObjectDictionary.Add(interactionObject, button);

                interactionButtonInfos.Add(new InteractionButtonInfo(button, target, interaction));

                //if (interactionButtonInfos.Count == 1)
                //{
                //    interactionIndex = 0;
                //    interactionType = interaction;
                //    //if (HasInputAuthority)
                //    //{
                //    //    mouseLeftImage = GameManager.Instance.PoolManager.Instantiate(ResourceEnum.Prefab.MouseLeftUI, interactionUI);
                //    //    Canvas.ForceUpdateCanvases();
                //    //}
                //}

                //UpdateInteractionUI(interactionIndex);

            }

            if (interactionButtonInfos.Count > 0)
            {
                if (HasStateAuthority)
                {
                    interactionIndex = Mathf.Clamp(index, 0, interactionButtonInfos.Count - 1);
                    interactionType = interactionButtonInfos[interactionIndex].interactionType;
                } 
                
                //UpdateInteractionUI(interactionIndex);
            }
        }
        else
        {
            List<InteractionButtonInfo> removeInfos = interactionButtonInfos.FindAll(obj => obj.interactionObject == target);
            if (removeInfos.Count == 0) return;

            foreach (InteractionButtonInfo info in removeInfos)
            {
                interactionButtonInfos.Remove(info);
                if(GameManager.Instance.NetworkManager.LocalController.ControlledPlayer == this) GameManager.Instance.PoolManager.Destroy(info.button);
            }

            if (interactionButtonInfos.Count == 0)
            {
                if (HasStateAuthority) interactionIndex = -1;
                if (isInteracting)
                {
                    InteractionEnd();
                }
                interactionObject = null;
                if (HasStateAuthority) interactionType = Interaction.None;

                if (HasInputAuthority)
                {
                    GameManager.Instance.PoolManager.Destroy(mouseLeftImage);
                }
            }
            else
            {
                if (HasStateAuthority) interactionIndex = Mathf.Min(index, interactionButtonInfos.Count - 1);
                if (isInteracting && target == interactionObject)
                {
                    InteractionEnd();
                }
                interactionObject = interactionButtonInfos[interactionIndex].interactionObject;
                if (HasStateAuthority) interactionType = interactionButtonInfos[interactionIndex].interactionType;

            }

            //UpdateInteractionUI(interactionIndex);
        }

        if(interactionIndex == index) UpdateInteractionUI(index);

    }

    //public void RenewalInteractionUI(IInteraction target)
    //{
    //    List<InteractionButtonInfo> removeInfos = interactionButtonInfos.FindAll(obj => obj.interactionObject == target);

    //    foreach (InteractionButtonInfo info in removeInfos)
    //    {
    //        GameManager.Instance.PoolManager.Destroy(info.button);
    //        interactionButtonInfos.Remove(info);
    //    }

    //    if (interactionButtonInfos.Count == 0)
    //    {
    //        interactionIndex = -1;
    //        if (isInteracting)
    //        {
    //            InteractionEnd();
    //        }
    //        interactionObject = null;
    //        interactionType = Interaction.None;

    //        if (HasInputAuthority)
    //        {
    //            GameManager.Instance.PoolManager.Destroy(mouseLeftImage);
    //        }
    //    }
    //    else
    //    {
    //        interactionIndex = Mathf.Min(interactionIndex, interactionButtonInfos.Count - 1);
    //        if (isInteracting && target == interactionObject)
    //        {
    //            InteractionEnd();
    //        }
    //        interactionObject = interactionButtonInfos[interactionIndex].interactionObject;
    //        interactionType = interactionButtonInfos[interactionIndex].interactionType;

    //    }

    //    UpdateInteractionUI(interactionIndex);
    //}



    public override void Render()
    {
        foreach (var change in _changeDetector.DetectChanges(this, out var previousBuffer, out var currentBuffer))
        {

            switch (change)
            {
                case nameof(PreviousRotation):
                    rb.rotation = PreviousRotation;
                    break;
                case nameof(IsGround):
                    bool isUseGravity;
                    isUseGravity = IsGround ? false : true;
                    rb.useGravity = isUseGravity;
                    break;
                case nameof(OreAmount):
                    if (HasInputAuthority)
                        oreAmountText.text = "x " + $"{OreAmount}";
                    break;
                case nameof(IsBuildingComfirmUIOpen):
                    if (HasInputAuthority)
                    buildingConfirmUI.SetActive(IsBuildingComfirmUIOpen);
                    break;
                case nameof(CanSetRope):
                    if(!CanSetRope)
                    {
                        if (HasInputAuthority && ropeMaxDistanceSignUI != null) ropeMaxDistanceSignUI.SetActive(true);
                        if (ropeSource == null || !ropeSource.isPlaying)
                        {
                            SoundManager.Play(ResourceEnum.SFX.rope_stretching, transform.position, false, out ropeSource);
                        }
                    }
                    else
                    {
                        if (HasInputAuthority && ropeMaxDistanceSignUI != null) ropeMaxDistanceSignUI.SetActive(false);
                    }
                    break;
                case nameof(BuildableEnumPageIndex):
                    if (HasInputAuthority)
                    {
                        SetPageIndexText();
                        RenewBuildingImanges();
                        Canvas.ForceUpdateCanvases();
                    }
                    break;
                case nameof(interactionIndex):
                    var reader = GetPropertyReader<int>(nameof(interactionIndex));
                    int previous;
                    int current;
                    (previous, current) = reader.Read(previousBuffer, currentBuffer);
                    if (previous == -1 && current != -1)
                    {
                        if (HasInputAuthority)
                        {
                            mouseLeftImage = GameManager.Instance.PoolManager.Instantiate(ResourceEnum.Prefab.MouseLeftUI, interactionUI);
                            Canvas.ForceUpdateCanvases();
                        }
                    }
                    if (interactionIndex != -1) UpdateInteractionUI(interactionIndex);
                    break;
                //case nameof(AngleCheck):
                //    angleCheck = AngleCheck;
                //    break;
            }

        }

    }

    //땅이 무엇인지 저장해둘 거예요!
    [SerializeField] protected Collider ground;

    //움직이기 위해 레이를 발사할 거예요!

    //이 캐릭터와 닿아있는 대상을 저장해둘게요!
    protected Dictionary<Collider, Vector3> attachedCollision = new();

    //지금 제가 딛고 있는 땅의 노말을 저장해둡시다!
    [SerializeField]Vector3 _groundNormal = Vector3.down;
    Vector3 GroundNormal
    {
        get => _groundNormal;
        set
        {
            _groundNormal = value;
            IsGround = (value.y > 0 && Vector3.Angle(Vector3.up, value) < 90f);
        }
    }

    //땅에 있는지 여부는 계산하는 거니까, isGround는 get만 열어줄 거예요! 땅이 있으면 땅에 있는 거예요!
    [Networked] public bool IsGround { get; set; }


    private void OnCollisionEnter(Collision collision)
    {
        //일단 닿은 바닥의 방향을 확인해볼게요!
        Vector3 normal = collision.GetContact(0).normal;

        if (normal.y <= 0.3f) return;



        //그래서 부딪힌 대상을 저장할 거예요! 상대와  닿은 노말!
        if (attachedCollision.ContainsKey(collision.collider))
        {
            //닿은 바닥의 방향이 옛날과 다르면
            if (attachedCollision[collision.collider] != normal)
            {
                //값을 변경해줍니다!
                attachedCollision[collision.collider] = normal;
            }
        }
        else
        {
            attachedCollision.Add(collision.collider, normal);
        }

        ground = collision.collider;
        switch (ground.material.name)
        {
            case "Metal (Instance)":
                FootstepSFX = ResourceEnum.SFX.footsteps_metal_cut;
                break;
            case "Dirt (Instance)":
            default:
                FootstepSFX = ResourceEnum.SFX.footsteps_dirt_cut;
                break;

        }
        GroundNormal = normal;


        //그리고 변경되었으니 땅을 체크해봅시다! 
        //Calculate_Ground();
    }


    private void OnCollisionExit(Collision collision)
    {
        //대상이 나갔으니 그냥 지웁시다!
        if (attachedCollision.Remove(collision.collider))
        {
            //나갔는데 이게 땅이었네요?
            if (collision.collider == ground)
            {
                //그러면 땅을 초기화하고
                ground = null;
                GroundNormal = Vector3.down;
            }
        }
        //다시 계산해봅시다!
        Calculate_Ground();

    }

    void Calculate_Ground()
    {
        //닿은게 없으면 땅이 아니죠!
        if (attachedCollision.Count == 0)
        {
            //땅이 아직 초기화가 안되었다면
            if (ground != null)
            {
                //땅 지워버리고
                ground = null;
            }
            //노말도 아래로 바꿔버립시다!
            GroundNormal = Vector3.down;
            //끝
            return;
        }
        else //아니면 계산해봐야해요
        {
            //가장 땅 같은 친구 찾기!
            Collider mostGroundObject = ground; //일단 지금 땅의 정보로 시작!
            //이거는 땅의 노말을 확인할 거예요!
            Vector3 mostGroundNormal;

            //노말은 만약, 가장 땅같은 친구가 있으면 그 친구를 기준으로!
            if (ground) mostGroundNormal = GroundNormal;
            else mostGroundNormal = Vector3.down;
            //아니면 땅이 없다고 생각해서 노말을 초기화해줄 거예요!

            foreach (var currentTarget in attachedCollision)
            {
                //가장 땅 같다는 건 가장 위를 보고 있다는 것!
                if (mostGroundNormal.y < currentTarget.Value.y)
                {
                    //그래서 1등 자리를 이 친구한테 줍시다!
                    mostGroundNormal = currentTarget.Value;
                    mostGroundObject = currentTarget.Key;
                }
            };

            //나온 결과를 저장하고
            ground = mostGroundObject;
            GroundNormal = mostGroundNormal;
        }

    }

    private float CalculrateNextFrameGroundAngle()
    {
        var nextFramePlayerPosition = transform.position + transform.forward * 1.25f + (transform.forward * MoveDir.z + transform.right * MoveDir.x) * Runner.DeltaTime * moveSpeed;

        if (Physics.Raycast(nextFramePlayerPosition, Vector3.down, out RaycastHit hitInfo, 1f))
        {
            return Vector3.Angle(Vector3.up, hitInfo.normal);
        }

        return 0f;
    }

    public void SetKeyGuideUI()
    {
        if (HasInputAuthority)
            onScreenKeyGuideUIAnim.SetBool("OnOff", !onScreenKeyGuideUIAnim.GetBool("OnOff"));
    }

    public void SetPageIndexText()
    {
        if (buildingSelectUI.activeInHierarchy && HasInputAuthority)
            pageIndexText.text = $"{BuildableEnumPageIndex + 1} / {(ResourceEnum.Prefab.buildingEnd - ResourceEnum.Prefab.buildingStart - 2) / 5 + 1}";
        
    }

    public void RenewBuildingImanges()
    {
        if (buildingSelectUI.activeInHierarchy && HasInputAuthority)
        {
            for (int i = 0; i < 5; i++)
            {
                int siblingIndex = buildingSelectUIBuildingImages[i].transform.parent.GetSiblingIndex();

                ResourceEnum.Prefab targetPrefabEnum = BuildableEnumArray[BuildableEnumPageIndex, siblingIndex];

                // 이미지 교체
                Debug.Log(targetPrefabEnum.ToString());
                Enum.TryParse(targetPrefabEnum.ToString(), out ResourceEnum.Sprite result);

                Image buildingImage = buildingSelectUIBuildingImages[i].GetComponent<Image>();

                buildingImage.sprite = ResourceManager.Get(result);
                buildingImage.transform.GetChild(0).GetComponent<Image>().enabled = result == ResourceEnum.Sprite.BlastTower;

                // 광물사용량 텍스트 교체
                int requireOre = GetRequireOreResourceAmount(targetPrefabEnum);
                buildingSelectUIBuildingImages[i].transform.parent.GetComponentInChildren<TextMeshProUGUI>().text = requireOre != -1 ? requireOre.ToString() : null;
            }
        }
    }

    private int GetRequireOreResourceAmount(ResourceEnum.Prefab prefab) => prefab switch
    {
        ResourceEnum.Prefab.Turret1a => 10,
        ResourceEnum.Prefab.Turret1d => 20,
        ResourceEnum.Prefab.ION_Cannon => 20,
        ResourceEnum.Prefab.BlastTower => 40,
        ResourceEnum.Prefab.Bridge => 5,

        ResourceEnum.Prefab.Pylon => 10,

        _ => -1,
    };


    public void Greeting()
    {
        AnimTrigger?.Invoke("GreetingTrigger");
    }

    public void ChangeView()
    {
        TPS_Mode = !TPS_Mode;
    }
}
