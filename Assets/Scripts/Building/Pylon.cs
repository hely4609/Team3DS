using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pylon : InteractableBuilding
{
    [SerializeField] protected List<RopeStruct> multiTabList;
    public List<RopeStruct> MultiTabList { get { return multiTabList; } }
    public List<bool> isSettingRopeList;
    public List<float> ropeLengthList;
    [SerializeField, Networked] public bool OnOff { get; set; } // 꺼졌는지 켜졌는지.
    public List<Pylon> attachedPylonList;
    protected override void Initialize()
    {
        ropeStruct.ropePositions.Add(transform.position);

        GameManager.Instance.BuildingManager.PylonList.Add(this);
        // 디폴트 값.
        buildingType = BuildingEnum.Pylon;
        objectName = "Pylon";
        buildingTimeMax = 1;
        size = new Vector2Int(2, 2);
        maxRopeLength = 20;
        currentRopeLength = maxRopeLength;
        multiTabList = new List<RopeStruct>();
        isSettingRopeList = new List<bool>();
        for (int i = 0; i < 4; i++)
        {
            RopeStruct ropes = new RopeStruct();
            ropes.ropePositions = new List<Vector3>();
            ropes.ropeObjects = new List<NetworkObject>();
            multiTabList.Add(ropes);

            multiTabList[i].ropePositions.Add(transform.position);
            isSettingRopeList.Add(false);
            ropeLengthList.Add(maxRopeLength);
        }
    }

    public override void Spawned()
    {
        base.Spawned();
        marker_on.SetActive(OnOff);
        marker_off.SetActive(!OnOff);
        foreach (MeshRenderer r in meshes)
        {
            r.material.SetFloat("_OnOff", OnOff ? 1f : 0f);
        }
        foreach (Pylon py in attachedPylonList)
        {
            py.TurnOnOff(OnOff);
        }
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        if (!GameManager.IsGameStart) return;

        if (hasState)
        {
            GameManager.Instance.BuildingManager.RemoveBuilding(this);
        }
    }

    public override bool FixPlace()
    {
        bool toReturn = base.FixPlace();

        foreach (var rope in multiTabList)
        {
            rope.ropePositions[0] = transform.position;
        }

        return toReturn;
    }

    public override List<Interaction> GetInteractions(Player player)
    {
        List<Interaction> currentAbleInteractions = new List<Interaction>();

        if (CompletePercent < 1)
        {
            currentAbleInteractions.Add(Interaction.Build);
            currentAbleInteractions.Add(Interaction.Demolish);
        }
        else if (player.ropeBuilding == null)
        {
            currentAbleInteractions.Add(Interaction.takeRope);
        }
        else if(player.ropeBuilding != this)
        {
            currentAbleInteractions.Add(Interaction.AttachRope);
        }


        return currentAbleInteractions;
    }

    public override Interaction InteractionStart(Player player, Interaction interactionType)
    {
        Player localPlayer = GameManager.Instance.NetworkManager.LocalController.ControlledPlayer;
        int playerID = player.PossesionController.MyNumber;

        switch (interactionType)
        {
            case Interaction.Demolish:
                GameManager.Instance.BuildingManager.supply.TotalOreAmount += cost;
                Runner.Despawn(GetComponent<NetworkObject>());
                localPlayer.RenewalInteractionUI(this, false);
                break;

            case Interaction.takeRope:
               
                Vector3 playerTransformVector3 = new Vector3((int)(player.transform.position.x), (int)(player.transform.position.y), (int)(player.transform.position.z));
                if (!isSettingRopeList[playerID] && player.ropeBuilding == null)
                {
                    OnRopeSet(playerTransformVector3, playerID);
                    player.ropeBuilding = this;
                    // 줄을 집을거임.
                    return Interaction.takeRope;
                }
                localPlayer.RenewalInteractionUI(this);
                break;

            case Interaction.AttachRope:
                AttachRope(player, playerID);
                localPlayer.RenewalInteractionUI(player.ropeBuilding);
                localPlayer.RenewalInteractionUI(this);
                break;
        }

        
        return interactionType;
        

        //int playerID = player.PossesionController.MyNumber;
        //// 완성이 아직 안됨.
        //if (CompletePercent < 1)
        //{
        //    return Interaction.Build;
        //}
        //else if (player.ropeBuilding == null)
        //{
        //    Vector3 playerTransformVector3 = new Vector3((int)(player.transform.position.x), (int)(player.transform.position.y), (int)(player.transform.position.z));
        //    if (!isSettingRopeList[playerID] && player.ropeBuilding == null)
        //    {
        //        OnRopeSet(playerTransformVector3, playerID);
        //        player.ropeBuilding = this;
        //        // 줄을 집을거임.
        //        return Interaction.takeRope;
        //    }
        //}
        //else
        //{
        //    AttachRope(player, playerID);

        //}
        //return Interaction.None;
    }
    public override void AttachRope(Player player, int number)
    {
        if (player.PossesionController.myAuthority == Runner.LocalPlayer)
        {
            player.ropeMaxDistanceSignUI.SetActive(false);
        }
        InteractableBuilding building = player.ropeBuilding;
        if (building is Tower)
        //if (building.GetType().IsSubclassOf(typeof(Tower)))
        {
            Tower tw = building as Tower;
            Vector3 thisVector3 = new Vector3((int)(transform.position.x), (int)(transform.position.y), (int)(transform.position.z));

            tw.OnRopeSet(thisVector3, number);
            marker_on.SetActive(OnOff);
            marker_off.SetActive(!OnOff);
            foreach (MeshRenderer r in meshes)
            {
                r.material.SetFloat("_OnOff", OnOff ? 1f : 0f);
            }
            foreach (Pylon py in attachedPylonList)
            {
                py.TurnOnOff(OnOff);
            }
            isSettingRopeList[number] = false;
            tw.IsRoped = true;
            if (this.OnOff)
            {
                tw.BuildingSignCanvas.SetActive(false);
            }
            tw.attachedPylon = this;
            player.CanSetRope = true;
            player.ropeBuilding = null;

            SoundManager.Play(ResourceEnum.SFX.plug_in, transform.position);
        }
        else if (building is Pylon && building != this)
        {
            Pylon py = building as Pylon;
            if (!attachedPylonList.Contains(py))
            {
                Vector3 thisVector3 = new Vector3((int)(transform.position.x), (int)(transform.position.y), (int)(transform.position.z));

                py.OnRopeSet(thisVector3, number);
                isSettingRopeList[number] = false;
                player.CanSetRope = true;
                player.ropeBuilding = null;

                SoundManager.Play(ResourceEnum.SFX.plug_in, transform.position);

                py.attachedPylonList.Add(this);
                this.attachedPylonList.Add(py);
                if (OnOff)
                {
                    foreach (Pylon listPy in attachedPylonList)
                    {
                        listPy.TurnOnOff(true);
                    }
                }
                else
                {
                    foreach (Pylon listPy in attachedPylonList)
                    {
                        if (listPy.OnOff)
                        {
                            py.TurnOnOff(true);
                            this.TurnOnOff(true);
                            break;
                        }
                    }
                }
                py.FixRope(player, number);
            }
        }

    }

    public override void ResetRope(Player player, int number)
    {
        foreach (var rope in multiTabList[number].ropeObjects)
        {
            Runner.Despawn(rope);
        }
        multiTabList[number].ropeObjects.Clear();
        multiTabList[number].ropePositions.Clear();
        multiTabList[number].ropePositions.Add(transform.position);
        ropeLengthList[number] = maxRopeLength;
        player.CanSetRope = true;
        isSettingRopeList[number] = false;
    }
    public virtual void FixRope(Player player, int number)
    {
        multiTabList[number].ropeObjects.Clear();
        multiTabList[number].ropePositions.Clear();
        multiTabList[number].ropePositions.Add(transform.position);
        ropeLengthList[number] = maxRopeLength;
        player.CanSetRope = true;
        isSettingRopeList[number] = false;
    }
    public override bool CheckRopeLength(Vector3 end, int number) // 전선을 끌수 있었나?
    {
        Vector3 start = multiTabList[number].ropePositions[multiTabList[number].ropePositions.Count - 1];
        Vector3 delta = end - start;
        if (ropeLengthList[number] > 0)
        {

            return true;
        }
        else return false;
    }
    public override void OnRopeSet(Vector3 playerPosition, int number) // 전선을 놓기. 길이랑 같은 원리.
    {
        if (HasStateAuthority)
        {
            IsSettingRope = true;
            if (multiTabList[number].ropePositions[multiTabList[number].ropePositions.Count - 1].y > playerPosition.y)
            {
                if (playerPosition.y < 4.7)
                {
                    playerPosition.y = (int)playerPosition.y - 1;
                    if (playerPosition.y < 0)
                        playerPosition.y = 0;
                }
                else
                {
                    playerPosition.y = (int)Mathf.Round(playerPosition.y);
                }
            }
            else
            {
                playerPosition.y = (int)Mathf.Round(playerPosition.y);

            }
            multiTabList[number].ropePositions.Add(playerPosition);
            CreateRope(number);
        }
    }
    public override void CreateRope(int number)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        if (multiTabList[number].ropePositions.Count >= 3 && multiTabList[number].ropePositions[multiTabList[number].ropePositions.Count - 3] == multiTabList[number].ropePositions[multiTabList[number].ropePositions.Count - 1])
        {
            multiTabList[number].ropePositions.RemoveRange(multiTabList[number].ropePositions.Count - 2, 2);
            NetworkObject target = multiTabList[number].ropeObjects[multiTabList[number].ropeObjects.Count - 1];
            ropeLengthList[number] += target.gameObject.transform.localScale.z;
            Runner.Despawn(target);
            multiTabList[number].ropeObjects.Remove(target);

            foreach (var player in players)
            {
                var playerCS = player.GetComponent<Player>();
                if (number == playerCS.PossesionController.MyNumber)
                {
                    playerCS.angleCheckVector = multiTabList[number].ropePositions[multiTabList[number].ropePositions.Count - 2];
                }
            }
            return;
        }

        Vector3 start = multiTabList[number].ropePositions[multiTabList[number].ropePositions.Count - 2];


        foreach (var player in players)
        {
            var playerCS = player.GetComponent<Player>();
            if (number == playerCS.PossesionController.MyNumber)
            {
                playerCS.angleCheckVector = start;
            }
        }

        Vector3 end = multiTabList[number].ropePositions[multiTabList[number].ropePositions.Count - 1];
        Vector3 delta = end - start;

        ropeLengthList[number] -= delta.magnitude;

        NetworkObject ropeObject = GameManager.Instance.NetworkManager.Runner.Spawn(ResourceManager.Get(ResourceEnum.Prefab.Rope), new Vector3(start.x, start.y, start.z), Quaternion.LookRotation(delta));
        multiTabList[number].ropeObjects.Add(ropeObject);
        ropeObject.transform.localScale = new Vector3(1, 1, delta.magnitude);
        ropeObject.GetComponent<Rope>().Scale = delta.magnitude;
    }
    public void TurnOnOff(bool power) //전원을 키고 끄는 함수
    {
        if (HasStateAuthority && power != OnOff)
        {
            SoundManager.Play(ResourceEnum.SFX._switch, transform.position);
            IsRoped = true;
            OnOff = power;
        }
    }

    public override void Render()
    {
        foreach (var chage in _changeDetector.DetectChanges(this))
        {
            switch (chage)
            {
                case nameof(isBuildable):
                    VisualizeBuildable();
                    break;


                case nameof(IsFixed):
                    foreach (Collider col in cols)
                    {
                        col.enabled = true;
                    }
                    break;

                case nameof(BuildingTimeCurrent):
                    {
                        foreach (MeshRenderer r in meshes)
                        {
                            r.material.SetFloat("_CompletePercent", CompletePercent);
                        }

                        if (CompletePercent >= 1)
                        {
                            foreach (MeshRenderer r in meshes)
                            {
                                r.material = completeMat;
                            }
                            TurnOnOff(false);
                            marker_designed.SetActive(false);
                            marker_off.SetActive(true);
                            buildingSignCanvas.transform.localPosition = new Vector3(0, heightMax * 0.5f / transform.localScale.y, 0);
                            buildingSignCanvas.transform.localScale /= transform.localScale.x;
                            buildingSignCanvas.GetComponent<BuildingSignCanvas>().SetRadius(size.x);
                            buildingSignCanvas.SetActive(!IsRoped);
                        }
                    }
                    break;
                case nameof(OnOff):
                    marker_on.SetActive(OnOff);
                    marker_off.SetActive(!OnOff);
                    foreach (MeshRenderer r in meshes)
                    {
                        r.material.SetFloat("_OnOff", OnOff ? 1f : 0f);
                    }
                    foreach (Pylon py in attachedPylonList)
                    {
                        py.TurnOnOff(OnOff);
                    }
                    break;
                case nameof(IsRoped):
                    buildingSignCanvas.SetActive(!IsRoped);
                    break;
            }
        }
    }
}

