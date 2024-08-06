using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using Fusion;

public class Player : Character
{
    protected NetworkCharacterController _ncc;

    [SerializeField]protected ControllerBase possessionController;
    [SerializeField] bool TPS_Mode;
    [SerializeField] protected Transform cameraOffset_FPS;
    [SerializeField] protected Transform cameraOffset_TPS;
    public Transform CameraOffset => TPS_Mode ? cameraOffset_TPS : cameraOffset_FPS;

    private ChangeDetector _changeDetector;
    /////////////////////////////interaction ������
    [SerializeField] protected Transform interactionUI; // ��ȣ�ۿ� UI��ġ
    [SerializeField] protected RectTransform interactionContent; // ��ȣ�ۿ��� UI����� �������� ��ġ
    [SerializeField] protected GameObject interactionUpdateUI; // ��ȣ�ۿ� ������ UI
    [SerializeField] public  GameObject buildingSelectUI; // ���� ���� UI
    protected ImgsFillDynamic interactionUpdateProgress; // ��ȣ�ۿ� ������ UI ä�� ����
    protected GameObject mouseLeftImage; // ���콺��Ŭ�� Image
    protected List<IInteraction> interactionObjectList = new List<IInteraction>(); // ������ ��ȣ�ۿ� ������ ������ ����Ʈ
    protected IInteraction interactionObject = null; // ���� ������ ��ȣ�ۿ� ���
    public IInteraction InteractionObject => interactionObject;
    protected int interactionIndex = 0; // ���� ������ ��ȣ�ۿ� ����� ����Ʈ���� ���° ����
    protected Dictionary<IInteraction, GameObject> interactionObjectDictionary = new(); // ��ȣ�ۿ� ������ ������ ����Ʈ�� ��ưUI������Ʈ�� 1:1���������� Dictionary 
    protected TextMeshProUGUI buttonText; // ��ư�� ����� text

    protected bool isInteracting; // ���� ���� ��ȣ�ۿ� ���ΰ�?
    public bool IsInteracting => isInteracting;
    protected Interaction interactionType; // ���� � ��ȣ�ۿ��� �ϰ��ִ°�?
    /////////////////////////////

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

    protected Vector3 moveDir;
    protected Vector3 currentDir = Vector3.zero;

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

        targetController.DoMove += Move;
        targetController.DoScreenRotate += ScreenRotate;
        targetController.DoDesignBuilding += DesignBuilding;
        targetController.DoBuild += Build;
        targetController.DoInteractionStart += InteractionStart;
        targetController.DoInteractionEnd += InteractionEnd;
        targetController.DoMouseWheel += MouseWheel;
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

        for (ResourceEnum.Prefab index = ResourceEnum.Prefab.Turret1a; index <= ResourceEnum.Prefab.ION_Cannon; index++)
        {
            int y = index - ResourceEnum.Prefab.Turret1a;
            int x = y / 5;
            y %= 5;
            buildableEnumArray[x, y] = index;
        }

        //buildableEnumArray[0, 0] = ResourceEnum.Prefab.Turret1a;
        //buildableEnumArray[0, 1] = ResourceEnum.Prefab.ION_Cannon;

    }

    public override void Spawned()
    {
        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
    }

    public override void FixedUpdateNetwork()
    {
        /////////////////////////// 
        //�̵������� ���� �� �ش� �������� ������. +�ִϸ��̼� ����
        if (moveDir.magnitude == 0)
        {
            float velocityX = Mathf.Lerp(rb.velocity.x, 0f, 0.1f);
            float velocityZ = Mathf.Lerp(rb.velocity.z, 0f, 0.1f);
            rb.velocity = new Vector3(velocityX, rb.velocity.y, velocityZ);
        }
        else
        {
            rb.velocity = (transform.forward * moveDir.z + transform.right * moveDir.x).normalized * moveSpeed;
        }

        AnimFloat?.Invoke("Speed", rb.velocity.magnitude);
        currentDir = new Vector3(Mathf.Lerp(currentDir.x, moveDir.x, 0.1f), currentDir.y, Mathf.Lerp(currentDir.z, moveDir.z, 0.1f));
        AnimFloat?.Invoke("MoveForward", currentDir.z);
        AnimFloat?.Invoke("MoveRight", currentDir.x);
        //////////////////////////

        ///////////////////////////// 
        // ���ǹ��� ��������� �ش� ���ǹ��� ��ġ�� int������ �����ִ� �κ�.
        if (DesigningBuilding != null)
        {
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

    protected override void MyUpdate(float deltaTime)
    {
        // CharacterUICanvas
        if(buildingSelectUI == null)
        {
            if(possessionController != null && possessionController.myAuthority == Runner.LocalPlayer)
            {
                interactionUI = GameObject.FindGameObjectWithTag("InteractionScrollView").transform;
                interactionContent = GameObject.FindGameObjectWithTag("InteractionContent").GetComponent<RectTransform>();
                interactionUpdateUI = GameObject.FindGameObjectWithTag("InteractionUpdateUI");
                buildingSelectUI = GameObject.FindGameObjectWithTag("BuildingSelectUI");

                interactionUpdateUI.SetActive(false);
                buildingSelectUI.SetActive(false);

            }

        }

        /////////////////////////////
        // ��ȣ�ۿ�
        if (isInteracting && interactionObject != null)
        {
            float progress = interactionObject.InteractionUpdate(deltaTime, interactionType);

            if (possessionController != null && possessionController.myAuthority == Runner.LocalPlayer)
                interactionUpdateProgress.SetValue(progress, true);

            if (progress >= 1f)
            {
                InteractionEnd();
            }
        }
        ////////////////////////////

        
    }

    // Ű���� �Է����� �÷��̾� �̵������� �����ϴ� �Լ�.
    public override void Move(Vector3 direction)
    {
        moveDir = direction.normalized;

        //_ncc.Move(direction, moveSpeed * 10);
        //AnimFloat?.Invoke("Speed", direction.magnitude);

        ////currentDir = new Vector3(Mathf.Lerp(currentDir.x, moveDir.x, 0.1f), currentDir.y, Mathf.Lerp(currentDir.z, moveDir.z, 0.1f));

        //AnimFloat?.Invoke("MoveForward", direction.z);
        //AnimFloat?.Invoke("MoveRight", direction.x);
    }

    // ���콺�� �����ӿ� ���� ī�޶� ȸ����Ű�� �Լ�.
    public virtual void ScreenRotate(Vector2 mouseDelta)
    {
        //�¿�ȸ���� ĳ���͸� ȸ��
        rotate_y = transform.eulerAngles.y + mouseDelta.x * 0.02f * 10f;
        transform.localEulerAngles = new Vector3(0f, rotate_y, 0f);

        mouseDelta_y = -mouseDelta.y * 0.02f * 10f;
        rotate_x = rotate_x + mouseDelta_y;
        rotate_x = Mathf.Clamp(rotate_x, -45f, 45f);

        // ����ȸ���� ī�޶� ȸ��
        cameraOffset_FPS.localEulerAngles = new Vector3(rotate_x, 0f, 0f);

        //rotate_y = transform.eulerAngles.y + mouseDelta.x * Runner.DeltaTime * 10f;
        //transform.localEulerAngles = new Vector3(0f, rotate_y, 0f);

        //mouseDelta_y = -mouseDelta.y * Runner.DeltaTime * 10f;
        //rotate_x += mouseDelta_y;
        //rotate_x = Mathf.Clamp(rotate_x, -45f, 45f);
        //if (cameraOffset_FPS == null)
        //{
        //    cameraOffset_FPS = transform.Find("CameraOffset");
        //}
        //cameraOffset_FPS.localEulerAngles = new Vector3(rotate_x, 0f, 0f);
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

                if (possessionController.myAuthority == Runner.LocalPlayer)
                {
                    interactionUI.gameObject.SetActive(false);
                    interactionUpdateUI.SetActive(true);
                    interactionUpdateProgress = interactionUpdateUI.GetComponentInChildren<ImgsFillDynamic>();
                    buttonText = interactionUpdateUI.GetComponentInChildren<TextMeshProUGUI>();
                    buttonText.text = $"Building...";
                }
                
                GameManager.Instance.PoolManager.Instantiate(ResourceEnum.Prefab.Hammer, sockets.FindSocket("RightHand").gameObject.transform);
                break;
        }

        Debug.Log($"{interactionObject} �� ��ȣ�ۿ�");

        return default;
    }

    public bool InteractionStart<T>(T target) where T : IInteraction
    {
        interactionObject = target;

        if (interactionObject == null) return false;

        interactionType = interactionObject.InteractionStart(this);

        switch (interactionType)
        {
            default: InteractionEnd(); break;
            case Interaction.Build:
                isInteracting = true;
                AnimBool?.Invoke("isBuild", true);
                interactionUI.gameObject.SetActive(false);
                interactionUpdateUI.SetActive(true);
                interactionUpdateProgress = interactionUpdateUI.GetComponentInChildren<ImgsFillDynamic>();
                buttonText = interactionUpdateUI.GetComponentInChildren<TextMeshProUGUI>();
                buttonText.text = $"Building...";
                GameManager.Instance.PoolManager.Instantiate(ResourceEnum.Prefab.Hammer, sockets.FindSocket("RightHand").gameObject.transform);
                break;
        }

        Debug.Log($"{interactionObject} �� ��ȣ�ۿ�");

        return default;
    }

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
        interactionObject.InteractionEnd();

        switch (interactionType)
        {
            default: break;
            case Interaction.Build: 
                AnimBool?.Invoke("isBuild", false);
                if(possessionController.myAuthority == Runner.LocalPlayer)
                {
                    interactionUI.gameObject.SetActive(true);
                    interactionUpdateUI.SetActive(false);
                    interactionUpdateProgress = null;
                }
                GameManager.Instance.PoolManager.Destroy(sockets.FindSocket("RightHand").gameObject.GetComponentInChildren<PoolingInfo>());
                break;
        }

        //interactionObject = null;
        interactionType = Interaction.None;

        return default;
    }

    // ��ȣ�ۿ� ������ ����� �����Ǿ��� �� ó��
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IInteraction target))
        {
            // �̹� �ִٸ� �߰���������
            if (interactionObjectList.Exists(inst => inst == target)) return;
            if (System.Array.Find(target.GetInteractionColliders(), col => other == col)) 
            {
                interactionObjectList.Add(target);

                //GameObject button = Instantiate(ResourceManager.Get(ResourceEnum.Prefab.InteractableObjButton), interactionContent);
                GameObject button = GameManager.Instance.PoolManager.Instantiate(ResourceEnum.Prefab.InteractableObjButton, interactionContent);
                button.transform.SetSiblingIndex(9999);  // SiblingIndex - ���� �θ��� �ڽ��߿� ���° Index�� �ִ°�
                buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
                buttonText.text = $"{other.name}";
                interactionObjectDictionary.Add(target, button);

                if (interactionObjectList.Count == 1)
                {
                    interactionIndex = 0;
                    interactionObject = target;
                    mouseLeftImage = GameManager.Instance.PoolManager.Instantiate(ResourceEnum.Prefab.MouseLeftUI, interactionUI);
                    Canvas.ForceUpdateCanvases();
                }

                UpdateInteractionUI(interactionIndex);
            }
        }
    }

    // ��ȣ�ۿ� ������ ��� ����Ʈ�� �ִ� ����� ������������ ������ �� ó��
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out IInteraction target))
        {
            if (System.Array.Find(target.GetInteractionColliders(), col => other == col))
            { 
                interactionObjectList.Remove(target);

                if (interactionObjectList.Count == 0)
                {
                    interactionIndex = -1;
                    if (isInteracting)
                    {
                        InteractionEnd();
                    }
                    interactionObject = null;
                    GameManager.Instance.PoolManager.Destroy(mouseLeftImage);
                }
                else
                {
                    interactionIndex = Mathf.Min(interactionIndex, interactionObjectList.Count - 1);
                    if (isInteracting && target == interactionObject)
                    {
                        InteractionEnd();
                    }
                    interactionObject = interactionObjectList[interactionIndex];

                    UpdateInteractionUI(interactionIndex);
                }

                if (interactionObjectDictionary.TryGetValue(target, out GameObject result))
                {
                    GameManager.Instance.PoolManager.Destroy(interactionObjectDictionary[target]);
                    interactionObjectDictionary.Remove(target);
                }
            }
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

            if (interactionIndex < interactionObjectList.Count - 4)
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

            if (interactionIndex > 4)
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
        for (int i = 0; i < interactionObjectList.Count; i++)
        {
            GameObject button = interactionObjectDictionary[interactionObjectList[i]];
            Image buttonImage = button.GetComponentInChildren<Image>();
            if (targetIndex == i)
            {
                buttonImage.color = Color.yellow;
                mouseLeftImage.transform.position = button.transform.position;
            } 
            else buttonImage.color = Color.white;
        }
    }
    }
