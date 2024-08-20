namespace ResourceEnum
{
    public enum Prefab
    {
        // UI
        Test, 
        ErrorWindow,
        SignInCanvas,
        SetNicknameCanvas,
        BeInvitedWindow,
        InteractableObjButton,
        CharacterUICanvas,
        MouseLeftUI,
        RoomButton,
        Minimap,
        Character_Marker_Me,
        Character_Marker_Other,
        Building_Designed,
        Building_On,
        Building_Off,
        Enemy_Marker,
        // Character
        Player,
        // Building
        Road,
        EnergyBarrier,
        CornerWithBarrier,
        RoadWithBarrier,
        // Object
        Hammer,
        Rope,
        PowerSupply,
        EnergyBarrierGenerator,
        // Monster
        EnemyTest,

        // 지을 수 있는것
        buildingStart,
        Turret1a,
        ION_Cannon,
        Bridge,
        buildingEnd
    }

    public enum Material
    {
        Buildable,
        Buildunable,
        Turret1a,
        ION_Cannon,
    }
}
