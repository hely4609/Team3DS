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
        Marker_Player_Me,
        Marker_Player_Other,
        Marker_Building_Designed,
        Marker_Building_On,
        Marker_Building_Off,
        Marker_Enemy,
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
