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
    /////////////////////////////interaction ������
    [SerializeField] protected Transform interactionUI; // ��ȣ�ۿ� UI��ġ
    [SerializeField] protected RectTransform interactionContent; // ��ȣ�ۿ��� UI����� �������� ��ġ
    [SerializeField] protected GameObject interactionUpdateUI; // ��ȣ�ۿ� ������ UI
    [SerializeField] public GameObject buildingSelectUI; // ���� ���� UI
    public GameObject[] buildingSelectUIBuildingImages;
    [SerializeField] public GameObject buildingConfirmUI; // ���ǹ� ��������� ��ġ ���Ű �����ִ� UI
    public GameObject ropeMaxDistanceSignUI; // ���� ���̰� �ִ��� �� ������ ��� UI
    protected Animator onScreenKeyGuideUIAnim;

    protected ImgsFillDynamic interactionUpdateProgress; // ��ȣ�ۿ� ������ UI ä�� ����
    protected GameObject mouseLeftImage; // ���콺��Ŭ�� Image
    protected TextMeshProUGUI buttonText; // ��ư�� ����� text
    protected TextMeshProUGUI oreAmountText; // ������ �ִ� �������� ������ UI
    protected Image buttonImage;
    public TextMeshProUGUI pageIndexText;
    public TextMeshProUGUI leftRopeLengthText;
    public TextMeshProUGUI guidelineText;
    GameObject directPowerSupply;

    protected IInteraction interactionObject = null; // ���� ������ ��ȣ�ۿ� ���
    public IInteraction InteractionObject => interactionObject;
    [SerializeField, Networked] protected int interactionIndex { get; set; } = -1; // ���� ������ ��ȣ�ۿ� ����� ����Ʈ���� ���° ����
    protected List<IInteraction> interactionObjectList = new List<IInteraction>(); // ������ ��ȣ�ۿ� ������ ������ ����Ʈ
    protected Dictionary<IInteraction, GameObject> interactionObjectDictionary = new(); // ��ȣ�ۿ� ������ ������ ����Ʈ�� ��ưUI������Ʈ�� 1:1���������� Dictionary 
    
    // test
    [SerializeField]protected List<InteractionButtonInfo> interactionButtonInfos = new List<InteractionButtonInfo>();
     
    [SerializeField, Networked] protected bool isInteracting { get; set; } // ���� ���� ��ȣ�ۿ� ���ΰ�?
    //public bool IsInteracting => isInteracting;
    [SerializeField, Networked] protected Interaction interactionType { get; set; } // ���� � ��ȣ�ۿ��� �ϰ��ִ°�?

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

    protected float rotate_x; // ���콺 �̵��� ���� ���� ȸ�� x��
    protected float rotate_y; // ���콺 �̵��� ���� ���� ȸ�� y��
    protected float mouseDelta_y; // ���콺 �̵� ��ȭ�� y��

    [Networked] public Vector3 MoveDir { get; set; }
    protected Vector3 currentDir = Vector3.zero;
    Vector2 lastPos;
    //[Networked] bool AngleCheck { get; set; } = false;
    bool angleCheck;
    [Networked] public Vector3 angleCheckVector { get; set; }

    [Networked] public Vector3 PreviousPosition { get; set; }
    [Networked] public Quaternion PreviousRotation { get; set; }

    //public Vector3 interpolatedPosition;
    ////����Ű ������ ���� ���� ������ ���غþ��!
    //Vector3 prefferedMoveDirection;
    ////���� �������� �����̴� �����Դϴ�!
    //public Vector3 planeMovementVector;
    ////�ٴ� �븻�� �������� �����̴� �����Դϴ�!
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

        //�Լ��� ���޸� �ϸ� �Ǵ� �ſ���! -> ��Ʈ�ѷ��� ���ϰ� ���� ��
        //�� �Լ��� ��� ��������!
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
        //�̵������� ���� �� �ش� �������� ������. + �ִϸ��̼� ����
        
        currentDir = new Vector3(Mathf.Lerp(currentDir.x, MoveDir.x, deltaTime), currentDir.y, Mathf.Lerp(currentDir.z, MoveDir.z, deltaTime));

        AnimFloat?.Invoke("Speed", MoveDir.magnitude);
        AnimFloat?.Invoke("MoveForward", currentDir.z);
        AnimFloat?.Invoke("MoveRight", currentDir.x);
        
        // ��ȣ�ۿ�
        if (isInteracting && interactionObject != null)
        {
            // �Ǽ� ������ ��������
            float progress = interactionObject.InteractionUpdate(deltaTime, Interaction.Build);

            if (possessionController != null && HasInputAuthority && interactionUpdateProgress != null)
                interactionUpdateProgress.SetValue(progress, true);

            if (progress >= 1f)
            {
                InteractionEnd();
            }
            // �Ǽ� ��
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

            // �ǹ���ġ�� ��ȭ�� ������ �� �ǹ��� ���� �� �ִ� �������� üũ��.
            if (DesigningBuilding.TiledBuildingPos != currentPos)
            {
                DesigningBuilding.transform.position = new Vector3(currentPos.x, DesigningBuilding.gameObject.transform.position.y, currentPos.y);
                DesigningBuilding.TiledBuildingPos = new Vector2Int((int)DesigningBuilding.transform.position.x, (int)DesigningBuilding.transform.position.z);
                DesigningBuilding.CheckBuild();
            }
        }
        if (ropeBuilding != null)
        {
            // ���� ����
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

        // �߼Ҹ�
        if (NetworkIsWalking)
        {

            if (footstepAudioSource == null)
            {
                // ����� �ҽ��� ���°��
                SoundManager.Play(FootstepSFX, transform.position, true, out footstepAudioSource);
                currentFootstep = FootstepSFX;
            }
            else if (FootstepSFX != currentFootstep)
            {
                // ����� �ҽ��� �ִµ� ���� ����ǰ��ִ� �ٴ������� ����ϰ���� �ٴ������� �ٸ����
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
            // �ȰȰ��ִ°��
            SoundManager.StopSFX(footstepAudioSource);
            footstepAudioSource = null;
        }
        // û�ұ� �Ҹ�
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

    // Ű���� �Է����� �÷��̾� �̵������� �����ϴ� �Լ�.
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

                // CanSetRope�� false�̸� ���� ȸ���� �����ϰ�
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
            //�¿�ȸ���� ĳ���͸� ȸ��
            rotate_y = transform.eulerAngles.y + mouseDelta.x * 0.02f * 10f;
            transform.localEulerAngles = new Vector3(0f, rotate_y, 0f);

            mouseDelta_y = -mouseDelta.y * 0.02f * 10f;
            rotate_x += mouseDelta_y;
            rotate_x = Mathf.Clamp(rotate_x, -45f, 30f + 13f / Mathf.Abs(GroundNormal.y));

            
            // ����ȸ���� ī�޶� ȸ��
            cameraOffset_FPS.localEulerAngles = new Vector3(rotate_x, 0f, 0f);
        }
    }

    

    public bool PickUp(GameObject target) { return default; }
    public bool PutDown() { return default; }

    // �Ǽ��� �ǹ��� ������(���ǹ�) ���·� ����� �Լ�
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
            Debug.Log("�Ǽ��� �ʿ��� ������ �����մϴ�.");
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

    // ��ȣ�ۿ� ������ ����� �����Ǿ��� �� ó��
    private void OnTriggerEnter(Collider other)
    {
        IInteraction target;

        if (!other.TryGetComponent(out target))
        {
            target = other.GetComponentInParent<IInteraction>();
            if (target == null) return;
        }

        // �̹� �ִٸ� �߰���������
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
                

                button.transform.SetSiblingIndex(9999);  // SiblingIndex - ���� �θ��� �ڽ��߿� ���° Index�� �ִ°�
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

    // ��ȣ�ۿ� ������ ��� ����Ʈ�� �ִ� ����� ������������ ������ �� ó��
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

    // ���콺 ���� ������ ��ȣ�ۿ��� ����� ����.
    public void MouseWheel(Vector2 scrollDelta)
    {
        //if (interactionContent == null || interactionContent.gameObject.activeInHierarchy == false) return;
        if (interactionButtonInfos.Count == 0) return;

        int index = interactionIndex;

        if (scrollDelta.y == 0f) return;
        // ���� ���� ������ ��
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
        // ���� �Ʒ��� ������ ��
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

    // ��ȣ�ۿ� UI�� �ֽ�ȭ�ϴ� �Լ�
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

                button.transform.SetSiblingIndex(9999);  // SiblingIndex - ���� �θ��� �ڽ��߿� ���° Index�� �ִ°�
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

    //���� �������� �����ص� �ſ���!
    [SerializeField] protected Collider ground;

    //�����̱� ���� ���̸� �߻��� �ſ���!

    //�� ĳ���Ϳ� ����ִ� ����� �����صѰԿ�!
    protected Dictionary<Collider, Vector3> attachedCollision = new();

    //���� ���� ��� �ִ� ���� �븻�� �����صӽô�!
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

    //���� �ִ��� ���δ� ����ϴ� �Ŵϱ�, isGround�� get�� ������ �ſ���! ���� ������ ���� �ִ� �ſ���!
    [Networked] public bool IsGround { get; set; }


    private void OnCollisionEnter(Collision collision)
    {
        //�ϴ� ���� �ٴ��� ������ Ȯ���غ��Կ�!
        Vector3 normal = collision.GetContact(0).normal;

        if (normal.y <= 0.3f) return;



        //�׷��� �ε��� ����� ������ �ſ���! ����  ���� �븻!
        if (attachedCollision.ContainsKey(collision.collider))
        {
            //���� �ٴ��� ������ ������ �ٸ���
            if (attachedCollision[collision.collider] != normal)
            {
                //���� �������ݴϴ�!
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


        //�׸��� ����Ǿ����� ���� üũ�غ��ô�! 
        //Calculate_Ground();
    }


    private void OnCollisionExit(Collision collision)
    {
        //����� �������� �׳� ����ô�!
        if (attachedCollision.Remove(collision.collider))
        {
            //�����µ� �̰� ���̾��׿�?
            if (collision.collider == ground)
            {
                //�׷��� ���� �ʱ�ȭ�ϰ�
                ground = null;
                GroundNormal = Vector3.down;
            }
        }
        //�ٽ� ����غ��ô�!
        Calculate_Ground();

    }

    void Calculate_Ground()
    {
        //������ ������ ���� �ƴ���!
        if (attachedCollision.Count == 0)
        {
            //���� ���� �ʱ�ȭ�� �ȵǾ��ٸ�
            if (ground != null)
            {
                //�� ����������
                ground = null;
            }
            //�븻�� �Ʒ��� �ٲ�����ô�!
            GroundNormal = Vector3.down;
            //��
            return;
        }
        else //�ƴϸ� ����غ����ؿ�
        {
            //���� �� ���� ģ�� ã��!
            Collider mostGroundObject = ground; //�ϴ� ���� ���� ������ ����!
            //�̰Ŵ� ���� �븻�� Ȯ���� �ſ���!
            Vector3 mostGroundNormal;

            //�븻�� ����, ���� ������ ģ���� ������ �� ģ���� ��������!
            if (ground) mostGroundNormal = GroundNormal;
            else mostGroundNormal = Vector3.down;
            //�ƴϸ� ���� ���ٰ� �����ؼ� �븻�� �ʱ�ȭ���� �ſ���!

            foreach (var currentTarget in attachedCollision)
            {
                //���� �� ���ٴ� �� ���� ���� ���� �ִٴ� ��!
                if (mostGroundNormal.y < currentTarget.Value.y)
                {
                    //�׷��� 1�� �ڸ��� �� ģ������ �ݽô�!
                    mostGroundNormal = currentTarget.Value;
                    mostGroundObject = currentTarget.Key;
                }
            };

            //���� ����� �����ϰ�
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

                // �̹��� ��ü
                Debug.Log(targetPrefabEnum.ToString());
                Enum.TryParse(targetPrefabEnum.ToString(), out ResourceEnum.Sprite result);

                Image buildingImage = buildingSelectUIBuildingImages[i].GetComponent<Image>();

                buildingImage.sprite = ResourceManager.Get(result);
                buildingImage.transform.GetChild(0).GetComponent<Image>().enabled = result == ResourceEnum.Sprite.BlastTower;

                // ������뷮 �ؽ�Ʈ ��ü
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
