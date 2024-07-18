using ExitGames.Client.Photon.StructWrapping;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public enum BuildingEnum
{
    Tower,
    Pylon,
    Bridge,
    Barrier
}

public abstract class Building : MyComponent, IInteraction
{
    protected BuildingEnum type; // 타워 종류
    protected bool isNeedLine; // 전선이 필요한가?

    protected float buildingTimeMax; // 제작에 얼마나 걸리나
    protected float buildingTimeCurrent; // 얼마나 제작했나
    protected float completePercent; //(0~1) 제작한 퍼센트
    public float CompletePercent { get { return buildingTimeCurrent / buildingTimeMax; } 
        set { buildingTimeCurrent = buildingTimeMax * value; } }
    // 10%로 하라. 라고 들어옴.
    [SerializeField] protected MeshRenderer[] meshes;
    [SerializeField] protected Collider[] cols;

    [SerializeField] protected bool isBuildable; // 이 장소에 건설할 수 있나
    protected Vector2Int tiledBuildingPositionCurrent; // 건설하고싶은 현재 위치. 
    [SerializeField] protected Vector2Int tiledBuildingPositionLast; // 건설하고자하는 마지막 위치.
    public Vector2Int TiledBuildingPos { get { return tiledBuildingPositionLast; } set { tiledBuildingPositionLast = value; } } // 임시 코드
    // 건설 매커니즘.
    // tiledBuildingPositionLast는 생성할려고한다 하는 처음 위치.
    // 그러다가 움직이면, float형을 받아오고, 반올림을 했을 때, tiledBuildingPosiotionLast 와 다르면 그때 CheckBuild를 실행

    [SerializeField] protected Vector2Int startPos; // 시작될 포지션. 건물의 중앙값
    [SerializeField] protected Vector2Int size; // 사이즈. 건물의 xy 크기
    protected override void MyStart()
    {
        Initialize();
        HeightCheck();
    }
    protected abstract void Initialize(); // 건물의 Enum 값 지정해줘야함.
    public virtual bool CheckBuild()  // buildPos는 건설하는 타워의 왼쪽아래
    {
        isBuildable = CheckAlreadyBuild();
        
        VisualizeBuildable();
        return isBuildable;
    }
    public virtual bool CheckAlreadyBuild() // 건설하려는 건물이 다른 건물에 겹쳤는지 체크.
    {
        isBuildable = true;
        List<Building> buildingList = GameManager.Instance.BuildingManager.Buildings;
        Debug.Log($"{buildingList.Count}");
        if (buildingList.Count > 0)
        {
            foreach (Building building in buildingList)
            {
                Vector2Int distance = building.startPos - tiledBuildingPositionLast;
                Vector2Int sizeSum = (building.size + size + Vector2Int.one) / 2;
                if (Mathf.Abs(distance.x) >= sizeSum.x || Mathf.Abs(distance.y) >= sizeSum.y)
                {
                    isBuildable = true;
                }
                else
                {
                    isBuildable = false;
                    break;
                }

            }
        }
        return isBuildable;
    }

    public void VisualizeBuildable() // 건설 가능한지 화면에 표시함.
    { 
        if (isBuildable)
        {
            Debug.Log("OK");
            
                foreach(MeshRenderer render in meshes)
                {
                    render.material = ResourceManager.Get(ResourceEnum.Material.Buildable);
                }
                
            
        }
        else
        {
            Debug.Log("안됨");
            foreach (MeshRenderer render in meshes)
            { 
                render.material = ResourceManager.Get(ResourceEnum.Material.Buildunable);
            }
        }
    }
    public virtual bool FixPlace() // 건설완료
    {
        startPos = tiledBuildingPositionLast;
        if (CheckBuild())
        {
            GameManager.Instance.BuildingManager.AddBuilding(this);
            foreach (Collider col in cols)
            {
                col.enabled = true;
            }
            return true;
        }
        else
        {
            return false;
        }
    } // 위치를 고정함.


    // 상호작용을 시작함.
    // 상호작용을 할 분류를 받아와서 어떤 걸 할 지 저장.
    // 플레이어가 

    // 스타트와 엔드는 단발성.
    // 플레이어는 상호작용 키 만 누를거임. 내용은 모름.
    // 뭘 할지는 얘가 플레이어한테 알려줄거임.
    // 플레이어는 건설, 수리이면 망치를 휘두를거고, onoff 면 손만 나와서 누를거고, 납품이면 손에 있는게 사라질거임.
    // 지속 = 업데이트.

    // 아무것도 없는 깡통건물. 세우기만 하고 상호작용 없음. ex) 육교
    public virtual Interaction InteractionStart( Player player)
    {
        // 완성이 아직 안됨.
        if (completePercent < 1)
        {
            return Interaction.Build;
        }
        else
        {
            return Interaction.None;
        }
    }
    public virtual bool InteractionUpdate(float deltaTime) // 상호작용시 적용할 함수. 제작을 진행함.
    {
        BuildBuilding(deltaTime);
        
        return true;
    }

    public bool InteractionEnd()
    {
        return true;
    }

    public void BuildBuilding(float deltaTime)
    {
        // 마우스를 누르고 있으면 점점 수치가 차오름.
        // 델타 타임 만큼 자신의 buildingTimeCurrent를 올림.
        if(completePercent< 1)
        {
            buildingTimeCurrent += deltaTime;
        }

        // 마우스를 떼면 정지. 다른 곳으로 돌려도 정지.

        // 완성되면 완성본 Material로 한다.

        // 건설 완료시 
        foreach (MeshRenderer r in meshes)
        {
            r.material.SetFloat("_CompletPercent", CompletePercent);
        }

        if (completePercent >= 1)
        {
            foreach (MeshRenderer r in meshes)
                r.material = ResourceManager.Get(ResourceEnum.Material.Turret1a);
        }
    }

    // 건물의 높이를 측정하는 함수.
    // 건물을 지을 때 진행도에 따라 차오르는것을 표현하기 위함.
    protected void HeightCheck()
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        int i = 0;
        while (i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;

            i++;
        }

        Mesh mesh = new Mesh();
        mesh.CombineMeshes(combine);

        float max = mesh.bounds.max.y;
        float min = mesh.bounds.min.y;
        Debug.Log(mesh.bounds.max.y);
        Debug.Log(mesh.bounds.min.y);

        Debug.Log(mesh.bounds.max.y + Mathf.Abs(mesh.bounds.min.y));

        foreach (MeshRenderer r in meshes)
        {
            r.material.SetFloat("_HeightMin", min);
            r.material.SetFloat("_HeightMax", max);
        }
    }
}
