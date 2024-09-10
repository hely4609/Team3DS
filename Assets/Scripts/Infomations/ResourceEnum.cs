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
        // Character
        Player,
        PlayerPrefab,
        // Building
        Road,
        EnergyBarrier,
        CornerWithBarrier,
        RoadWithBarrier,
        // Object
        Rope,
        PowerSupply,
        EnergyBarrierGenerator,
        Ore,
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

    public enum BGM
    {
        Silent_Partner__Whistling_Down_the_Road,
    }

    public enum SFX
    {
        None = 0,
        Cannon_1 = 1,
        Cannon_2 = 2,
        anvil_1 = 3,
        _switch = 4,
        cleaner_start = 5,
        cleaner_loop = 6,
        cleaner_end = 7,
        coin = 8,
        Wind = 9,
        // Footsteps
        footsteps_dirt_cut = 1000,
        footsteps_metal_cut = 1001,
    }
}
