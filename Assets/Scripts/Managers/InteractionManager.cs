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

    protected Vector3 detectingPoint; // Ž�� ����Ʈ
    protected float detectingRange = 0.5f; // Ž�� �Ÿ�

    protected float updateTimeMax = 0.2f; // ������Ʈ �ֱ�
    protected float updateTimeCurrent; // ������Ʈ �����ð�

    protected List<IInteraction> totalInteractionObjectList = new List<IInteraction>(); // ��ü ����Ʈ
    protected List<IInteraction> interactionObjectList = new List<IInteraction>(); // ������ ��ȣ�ۿ� ������ ������ ����Ʈ
    protected List<IInteraction> updatedInteractionObjectList = new List<IInteraction>(); // �̹� ������Ʈ�� �ɸ� ����Ʈ
    protected Dictionary<IInteraction, GameObject> interactionObjectDictionary = new(); // ��ȣ�ۿ� ������ ������ ����Ʈ�� ��ưUI������Ʈ�� 1:1���������� Dictionary
    protected IInteraction interactionObject = null; // ���� ������ ��ȣ�ۿ� ���
    public IInteraction InteractionObject => interactionObject;
    protected int interactionIndex = 0; // ���� ������ ��ȣ�ۿ� ����� ����Ʈ���� ���° ����

    protected GameObject characterUICanvas;
    protected Transform interactionUI; // ��ȣ�ۿ� UI��ġ
    protected RectTransform interactionContent; // ��ȣ�ۿ��� UI����� �������� ��ġ

    //public GameObject buildingSeletUI; // ���� ���� UI
    protected GameObject interactionUpdateUI; // ��ȣ�ۿ� ������ UI
    protected ImgsFillDynamic interactionUpdateProgress; // ��ȣ�ۿ� ������ UI ä�� ����

    protected GameObject mouseLeftImage; // ���콺��Ŭ�� Image

    protected TextMeshProUGUI buttonText; // ��ư�� ����� text

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
        // �����ۿ� ���� ������Ʈ ����
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

        // �����ȿ� �ִ� ������Ʈ ����Ʈ ����
        updatedInteractionObjectList.Clear();
        foreach (var target in totalInteractionObjectList)
        {
            Debug.Log(Vector3.Distance(target.GetInteractionBounds().ClosestPoint(detectingPoint), detectingPoint));
            if (Vector3.Distance(target.GetInteractionBounds().ClosestPoint(detectingPoint), detectingPoint) <= detectingRange)
            {
                updatedInteractionObjectList.Add(target);

            }
        }

        // ��������Ʈ�� ���� �׸� �߰�
        foreach (var target in updatedInteractionObjectList)
        {
            if (!interactionObjectList.Exists(inst => inst == target))
            {
                interactionObjectList.Add(target);

                GameObject button = GameManager.Instance.PoolManager.Instantiate(ResourceEnum.Prefab.InteractableObjButton, interactionContent);
                button.transform.SetSiblingIndex(99);  // SiblingIndex - ���� �θ��� �ڽ��߿� ���° Index�� �ִ°�
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

        // ���� ���� ������ ��
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
