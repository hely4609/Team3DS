using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pylon : InteractableBuilding
{
    protected int cost;
    protected List<RopeStruct> multiTabList;
    public List<RopeStruct> MultiTabList { get { return multiTabList; } }
    public List<bool> isSettingRopeList;
    public List<float> ropeLengthList;
    [SerializeField, Networked] public bool OnOff { get; set; } // 꺼졌는지 켜졌는지.
    public List<Pylon> attachedPylonList;
    protected override void Initialize()
    {
        ropeStruct.ropePositions.Add(startPos);

        GameManager.Instance.BuildingManager.PylonList.Add(this);
        // 디폴트 값.
        buildingType = BuildingEnum.Pylon;
        buildingTimeMax = 1;
        size = new Vector2Int(2, 2);
        maxRopeLength = 20;
        currentRopeLength = maxRopeLength;
        multiTabList = new List<RopeStruct>();
        isSettingRopeList = new List<bool>();
        for (int i = 0; i < 4; i++)
        {
            RopeStruct ropes = new RopeStruct();
            ropes.ropePositions = new List<Vector2>();
            ropes.ropeObjects = new List<NetworkObject>();
            multiTabList.Add(ropes);

            multiTabList[i].ropePositions.Add(startPos);
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

    public override bool FixPlace()
    {
        bool toReturn = base.FixPlace();

        foreach (var rope in multiTabList)
        {
            rope.ropePositions[0] = startPos;
        }

        return toReturn;
    }
    public override Interaction InteractionStart(Player player)
    {
        int playerID = player.PossesionController.MyNumber;
        // 완성이 아직 안됨.
        if (CompletePercent < 1)
        {
            return Interaction.Build;
        }
        else if (player.ropeBuilding == null)
        {
            Vector2 playerTransformVector2 = new Vector2((int)(player.transform.position.x), (int)(player.transform.position.z));
            if (!isSettingRopeList[playerID] && player.ropeBuilding == null)
            {
                OnRopeSet(playerTransformVector2, playerID);
                player.ropeBuilding = this;
                // 줄을 집을거임.
                return Interaction.takeRope;
            }
        }
        else
        {
            AttachRope(player, playerID);
            
        }
        return Interaction.None;
    }
    public override void AttachRope(Player player, int number)
    {
        InteractableBuilding building = player.ropeBuilding;
        if (building is Tower)
        //if (building.GetType().IsSubclassOf(typeof(Tower)))
        {
            Tower tw = building as Tower;
            Vector2 thisVector2 = new Vector2((int)(transform.position.x), (int)(transform.position.z));

            tw.OnRopeSet(thisVector2, number);
            isSettingRopeList[number] = false;
            tw.IsRoped = true;
            buildingSignCanvas.SetActive(false);
            tw.attachedPylon = this;
            player.CanSetRope = true;
            player.ropeBuilding = null;
            
            SoundManager.Play(ResourceEnum.SFX.plug_in, transform.position);
        }
        else if (building is Pylon && !building.IsRoped)
        {
            Pylon py = building as Pylon;
            if (!attachedPylonList.Contains(py))
            {
                Vector2 thisVector2 = new Vector2((int)(transform.position.x), (int)(transform.position.z));

                building.OnRopeSet(thisVector2, number);
                isSettingRopeList[number] = false;
                player.CanSetRope = true;
                player.ropeBuilding = null;

                SoundManager.Play(ResourceEnum.SFX.plug_in, transform.position);

                py.attachedPylonList.Add(this);
                this.attachedPylonList.Add(py);
                foreach (Pylon listPy in py.attachedPylonList)
                {
                    if (listPy.OnOff)
                    {
                        py.TurnOnOff(true);
                        this.TurnOnOff(true);
                        break;
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
        multiTabList[number].ropePositions.Add(startPos);
        ropeLengthList[number] = maxRopeLength;
        player.CanSetRope = true;
        isSettingRopeList[number] = false;
    }  
    public virtual void FixRope(Player player, int number)
    {
        multiTabList[number].ropeObjects.Clear();
        multiTabList[number].ropePositions.Clear();
        multiTabList[number].ropePositions.Add(startPos);
        ropeLengthList[number] = maxRopeLength;
        player.CanSetRope = true;
        isSettingRopeList[number] = false;
    }
    public override bool CheckRopeLength(Vector2 end, int number) // 전선을 끌수 있었나?
    {
        Vector2 start = multiTabList[number].ropePositions[multiTabList[number].ropePositions.Count - 1];
        Vector2 delta = end - start;
        if (ropeLengthList[number] > 0)
        {
            
            return true;
        }
        else return false;
    }
    public override void OnRopeSet(Vector2 playerPosition, int number) // 전선을 놓기. 길이랑 같은 원리.
    {
        if (HasStateAuthority)
        {
            IsSettingRope = true;
            multiTabList[number].ropePositions.Add(playerPosition);
            Debug.Log($"{multiTabList[number].ropePositions[multiTabList[number].ropePositions.Count - 1]}");

            CreateRope(number);
        }
    }
    public override void CreateRope(int number)
    {
        if (multiTabList[number].ropePositions.Count >= 3 && multiTabList[number].ropePositions[multiTabList[number].ropePositions.Count - 3] == multiTabList[number].ropePositions[multiTabList[number].ropePositions.Count - 1])
        {

            multiTabList[number].ropePositions.RemoveRange(multiTabList[number].ropePositions.Count - 2, 2);
            NetworkObject target = multiTabList[number].ropeObjects[multiTabList[number].ropeObjects.Count - 1];
            ropeLengthList[number] += target.gameObject.transform.localScale.z;
            Debug.Log($"{number}:{ropeLengthList[number]} + {target.gameObject.transform.localScale.z} 전선 길이");
            Runner.Despawn(target);
            multiTabList[number].ropeObjects.Remove(target);

            return;
        }

        Vector2 start = multiTabList[number].ropePositions[multiTabList[number].ropePositions.Count - 2];
        Vector2 end = multiTabList[number].ropePositions[multiTabList[number].ropePositions.Count - 1];
        Vector2 delta = end - start;

        ropeLengthList[number] -= delta.magnitude;
        float deltaAngle = Vector2.Angle(delta, Vector2.up);
        int deltaAngleCorrection = 1;
        if (delta.x < 0) deltaAngleCorrection = -1;
        deltaAngle = Vector2.Angle(delta, Vector2.up) * deltaAngleCorrection;

        NetworkObject ropeObject = GameManager.Instance.NetworkManager.Runner.Spawn(ResourceManager.Get(ResourceEnum.Prefab.Rope), new Vector3(start.x, 0, start.y), Quaternion.Euler(new Vector3(0, deltaAngle, 0)));
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
                            marker_on.SetActive(true);
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
                    foreach(Pylon py in attachedPylonList)
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

