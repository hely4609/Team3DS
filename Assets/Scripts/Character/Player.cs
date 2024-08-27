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
    /////////////////////////////interaction ������
    [SerializeField] protected Transform interactionUI; // ��ȣ�ۿ� UI��ġ
    [SerializeField] protected RectTransform interactionContent; // ��ȣ�ۿ��� UI����� �������� ��ġ
    [SerializeField] protected GameObject interactionUpdateUI; // ��ȣ�ۿ� ������ UI
    [SerializeField] public GameObject buildingSelectUI; // ���� ���� UI
    protected ImgsFillDynamic interactionUpdateProgress; // ��ȣ�ۿ� ������ UI ä�� ����
    protected GameObject mouseLeftImage; // ���콺��Ŭ�� Image
    protected TextMeshProUGUI buttonText; // ��ư�� ����� text

    protected List<IInteraction> interactionObjectList = new List<IInteraction>(); // ������ ��ȣ�ۿ� ������ ������ ����Ʈ
    protected IInteraction interactionObject = null; // ���� ������ ��ȣ�ۿ� ���
    public IInteraction InteractionObject => interactionObject;
    [SerializeField] protected int interactionIndex = -1; // ���� ������ ��ȣ�ۿ� ����� ����Ʈ���� ���° ����
    protected Dictionary<IInteraction, GameObject> interactionObjectDictionary = new(); // ��ȣ�ۿ� ������ ������ ����Ʈ�� ��ưUI������Ʈ�� 1:1���������� Dictionary 

    protected bool isInteracting; // ���� ���� ��ȣ�ۿ� ���ΰ�?
    public bool IsInteracting => isInteracting;
    protected Interaction interactionType; // ���� � ��ȣ�ۿ��� �ϰ��ִ°�?

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

    protected float rotate_x; // ���콺 �̵��� ���� ���� ȸ�� x��
    protected float rotate_y; // ���콺 �̵��� ���� ���� ȸ�� y��
    protected float mouseDelta_y; // ���콺 �̵� ��ȭ�� y��

    [Networked] public Vector3 MoveDir { get; set; }
    protected Vector3 currentDir = Vector3.zero;

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
        //�̵������� ���� �� �ش� �������� ������. + �ִϸ��̼� ����
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
        // ��ȣ�ۿ�
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

            // �ǹ���ġ�� ��ȭ�� ������ �� �ǹ��� ���� �� �ִ� �������� üũ��.
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

    // Ű���� �Է����� �÷��̾� �̵������� �����ϴ� �Լ�.
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
        // ���ǹ��� ��������� �ش� ���ǹ��� ��ġ�� int������ �����ִ� �κ�.
        //if (DesigningBuilding != null && HasStateAuthority)
        //{
        //    Vector3 pickPos = transform.position + transform.forward * 5f;
        //    int x = (int)pickPos.x;
        //    int z = (int)pickPos.z;
        //    DesigningBuilding.transform.position = new Vector3(x, DesigningBuilding.gameObject.transform.position.y, z);
        //    Vector2Int currentPos = new Vector2Int(x, z);

        //    // �ǹ���ġ�� ��ȭ�� ������ �� �ǹ��� ���� �� �ִ� �������� üũ��.
        //    if (DesigningBuilding.TiledBuildingPos != currentPos)
        //    {
        //        DesigningBuilding.TiledBuildingPos = currentPos;
        //        DesigningBuilding.CheckBuild();
        //    }
        //}

    }

    // ���콺�� �����ӿ� ���� ī�޶� ȸ����Ű�� �Լ�.
    //public virtual void ScreenRotate(Vector2 mouseDelta)
    //{
    //    ////�¿�ȸ���� ĳ���͸� ȸ��
    //    //rotate_y = transform.eulerAngles.y + mouseDelta.x * 0.02f * 10f;
    //    //transform.localEulerAngles = new Vector3(0f, rotate_y, 0f);

    //    //mouseDelta_y = -mouseDelta.y * 0.02f * 10f;
    //    //rotate_x = rotate_x + mouseDelta_y;
    //    //rotate_x = Mathf.Clamp(rotate_x, -45f, 45f);

    //    //// ����ȸ���� ī�޶� ȸ��
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
            //�¿�ȸ���� ĳ���͸� ȸ��
            rotate_y = transform.eulerAngles.y + mouseDelta.x * 0.02f * 10f;
            transform.localEulerAngles = new Vector3(0f, rotate_y, 0f);

            mouseDelta_y = -mouseDelta.y * 0.02f * 10f;
            rotate_x += mouseDelta_y;
            rotate_x = Mathf.Clamp(rotate_x, -45f, 45f);

            // ����ȸ���� ī�޶� ȸ��
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

        //    // �ǹ���ġ�� ��ȭ�� ������ �� �ǹ��� ���� �� �ִ� �������� üũ��.
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

    // �Ǽ��� �ǹ��� ������(���ǹ�) ���·� ����� �Լ�
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
                    buttonText.text = $"�Ǽ���...";
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
    //            buttonText.text = $"�Ǽ���...";
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
        if (interactionObjectList.Exists(inst => inst == target)) return;
        if (System.Array.Find(target.GetInteractionColliders(), col => other == col))
        {
            interactionObjectList.Add(target);

            //GameObject button = Instantiate(ResourceManager.Get(ResourceEnum.Prefab.InteractableObjButton), interactionContent);
            GameObject button = GameManager.Instance.PoolManager.Instantiate(ResourceEnum.Prefab.InteractableObjButton, interactionContent);
            button.transform.SetSiblingIndex(9999);  // SiblingIndex - ���� �θ��� �ڽ��߿� ���° Index�� �ִ°�
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

    // ���콺 ���� ������ ��ȣ�ۿ��� ����� ����.
    public void MouseWheel(Vector2 scrollDelta)
    {
        if (interactionObjectList.Count == 0) return;
        if (scrollDelta.y == 0f) return;

        // ���� ���� ������ ��
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
        // ���� �Ʒ��� ������ ��
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

    // ��ȣ�ۿ� UI�� �ֽ�ȭ�ϴ� �Լ�
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

    //���� �������� �����ص� �ſ���!
    [SerializeField] protected Collider ground;

    //�����̱� ���� ���̸� �߻��� �ſ���!

    //�� ĳ���Ϳ� ����ִ� ����� �����صѰԿ�!
    protected Dictionary<Collider, Vector3> attachedCollision = new();

    //���� ���� ��� �ִ� ���� �븻�� �����صӽô�!
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

    //���� �ִ��� ���δ� ����ϴ� �Ŵϱ�, isGround�� get�� ������ �ſ���! ���� ������ ���� �ִ� �ſ���!
    [Networked] public bool IsGround { get; set; }

    
    private void OnCollisionEnter(Collision collision)
    {
        //�ϴ� ���� �ٴ��� ������ Ȯ���غ��Կ�!
        Vector3 normal = collision.GetContact(0).normal;

        if (normal.y <= 0.3f) return;


        Debug.Log(normal.y);

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
        var nextFramePlayerPosition = transform.position + transform.forward * 1.25f + (transform.forward * MoveDir.z  + transform.right * MoveDir.x) * Runner.DeltaTime * moveSpeed ;

        if (Physics.Raycast(nextFramePlayerPosition, Vector3.down, out RaycastHit hitInfo, 1f))
        { 
            return Vector3.Angle(Vector3.up, hitInfo.normal);
        }
        
        return 0f;
    }
}
