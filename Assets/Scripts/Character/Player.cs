using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Player : Character
{
    protected ControllerBase possessionController;
    [SerializeField] protected Transform cameraOffset;
    public Transform CameraOffset => cameraOffset;

    // test
    [SerializeField] protected RectTransform interactContent;
    protected Dictionary<IInteraction, GameObject> interactionObjectDictionary = new Dictionary<IInteraction, GameObject>();
    protected int interactionIndex = 0;
    public List<IInteraction> interactionObjectList = new List<IInteraction>();
    public IInteraction interactionObject = null;
    protected TextMeshProUGUI buttonText;
    //

    protected GameObject bePicked;
    // protected bool isHandFree;
    protected Building designingBuilding;

    protected float rotate_x; // 마우스 이동에 따른 시점 회전 x값
    protected float rotate_y; // 마우스 이동에 따른 시점 회전 y값
    protected float mouseDelta_y; // 마우스 이동 변화량 y값

    protected Vector3 moveDir;
    protected Vector3 currentDir = Vector3.zero;

    public bool TryPossession() => possessionController == null;

    protected void RegistrationFunctions(ControllerBase targetController)
    {
        targetController.DoMove -= Move;
        targetController.DoScreenRotate -= ScreenRotate;
        targetController.DoDesignBuilding -= DesignBuiling;
        targetController.DoBuild -= Build;
        targetController.DoInteraction -= Interaction;
        targetController.DoMouseWheel -= MouseWheel;

        targetController.DoMove += Move;
        targetController.DoScreenRotate += ScreenRotate;
        targetController.DoDesignBuilding += DesignBuiling;
        targetController.DoBuild += Build;
        targetController.DoInteraction += Interaction;
        targetController.DoMouseWheel += MouseWheel;
    }
    
    protected void UnRegistrationFunction(ControllerBase targetController)
    {
        targetController.DoMove -= Move;
        targetController.DoScreenRotate -= ScreenRotate;
        targetController.DoDesignBuilding -= DesignBuiling;
        targetController.DoBuild -= Build;
        targetController.DoInteraction -= Interaction;
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
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }

        if (cameraOffset == null)
        {
            cameraOffset = transform.Find("CameraOffset");
        }
    }

    protected override void MyUpdate(float deltaTime)
    {
        // 테스트
        //if (rb == null)
        //{
        //    rb = GetComponent<Rigidbody>();
        //}
        //

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

        // 테스트
        if (designingBuilding != null)
        {
            Vector3 pickPos = transform.position + transform.forward * 5f;
            int x = (int)pickPos.x;
            int z = (int)pickPos.z;
            designingBuilding.transform.position = new Vector3(x, designingBuilding.gameObject.transform.position.y, z);

            Vector2Int currentPos = new Vector2Int(x, z);
            if (designingBuilding.TiledBuildingPos != currentPos)
            {
                designingBuilding.TiledBuildingPos = currentPos;
                designingBuilding.CheckBuild();
            }            
        }

    }

    public override void Move(Vector3 direction)
    {
        moveDir = direction.normalized;
    }

    public virtual void ScreenRotate(Vector2 mouseDelta)
    {
        rotate_y = transform.eulerAngles.y + mouseDelta.x * 0.02f * 10f;
        transform.localEulerAngles = new Vector3(0f, rotate_y, 0f);

        mouseDelta_y = -mouseDelta.y * 0.02f * 10f;
        rotate_x = rotate_x + mouseDelta_y;
        rotate_x = Mathf.Clamp(rotate_x, -45f, 45f);
        
        cameraOffset.localEulerAngles = new Vector3(rotate_x, 0f, 0f);
    }
    public bool PickUp(GameObject target) { return default; }
    public bool PutDown() { return default; }

    public bool DesignBuiling(ResourceEnum.Prefab wantBuilding)
    {
        designingBuilding = GameManager.Instance.PoolManager.Instantiate(wantBuilding).GetComponent<Building>();
        //bePicked.transform.parent = gameObject.transform;
        return default;
    }

    public bool Build() 
    {
        if (designingBuilding != null)
        {
            if (designingBuilding.FixPlace())
            {
                designingBuilding = null;
                return true;
            }
        }
        return false; 
    }
    public bool Repair(EnergyBarrierGenerator target) { return default; }

    public bool Interaction<T> (T target) where T : IInteraction
    {
        if (target == null) return false;
        Debug.Log($"{target} 과 상호작용");
        
        return default;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IInteraction target))
        {
            if (interactionObjectList.Exists(inst => inst == target)) return;

            interactionObjectList.Add(target);

            GameObject button = GameManager.Instance.PoolManager.Instantiate(ResourceEnum.Prefab.InteractableObjButton, interactContent);
            buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = $"{other.name}";
            interactionObjectDictionary.Add(target, button);

            if (interactionObjectList.Count == 1)
            {
                interactionIndex = 0;
                interactionObject = target;
            }

            ChangeInteractionUI(interactionIndex);
        }
    }

    
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out IInteraction target))
        {
            interactionObjectList.Remove(target);

            if (interactionObjectList.Count == 0)
            {
                interactionIndex = -1;
                interactionObject = null;
            }
            else
            {
                interactionIndex = Mathf.Min(interactionIndex, interactionObjectList.Count - 1);
                interactionObject = interactionObjectList[interactionIndex];

                ChangeInteractionUI(interactionIndex);
            }

            if (interactionObjectDictionary.TryGetValue(target, out GameObject result))
            {
                GameManager.Instance.PoolManager.Destroy(interactionObjectDictionary[target]);
                interactionObjectDictionary.Remove(target);
            }
        }
    }
    public void MouseWheel(Vector2 scrollDelta)
    {
        //Debug.Log($"{scrollDelta.x}, {scrollDelta.y}");
        if (interactionObjectList.Count == 0) return;
        if (scrollDelta.y == 0f) return;
        else if (scrollDelta.y > 0)
        {
            interactionIndex--;
            interactionIndex = Mathf.Max(interactionIndex, 0);
            interactionObject = interactionObjectList[interactionIndex];

            if (interactionIndex < interactionObjectList.Count - 4)
            {
                interactContent.anchoredPosition -= new Vector2(0, 12.5f);
            }

        }
        else if (scrollDelta.y < 0)
        {
            interactionIndex++;
            interactionIndex = Mathf.Min(interactionObjectList.Count - 1, interactionIndex);
            interactionObject = interactionObjectList[interactionIndex];

            if (interactionIndex > 4)
            {
                interactContent.anchoredPosition += new Vector2(0, 12.5f);
            }
        }

        ChangeInteractionUI(interactionIndex);
    }

    private void ChangeInteractionUI(int targetIndex)
    {
        for (int i = 0; i < interactionObjectList.Count; i++)
        {
            GameObject button = interactionObjectDictionary[interactionObjectList[i]];
            Image buttonImage = button.GetComponentInChildren<Image>();
            if (targetIndex == i) buttonImage.color = Color.yellow;
            else buttonImage.color = Color.white;
        }
    }
}
