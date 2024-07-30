using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class InteractionManager : Manager
{
    protected Socket.Container sockets = new Socket.Container();
    protected Player _controlledPlayer;
    public Player ControlledPlayer
    {
        get => _controlledPlayer;
        set
        {
            _controlledPlayer = value;
            detectingPoint = new Vector3(_controlledPlayer.transform.position.x, 0f, _controlledPlayer.transform.position.z + 0.5f);
        }
    }

    protected Vector3 detectingPoint; // 탐지 포인트
    protected float detectingRange = 0.5f; // 탐지 거리

    protected float updateTimeMax = 0.2f; // 업데이트 주기
    protected float updateTimeCurrent; // 업데이트 남은시간

    protected List<IInteraction> totalInteractionObjectList = new List<IInteraction>(); // 전체 리스트
    protected List<IInteraction> interactionObjectList = new List<IInteraction>(); // 범위내 상호작용 가능한 대상들의 리스트
    protected List<IInteraction> updatedInteractionObjectList = new List<IInteraction>(); // 이번 업데이트에 걸린 리스트
    protected Dictionary<IInteraction, GameObject> interactionObjectDictionary = new(); // 상호작용 가능한 대상들의 리스트와 버튼UI오브젝트를 1:1대응시켜줄 Dictionary
    protected IInteraction interactionObject = null; // 내가 선택한 상호작용 대상
    public IInteraction InteractionObject => interactionObject;
    protected int interactionIndex = 0; // 내가 선택한 상호작용 대상이 리스트에서 몇번째 인지

    protected GameObject characterUICanvas;
    protected Transform interactionUI; // 상호작용 UI위치
    protected RectTransform interactionContent; // 상호작용대상 UI띄워줄 컨텐츠의 위치

    //public GameObject buildingSeletUI; // 빌딩 선택 UI
    protected GameObject interactionUpdateUI; // 상호작용 진행중 UI
    protected ImgsFillDynamic interactionUpdateProgress; // 상호작용 진행중 UI 채울 정도

    protected GameObject mouseLeftImage; // 마우스좌클릭 Image

    protected TextMeshProUGUI buttonText; // 버튼에 띄워줄 text

    public override IEnumerator Initiate()
    {
        if (characterUICanvas != null) yield break;

        characterUICanvas = GameManager.Instance.PoolManager.Instantiate(ResourceEnum.Prefab.CharacterUICanvas);
        foreach (var socket in characterUICanvas.GetComponentsInChildren<Socket>())
        {
            sockets.AttachSocket(socket);
        }

        interactionContent = sockets.FindSocket("Content").GetComponent<RectTransform>();
        interactionUI = sockets.FindSocket("InteractionUI").transform;

        yield return null;
    }

    public override void ManagerStart()
    {

    }

    public override void ManagerUpdate(float deltaTime)
    {
        if (updateTimeCurrent <= 0f)
        {
            UpdateInteractionList();
            //Debug.Log(interactionObjectList.Count);
            updateTimeCurrent = updateTimeMax;
        }
        else
        {
            updateTimeCurrent -= deltaTime;
        }
    }

    public void UpdateInteractionList()
    {
        // 범위밖에 나간 오브젝트 제거
        var targetList = interactionObjectList.FindAll(target => detectingRange < Vector3.Distance(target.GetInteractionBounds().ClosestPoint(detectingPoint), detectingPoint));

        foreach (var target in targetList)
        {
            interactionObjectList.Remove(target);

            if (interactionObjectList.Count == 0)
            {
                interactionIndex = -1;
                if (_controlledPlayer.IsInteracting)
                {
                    _controlledPlayer.InteractionEnd();
                }
                interactionObject = null;
                GameManager.Instance.PoolManager.Destroy(mouseLeftImage);
            }
            else
            {
                interactionIndex = Mathf.Min(interactionIndex, interactionObjectList.Count - 1);
                if (_controlledPlayer.IsInteracting && target == interactionObject)
                {
                    _controlledPlayer.InteractionEnd();
                }
                interactionObject = interactionObjectList[interactionIndex];

                UpdateInteractionUI(interactionIndex);
            }

            if (interactionObjectDictionary.TryGetValue(target, out GameObject result))
            {
                GameManager.Instance.PoolManager.Destroy(result);
                interactionObjectDictionary.Remove(target);
            }
        }

        // 범위안에 있는 오브젝트 리스트 갱신
        updatedInteractionObjectList.Clear();
        foreach (var target in totalInteractionObjectList)
        {
            Debug.Log(Vector3.Distance(target.GetInteractionBounds().ClosestPoint(detectingPoint), detectingPoint));
            if (Vector3.Distance(target.GetInteractionBounds().ClosestPoint(detectingPoint), detectingPoint) <= detectingRange)
            {
                updatedInteractionObjectList.Add(target);

            }
        }

        // 기존리스트에 없는 항목 추가
        foreach (var target in updatedInteractionObjectList)
        {
            if (!interactionObjectList.Exists(inst => inst == target))
            {
                interactionObjectList.Add(target);

                GameObject button = GameManager.Instance.PoolManager.Instantiate(ResourceEnum.Prefab.InteractableObjButton, interactionContent);
                button.transform.SetSiblingIndex(99);  // SiblingIndex - 나는 부모의 자식중에 몇번째 Index에 있는가
                buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
                buttonText.text = $"{target.GetName()}";
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

    public void OnMouseWheel(InputValue value)
    {
        Vector2 scrollDelta = value.Get<Vector2>();

        if (interactionObjectList.Count == 0) return;
        if (scrollDelta.y == 0f) return;

        // 휠을 위로 굴렸을 때
        else if (scrollDelta.y > 0)
        {
            interactionIndex--;
            interactionIndex = Mathf.Max(interactionIndex, 0);
            interactionObject = interactionObjectList[interactionIndex];

            if (interactionIndex < interactionObjectList.Count - 4)
            {
                interactionContent.anchoredPosition -= new Vector2(0, 50f);
                interactionContent.anchoredPosition = new Vector2(0, Mathf.Clamp(interactionContent.anchoredPosition.y, 0, (interactionObjectList.Count - 6) * 50f));
            }
        }
        // 휠을 아래로 굴렸을 때
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
                Canvas.ForceUpdateCanvases();
            }
            else buttonImage.color = Color.white;
        }
    }

    public void AddInteractionObject(IInteraction addInteractionObject)
    {
        totalInteractionObjectList.Add(addInteractionObject);
    }

    public void RemoveInteractionObject(IInteraction removeInteractionObject) 
    {
        totalInteractionObjectList.Remove(removeInteractionObject);
    }
        
}
