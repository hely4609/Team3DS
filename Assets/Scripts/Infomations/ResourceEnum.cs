namespace ResourceEnum
{
    public enum Prefab
    {
        // UI
        Default, 
        ErrorWindow,
        SignInCanvas,
        SetNicknameCanvas,
        BeInvitedWindow,
        InteractableObjButton,
        CharacterUICanvas,
        MouseLeftUI,
        RoomButton,
        Minimap,
        GameOverCanvas,
        // Character
        Player,
        PlayerPrefab,
        // Building
        Road,
        EnergyBarrier,
        CornerWithBarrier,
        RoadWithBarrier,
        NoEnterWall,
        // Object
        Rope,
        PowerSupply,
        EnergyBarrierGenerator,
        Ore,
        // Monster
        Slime_Leaf,
        Slime_Viking,
        Slime_King,

        // 지을 수 있는것
        buildingStart,
        Turret1a,
        Turret1d,
        ION_Cannon,
        BlastTower,
        Bridge,
        Pylon,
        buildingEnd,

        // Effects
        Smoke1,

        // ETC
        BuildingSignCanvas,
    }

    public enum Sprite
    {
        Default,
        Turret1a,
        ION_Cannon,
        Bridge,
        Pylon,
        Turret1d,
        BlastTower,

        Battery,
        Ore,
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
        door_open = 10,
        door_close = 11,
        rope_stretching = 12,
        plug_in = 13,
        Rise03 = 14,
        // Footsteps
        footsteps_dirt_cut = 1000,
        footsteps_metal_cut = 1001,
    }
}
