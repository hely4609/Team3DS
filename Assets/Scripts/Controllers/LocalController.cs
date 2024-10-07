using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using ResourceEnum;
using System;

public class LocalController : ControllerBase
{
    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            OnMove(data.moveDirection);
            //OnScreenRotate(data.lookRotationDelta);
            OnInteraction(data.buttons.IsSet(MyButtons.Interaction));
            if(HasStateAuthority)
            {
                OnFarming(data.buttons.IsSet(MyButtons.Farming));
                controlledPlayer.NetworkIsFarmingPressed = data.buttons.IsSet(MyButtons.Farming);

            }

            OnMouseWheel(data.scrollbarDelta);

            if(data.buttons.IsSet(MyButtons.Cancel)) OnCancel();
            if (data.buttons.IsSet(MyButtons.Rope)) OnRope();

            //if (data.buttons.IsSet(MyButtons.Farming)) OnFarming();

            // DesignBuilding
            // ȣ��Ʈ�� OnDesignBuilding�� �Ѵ�.
            if(HasStateAuthority)
            {
                if(data.buttons.IsSet(MyButtons.Build)) OnBuild();
                OnDesignBuilding(data.selectedBuildingIndex);

            }
            
            // ������ UI�� ����
            if(data.selectedBuildingIndex != -1)
            {
                if(HasInputAuthority && controlledPlayer.IsThisPlayerCharacterUICanvasActivated)
                {
                    controlledPlayer.buildingSelectUI.SetActive(false);
                    //controlledPlayer.buildingConfirmUI.SetActive(true);
                }
                controlledPlayer.IsThisPlayerCharacterUICanvasActivated = false;
            }
        }
    }
    protected override void MyStart()
    {

    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        if(HasStateAuthority)
        {
            if (controlledPlayer.DesigningBuilding != null)
            runner.Despawn(controlledPlayer.DesigningBuilding.GetComponent<NetworkObject>());
            if (controlledPlayer.ropeBuilding != null)
            controlledPlayer.ropeBuilding.ResetRope(controlledPlayer, MyNumber);
            runner.Despawn(controlledPlayer.GetComponent<NetworkObject>());
            NetworkPhotonCallbacks.playerArray[MyNumber] = false;
        }
    }

    // ���� ��ǲ
    void Update()
    {
        
        // KeyCode.Return�� Enter��
        // OnCursorLockTogle
        //if (Input.GetKeyDown(KeyCode.Return) && HasInputAuthority)
        //{
        //    if (Cursor.lockState == CursorLockMode.Locked) Cursor.lockState = CursorLockMode.None;
        //    else Cursor.lockState = CursorLockMode.Locked;
        //}

        // OnOpenDesignBuildingUI
        //if (Input.GetKeyDown(KeyCode.B))
        //{
        //    Debug.Log($"DesiningBuilding : {controlledPlayer.DesigningBuilding}");
        //    if (controlledPlayer.DesigningBuilding == null && myAuthority == Runner.LocalPlayer)
        //        controlledPlayer.buildingSelectUI.SetActive(true);
        //}
    }

    protected void OnCursorLockTogle()
    {
        if (!GameManager.IsGameStart) return;
        if (minimap == null)
        {
            minimap = FindObjectOfType<MiniMapScript>();
        }
        if (HasInputAuthority && (minimap == null || !minimap.isLargeMapOpend))
        {
            if (Cursor.lockState == CursorLockMode.Locked) Cursor.lockState = CursorLockMode.None;
            else Cursor.lockState = CursorLockMode.Locked;
        }
    }

    protected void OnOpenDesignBuildingUI()
    {
        if (!GameManager.IsGameStart) return;
        if (controlledPlayer.DesigningBuilding == null && HasInputAuthority)
        {
            controlledPlayer.buildingSelectUI.SetActive(!controlledPlayer.buildingSelectUI.activeInHierarchy);

            if (controlledPlayer.buildingSelectUI.activeInHierarchy)
            {
                controlledPlayer.buildableEnumPageIndex = 0;
                controlledPlayer.SetPageIndexText();
                RenewBuildingImanges();
            }
            
        }
    }

    protected void RenewBuildingImanges()
    {
        if(controlledPlayer.buildingSelectUI.activeInHierarchy)
        {
            for(int i =0; i < 5; i++)
            {
                int siblingIndex = controlledPlayer.buildingSelectUIBuildingImages[i].transform.parent.GetSiblingIndex();


                Debug.Log(controlledPlayer.BuildableEnumArray[controlledPlayer.buildableEnumPageIndex, siblingIndex].ToString());
                Enum.TryParse(controlledPlayer.BuildableEnumArray[controlledPlayer.buildableEnumPageIndex, siblingIndex].ToString(), out ResourceEnum.Sprite result);

                controlledPlayer.buildingSelectUIBuildingImages[i].GetComponent<Image>().sprite = ResourceManager.Get(result);
            }
        }

    }

    protected void OnMove(Vector3 direaction)
    {
        DoMove?.Invoke(direaction);
    }

    //protected void OnMove(InputValue value)
    //{
    //    DoMove?.Invoke(value.Get<Vector3>());
    //}
    protected void OnRope()
    {
        
    }

    protected void OnScreenRotate(Vector2 mouseDelta)
    {
        
        DoScreenRotate?.Invoke(mouseDelta);
    }

    protected void OnScreenRotate(InputValue value)
    {
        DoScreenRotate?.Invoke(value.Get<Vector2>());
    }

    protected void OnPickUp() { }
    protected void OnPutDown() { }
    protected void OnDesignBuilding(int index) 
    {
        if (isFarmingKeyPressed) return;

        if (controlledPlayer == null || controlledPlayer.DesigningBuilding != null || !controlledPlayer.IsThisPlayerCharacterUICanvasActivated) return;
        DoDesignBuilding?.Invoke(index);
        

        // � �ǹ��� ������ UI�� ����ش�.
        // �׸��� �� ��ư�� ������ �ű⼭ �÷��̾��� �ǹ����⸦ �õ��Ѵ�.
        // DoDesignBuilding?.Invoke(ResourceEnum.Prefab.Turret1a);
    }
    protected void OnBuild() 
    {
        DoBuild?.Invoke();
    }

    protected void OnMouseWheel(Vector2 value)
    {
        DoMouseWheel?.Invoke(value);
        if (HasInputAuthority && minimap != null) minimap.LargeMapZoom(value.y);
    }

    protected void OnMouseWheel(InputValue value)
    {
        return;
    }
    protected void OnRepair() { }
    ////////////////////////////////////////////

    bool alreadyPressed = false;
    protected void OnInteraction(bool isPressed) 
    {
        if (controlledPlayer == null) return;

        if (isFarmingKeyPressed) return;

        if (isPressed ^ alreadyPressed)
        { 
            if(isPressed)
            {
                // �÷��̾ ���� �ڱⰡ �ϰ��ִ� ��ȣ�ۿ��� ���� �˾ƾ���.
                // ������Ʈ �Լ��� ����ؼ� �������� ����
                DoInteractionStart?.Invoke();
                alreadyPressed = true;
            }
            else
            {
                DoInteractionEnd?.Invoke();
                alreadyPressed = false;
            }
        }
    }

    protected void OnCancel()
    {
        DoCancel?.Invoke();
    }

    bool isFarmingKeyPressed = false;
    protected void OnFarming(bool isPressed)
    {
        if (!GameManager.IsGameStart) return;
        if (controlledPlayer?.DesigningBuilding != null) return;
        if (alreadyPressed) return;

        if (isPressed ^ isFarmingKeyPressed)
        {
            if (isPressed) isFarmingKeyPressed = true;
            else           isFarmingKeyPressed = false;
          
            DoFarming?.Invoke(isFarmingKeyPressed);
        }
    }

    protected void OnKeyGuide()
    {
        if (!GameManager.IsGameStart) return;
        DoKeyGuide?.Invoke();
    }

    MiniMapScript minimap;
    protected void OnLargeMap()
    {
        if (!GameManager.IsGameStart) return;
        if (HasInputAuthority)
        {
            if (minimap == null)
            {
                minimap = FindObjectOfType<MiniMapScript>();
            }
            if(minimap != null) minimap.LargeMapToggle();
        }
    }

    protected void OnBuildingPageUpDown(InputValue value)
    {
        if (!HasInputAuthority || value.Get() == null) return;
        int i = value.Get().ToString() == "1" ? 1 : -1;
        if (controlledPlayer.buildableEnumPageIndex != Mathf.Clamp(controlledPlayer.buildableEnumPageIndex + i, 0, controlledPlayer.BuildableEnumArray.GetLength(0) - 1))
        {
            controlledPlayer.buildableEnumPageIndex = Mathf.Clamp(controlledPlayer.buildableEnumPageIndex + i, 0, controlledPlayer.BuildableEnumArray.GetLength(0) - 1);
            controlledPlayer.SetPageIndexText();
            RenewBuildingImanges();
        }
       
    }
}
