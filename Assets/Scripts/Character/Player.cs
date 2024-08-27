using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Fusion;
using static UnityEngine.GraphicsBuffer;


public partial class Player : Character
{
    [SerializeField] protected ControllerBase possessionController;
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
    protected ImgsFillDynamic interactionUpdateProgress; // 상호작용 진행중 UI 채울 정도
    protected GameObject mouseLeftImage; // 마우스좌클릭 Image
    protected TextMeshProUGUI buttonText; // 버튼에 띄워줄 text

    protected List<IInteraction> interactionObjectList = new List<IInteraction>(); // 범위내 상호작용 가능한 대상들의 리스트
    protected IInteraction interactionObject = null; // 내가 선택한 상호작용 대상
    public IInteraction InteractionObject => interactionObject;
    [SerializeField] protected int interactionIndex = -1; // 내가 선택한 상호작용 대상이 리스트에서 몇번째 인지
    protected Dictionary<IInteraction, GameObject> interactionObjectDictionary = new(); // 상호작용 가능한 대상들의 리스트와 버튼UI오브젝트를 1:1대응시켜줄 Dictionary 

    protected bool isInteracting; // 나는 지금 상호작용 중인가?
    public bool IsInteracting => isInteracting;
    protected Interaction interactionType; // 나는 어떤 상호작용을 하고있는가?

    protected int oreAmount;
    public int OreAmount { get { return oreAmount; } set { oreAmount = value; } }
    /////////////////////////////
    [SerializeField] protected GameObject myMarker;
    [SerializeField] protected GameObject otherMarker;

    protected GameObject bePicked;
    public GameObject BePicked => bePicked;
    // protected bool isHandFree;
    protected ResourceEnum.Prefab[,] buildableEnumArray = new ResourceEnum.Prefab[5, 5];
    protected int buildableEnumPageIndex = 0;
    [Networked] public Building DesigningBuilding { get; set; }
    [Networked] public bool IsThisPlayerCharacterUICanvasActivated { get; set; } = false;

    protected float rotate_x; // 마우스 이동에 따른 시점 회전 x값
    protected float rotate_y; // 마우스 이동에 따른 시점 회전 y값
    protected float mouseDelta_y; // 마우스 이동 변화량 y값

    [Networked] public Vector3 MoveDir { get; set; }
    protected Vector3 currentDir = Vector3.zero;

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

        targetController.DoMove += Move;
        targetController.DoScreenRotate += ScreenRotate;
        targetController.DoDesignBuilding += DesignBuilding;
        targetController.DoBuild += Build;
        targetController.DoInteractionStart += InteractionStart;
        targetController.DoInteractionEnd += InteractionEnd;
        targetController.DoMouseWheel += MouseWheel;
        targetController.DoCancel += Cancel;
    }
    
    protected void UnRegistrationFunction(ControllerBase targetController)
    {
        targetController.DoMove -= Move;
        targetController.DoScreenRotate -= ScreenRotate;
        targetController.DoDesignBuilding -= DesignBuilding;
        targetController.DoBuild -= Build;
        targetController.DoInteractionStart -= InteractionStart;
        targetController.DoInteractionEnd-= InteractionEnd;
        targetController.DoMouseWheel -= MouseWheel;
        targetController.DoCancel -= Cancel;
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
        Debug.Log("player mystart");

        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }

        if (cameraOffset_FPS == null)
        {
            cameraOffset_FPS = transform.Find("CameraOffset");
        }

        for (ResourceEnum.Prefab index = ResourceEnum.Prefab.buildingStart+1; index < ResourceEnum.Prefab.buildingEnd; index++)
        {
            int y = index - (ResourceEnum.Prefab.buildingStart+1);
            int x = y / 5;
            y %= 5;
            buildableEnumArray[x, y] = index;
        }

        //buildableEnumArray[0, 0] = ResourceEnum.Prefab.Turret1a;
        //buildableEnumArray[0, 1] = ResourceEnum.Prefab.ION_Cannon;

        if(HasInputAuthority) otherMarker.SetActive(false);
        else myMarker.SetActive(false);

    }

    public override void Spawned()
    {
        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            PreviousPosition = data.currentPosition;
            PreviousRotation = data.currentRotation;
        }
    }

    protected override void MyUpdate(float deltaTime)
    {
        /////////////////////////// 
        //이동방향이 있을 시 해당 방향으로 움직임. + 애니메이션 설정
        //if (MoveDir.magnitude == 0)
        //{
        //    float velocityX = Mathf.Lerp(rb.velocity.x, 0f, 0.1f);
        //    float velocityZ = Mathf.Lerp(rb.velocity.z, 0f, 0.1f);
        //    rb.velocity = new Vector3(velocityX, rb.velocity.y, velocityZ);
        //}
        //else
        //{

        //    rb.velocity = (transform.forward * MoveDir.z + transform.right * MoveDir.x).normalized * moveSpeed;
        //}

        //currentVelocity = rb.velocity;
        //rb.MovePosition(transform.position + MoveDirCalculrate(MoveDir) * deltaTime);
        //transform.position += MoveDirCalculrate(MoveDir) * deltaTime;
        //updateTime += deltaTime;
        //interpolatedPosition = previousPosition + MoveDirCalculrate(MoveDir) * updateTime;
        //rb.MovePosition(interpolatedPosition);
        currentDir = new Vector3(Mathf.Lerp(currentDir.x, MoveDir.x, 0.1f), currentDir.y, Mathf.Lerp(currentDir.z, MoveDir.z, 0.1f));

        AnimFloat?.Invoke("Speed", MoveDir.magnitude);
        AnimFloat?.Invoke("MoveForward", currentDir.z);
        AnimFloat?.Invoke("MoveRight", currentDir.x);
        //////////////////////////

        // CharacterUICanvas
        if (buildingSelectUI == null)
        {
            if(possessionController != null && HasInputAuthority)
            {
                interactionUI = GameObject.FindGameObjectWithTag("InteractionScrollView").transform;
                interactionContent = GameObject.FindGameObjectWithTag("InteractionContent").GetComponent<RectTransform>();
                interactionUpdateUI = GameObject.FindGameObjectWithTag("InteractionUpdateUI");
                buildingSelectUI = GameObject.FindGameObjectWithTag("BuildingSelectUI");

                interactionUpdateUI.SetActive(false);
                buildingSelectUI.SetActive(false);
            }
            GameManager.CloseLoadInfo();
        }

        /////////////////////////////
        // 상호작용
        if (isInteracting && interactionObject != null)
        {
            float progress = interactionObject.InteractionUpdate(deltaTime, interactionType);

            if (possessionController != null && HasInputAuthority)
                interactionUpdateProgress.SetValue(progress, true);

            if (progress >= 1f)
            {
                if (InteractionEnd())
                {
                    interactionObjectList.Remove(interactionObject);

                    if (interactionObjectDictionary.TryGetValue(interactionObject, out GameObject result))
                    {
                        GameManager.Instance.PoolManager.Destroy(result);
                        interactionObjectDictionary.Remove(interactionObject);
                    }

                    if (interactionObjectList.Count == 0)
                    {
                        interactionIndex = -1;
                        interactionObject = null;

                        if (HasInputAuthority)
                        {
                            GameManager.Instance.PoolManager.Destroy(mouseLeftImage);
                        }
                    }
                    else
                    {
                        interactionIndex = Mathf.Min(interactionIndex, interactionObjectList.Count - 1);
                        interactionObject = interactionObjectList[interactionIndex];
                    }

                    UpdateInteractionUI(interactionIndex);
                }
            }
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

            Vector3 pickPos = transform.position + transform.forward * 5f;
            int x = (int)pickPos.x;
            int z = (int)pickPos.z;
            DesigningBuilding.transform.position = new Vector3(x, DesigningBuilding.gameObject.transform.position.y, z);
            Vector2Int currentPos = new Vector2Int(x, z);

            // 건물위치에 변화가 생겼을 때 건물을 지을 수 있는 상태인지 체크함.
            if (DesigningBuilding.TiledBuildingPos != currentPos)
            {
                DesigningBuilding.TiledBuildingPos = currentPos;
                DesigningBuilding.CheckBuild();
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
            Vector3 gravity = IsGround? Vector3.zero : Vector3.down * Mathf.Abs(rb.velocity.y);

            if (IsGround)
            {
                velocity = Vector3.ProjectOnPlane(wantMoveDir, GroundNormal);
            }

            if (!IsGround) velocity *= 0.1f;
            rb.velocity = velocity * moveSpeed + gravity;


            //if (MoveDir.magnitude == 0)
            //{
            //    float velocityX = Mathf.Lerp(rb.velocity.x, 0f, 0.1f);
            //    float velocityZ = Mathf.Lerp(rb.velocity.z, 0f, 0.1f);
            //    rb.velocity = new Vector3(velocityX, rb.velocity.y, velocityZ);
            //}
            //else
            //{
            //    //rb.velocity = Vector3.ProjectOnPlane((transform.forward * MoveDir.z + transform.right * MoveDir.x), GroundNormal).normalized * moveSpeed + Vector3.up * rb.velocity.y;
            //}
        }
        else
        {
            rb.position = PreviousPosition;
        }

        ///////////////////////////// 
        // 가건물을 들고있을때 해당 가건물의 위치를 int단위로 맞춰주는 부분.
        //if (DesigningBuilding != null && HasStateAuthority)
        //{
        //    Vector3 pickPos = transform.position + transform.forward * 5f;
        //    int x = (int)pickPos.x;
        //    int z = (int)pickPos.z;
        //    DesigningBuilding.transform.position = new Vector3(x, DesigningBuilding.gameObject.transform.position.y, z);
        //    Vector2Int currentPos = new Vector2Int(x, z);

        //    // 건물위치에 변화가 생겼을 때 건물을 지을 수 있는 상태인지 체크함.
        //    if (DesigningBuilding.TiledBuildingPos != currentPos)
        //    {
        //        DesigningBuilding.TiledBuildingPos = currentPos;
        //        DesigningBuilding.CheckBuild();
        //    }
        //}

    }

    // 마우스를 움직임에 따라서 카메라를 회전시키는 함수.
    //public virtual void ScreenRotate(Vector2 mouseDelta)
    //{
    //    ////좌우회전은 캐릭터를 회전
    //    //rotate_y = transform.eulerAngles.y + mouseDelta.x * 0.02f * 10f;
    //    //transform.localEulerAngles = new Vector3(0f, rotate_y, 0f);

    //    //mouseDelta_y = -mouseDelta.y * 0.02f * 10f;
    //    //rotate_x = rotate_x + mouseDelta_y;
    //    //rotate_x = Mathf.Clamp(rotate_x, -45f, 45f);

    //    //// 상하회전은 카메라만 회전
    //    //cameraOffset_FPS.localEulerAngles = new Vector3(rotate_x, 0f, 0f);
    //    if (possessionController != null && HasInputAuthority)
    //    {
    //        rotate_y = transform.eulerAngles.y + mouseDelta.x * Runner.DeltaTime * 10f;
    //        transform.localEulerAngles = new Vector3(0f, rotate_y, 0f);

    //        mouseDelta_y = -mouseDelta.y * Runner.DeltaTime * 10f;
    //        rotate_x += mouseDelta_y;
    //        rotate_x = Mathf.Clamp(rotate_x, -45f, 45f);
    //        if (cameraOffset_FPS == null)
    //        {
    //            cameraOffset_FPS = transform.Find("CameraOffset");
    //        }
    //        cameraOffset_FPS.localEulerAngles = new Vector3(rotate_x, 0f, 0f);
    //    }
    //    else
    //    {
    //        transform.rotation = previousRotation;
    //    }
    //}

    public virtual void ScreenRotate(Vector2 mouseDelta)
    {
        if (HasInputAuthority)
        {
            //좌우회전은 캐릭터를 회전
            rotate_y = transform.eulerAngles.y + mouseDelta.x * 0.02f * 10f;
            transform.localEulerAngles = new Vector3(0f, rotate_y, 0f);

            mouseDelta_y = -mouseDelta.y * 0.02f * 10f;
            rotate_x += mouseDelta_y;
            rotate_x = Mathf.Clamp(rotate_x, -45f, 45f);

            // 상하회전은 카메라만 회전
            cameraOffset_FPS.localEulerAngles = new Vector3(rotate_x, 0f, 0f);
        }

        //////////////////////////////////////////////////////////////////////
        //if (DesigningBuilding != null && HasStateAuthority)
        //{
        //    if (transform.rotation.y < 45f || transform.rotation.y >= 315f)
        //    {
        //        DesigningBuilding.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        //    }
        //    else if (transform.rotation.y < 135f)
        //    {
        //        DesigningBuilding.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
        //    }
        //    else if (transform.rotation.y < 225f)
        //    {
        //        DesigningBuilding.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        //    }
        //    else if (transform.rotation.y < 315f)
        //    {
        //        DesigningBuilding.transform.rotation = Quaternion.Euler(0f, 270f, 0f);
        //    }

        //    Vector3 pickPos = transform.position + transform.forward * 5f;
        //    int x = (int)pickPos.x;
        //    int z = (int)pickPos.z;
        //    DesigningBuilding.transform.position = new Vector3(x, DesigningBuilding.gameObject.transform.position.y, z);
        //    Vector2Int currentPos = new Vector2Int(x, z);

        //    // 건물위치에 변화가 생겼을 때 건물을 지을 수 있는 상태인지 체크함.
        //    if (DesigningBuilding.TiledBuildingPos != currentPos)
        //    {
        //        DesigningBuilding.TiledBuildingPos = currentPos;
        //        DesigningBuilding.CheckBuild();
        //    }
        //}
        //else
        //{
        //    transform.rotation = PreviousRotation;
        //}


        //if (possessionController != null && HasInputAuthority)
        //{
        //    rotate_y = transform.eulerAngles.y + mouseDelta.x * Runner.DeltaTime * 10f;
        //    transform.localEulerAngles = new Vector3(0f, rotate_y, 0f);

        //    mouseDelta_y = -mouseDelta.y * Runner.DeltaTime * 10f;
        //    rotate_x += mouseDelta_y;
        //    rotate_x = Mathf.Clamp(rotate_x, -45f, 45f);
        //    if (cameraOffset_FPS == null)
        //    {
        //        cameraOffset_FPS = transform.Find("CameraOffset");
        //    }
        //    cameraOffset_FPS.localEulerAngles = new Vector3(rotate_x, 0f, 0f);
        //}
        //else
        //{
        //    transform.rotation = previousRotation;
        //}
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

        if (index < 0 || buildableEnumArray[buildableEnumPageIndex, index] == 0) return false;
        Debug.Log(buildableEnumArray[buildableEnumPageIndex, index]);
        
        NetworkObject building = GameManager.Instance.NetworkManager.Runner.Spawn(ResourceManager.Get(buildableEnumArray[buildableEnumPageIndex, index]));
        DesigningBuilding = building.GetComponent<Building>();

        return true;

    }

    public bool Build()
    {
        if (DesigningBuilding == null)
        {
            IsThisPlayerCharacterUICanvasActivated = true;
            Debug.Log($"IsThisPCUICA : {IsThisPlayerCharacterUICanvasActivated}");
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

        if (DesigningBuilding != null)
        {
            Runner.Despawn(DesigningBuilding.GetComponent<NetworkObject>());
        }
        
    }


    public bool InteractionStart()
    {
        if(DesigningBuilding != null)
        {
            if (DesigningBuilding.FixPlace())
            {
                DesigningBuilding = null;
                return true;
            }

            return default;
        }
        if (interactionObject == null) return false;

        interactionType = interactionObject.InteractionStart(this);

        switch (interactionType)
        {
            default: InteractionEnd(); break;
            case Interaction.Build:
                isInteracting = true;
                AnimBool?.Invoke("isBuild", true);

                if (HasInputAuthority)
                {
                    interactionUI.gameObject.SetActive(false);
                    interactionUpdateUI.SetActive(true);
                    interactionUpdateProgress = interactionUpdateUI.GetComponentInChildren<ImgsFillDynamic>();
                    buttonText = interactionUpdateUI.GetComponentInChildren<TextMeshProUGUI>();
                    buttonText.text = $"건설중...";
                }
                
                //GameManager.Instance.PoolManager.Instantiate(ResourceEnum.Prefab.Hammer, sockets.FindSocket("RightHand").gameObject.transform);
                break;
        }

        return default;
    }

    //public bool InteractionStart<T>(T target) where T : IInteraction
    //{
    //    interactionObject = target;

    //    if (interactionObject == null) return false;

    //    interactionType = interactionObject.InteractionStart(this);

    //    switch (interactionType)
    //    {
    //        default: InteractionEnd(); break;
    //        case Interaction.Build:
    //            isInteracting = true;
    //            AnimBool?.Invoke("isBuild", true);
    //            interactionUI.gameObject.SetActive(false);
    //            interactionUpdateUI.SetActive(true);
    //            interactionUpdateProgress = interactionUpdateUI.GetComponentInChildren<ImgsFillDynamic>();
    //            buttonText = interactionUpdateUI.GetComponentInChildren<TextMeshProUGUI>();
    //            buttonText.text = $"건설중...";
    //            GameManager.Instance.PoolManager.Instantiate(ResourceEnum.Prefab.Hammer, sockets.FindSocket("RightHand").gameObject.transform);
    //            break;
    //    }


    //    return default;
    //}

    //public bool InteractionUpdate<T>(T target, float deltaTime) where T : IInteraction
    //{
    //    if (target == null) return false;
    //    if ((IInteraction)target != interactionObject) return false;
    //    if (isInteracting && target != null)
    //    {
    //        target.InteractionUpdate(deltaTime, interactionType);
    //    }
    //}

    public bool InteractionEnd()
    {
        if (interactionObject == null) return false;

        isInteracting = false;
        bool result = interactionObject.InteractionEnd();

        switch (interactionType)
        {
            default: break;
            case Interaction.Build: 
                AnimBool?.Invoke("isBuild", false);
                if(HasInputAuthority)
                {
                    interactionUI.gameObject.SetActive(true);
                    interactionUpdateUI.SetActive(false);
                    interactionUpdateProgress = null;
                }
                //GameManager.Instance.PoolManager.Destroy(sockets.FindSocket("RightHand").gameObject.GetComponentInChildren<PoolingInfo>());
                break;
        }

        //interactionObject = null;
        interactionType = Interaction.None;

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
        if (interactionObjectList.Exists(inst => inst == target)) return;
        if (System.Array.Find(target.GetInteractionColliders(), col => other == col))
        {
            interactionObjectList.Add(target);

            //GameObject button = Instantiate(ResourceManager.Get(ResourceEnum.Prefab.InteractableObjButton), interactionContent);
            GameObject button = GameManager.Instance.PoolManager.Instantiate(ResourceEnum.Prefab.InteractableObjButton, interactionContent);
            button.transform.SetSiblingIndex(9999);  // SiblingIndex - 나는 부모의 자식중에 몇번째 Index에 있는가
            buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = $"{target.GetName()}";
            interactionObjectDictionary.Add(target, button);

            if (interactionObjectList.Count == 1)
            {
                interactionIndex = 0;
                interactionObject = target;
                if (HasInputAuthority)
                {
                    mouseLeftImage = GameManager.Instance.PoolManager.Instantiate(ResourceEnum.Prefab.MouseLeftUI, interactionUI);
                    Canvas.ForceUpdateCanvases();
                }
            }

            UpdateInteractionUI(interactionIndex);
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
            interactionObjectList.Remove(target);

            if (interactionObjectDictionary.TryGetValue(target, out GameObject result))
            {
                GameManager.Instance.PoolManager.Destroy(result);
                interactionObjectDictionary.Remove(target);
            }

            if (interactionObjectList.Count == 0)
            {
                interactionIndex = -1;
                if (isInteracting)
                {
                    InteractionEnd();
                }
                interactionObject = null;

                if (HasInputAuthority)
                {
                    GameManager.Instance.PoolManager.Destroy(mouseLeftImage);
                }
            }
            else
            {
                interactionIndex = Mathf.Min(interactionIndex, interactionObjectList.Count - 1);
                if (isInteracting && target == interactionObject)
                {
                    InteractionEnd();
                }
                interactionObject = interactionObjectList[interactionIndex];

            }

            UpdateInteractionUI(interactionIndex);
        }
        
    }

    // 마우스 휠을 굴려서 상호작용할 대상을 정함.
    public void MouseWheel(Vector2 scrollDelta)
    {
        if (interactionObjectList.Count == 0) return;
        if (scrollDelta.y == 0f) return;

        // 휠을 위로 굴렸을 때
        else if (scrollDelta.y > 0)
        {
            interactionIndex--;
            interactionIndex = Mathf.Max(interactionIndex, 0);
            interactionObject = interactionObjectList[interactionIndex];

            if (interactionIndex < interactionObjectList.Count - 4 && HasInputAuthority)
            {
                interactionContent.anchoredPosition -= new Vector2(0, 50f);
                interactionContent.anchoredPosition = new Vector2(0, Mathf.Clamp(interactionContent.anchoredPosition.y,0, (interactionObjectList.Count - 6) * 50f));
            }
        }
        // 휠을 아래로 굴렸을 때
        else if (scrollDelta.y < 0)
        {
            interactionIndex++;
            interactionIndex = Mathf.Min(interactionObjectList.Count - 1, interactionIndex);
            interactionObject = interactionObjectList[interactionIndex];

            if (interactionIndex > 4 && HasInputAuthority)
            {
                interactionContent.anchoredPosition += new Vector2(0, 50f);
                interactionContent.anchoredPosition = new Vector2(0, Mathf.Clamp(interactionContent.anchoredPosition.y, 0, (interactionObjectList.Count - 6) * 50f));
            }
        }
        UpdateInteractionUI(interactionIndex);
    }

    // 상호작용 UI를 최신화하는 함수
    private void UpdateInteractionUI(int targetIndex)
    {
        Canvas.ForceUpdateCanvases();

        for (int i = 0; i < interactionObjectList.Count; i++)
        {
            GameObject button = interactionObjectDictionary[interactionObjectList[i]];
            Image buttonImage = button.GetComponentInChildren<Image>();
            if (targetIndex == i)
            {
                buttonImage.color = Color.yellow;
                if (HasInputAuthority)
                {
                    mouseLeftImage.transform.position = button.transform.position;
                }
            } 
            else buttonImage.color = Color.white;
        }
    }

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
            }
            
        }

    }

    //땅이 무엇인지 저장해둘 거예요!
    [SerializeField] protected Collider ground;

    //움직이기 위해 레이를 발사할 거예요!

    //이 캐릭터와 닿아있는 대상을 저장해둘게요!
    protected Dictionary<Collider, Vector3> attachedCollision = new();

    //지금 제가 딛고 있는 땅의 노말을 저장해둡시다!
    Vector3 _groundNormal = Vector3.down;
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


        Debug.Log(normal.y);

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
        var nextFramePlayerPosition = transform.position + transform.forward * 1.25f + (transform.forward * MoveDir.z  + transform.right * MoveDir.x) * Runner.DeltaTime * moveSpeed ;

        if (Physics.Raycast(nextFramePlayerPosition, Vector3.down, out RaycastHit hitInfo, 1f))
        { 
            return Vector3.Angle(Vector3.up, hitInfo.normal);
        }
        
        return 0f;
    }
}
