namespace WellRoundedBalance.Enemies.All
{
    internal class All : EnemyBase<All>
    {
        public override string Name => "::::: All Enemies :: Faster Spawn Animation";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.EntityStates.VagrantMonster.SpawnState.OnEnter += SpawnState_OnEnter;
            On.EntityStates.GenericCharacterSpawnState.OnEnter += GenericCharacterSpawnState_OnEnter;
        }

        private void GenericCharacterSpawnState_OnEnter(On.EntityStates.GenericCharacterSpawnState.orig_OnEnter orig, EntityStates.GenericCharacterSpawnState self)
        {
            orig(self);
            if (self.duration > 2.75f)
            {
                self.duration = 2.75f;
            }
        }

        private void SpawnState_OnEnter(On.EntityStates.VagrantMonster.SpawnState.orig_OnEnter orig, EntityStates.VagrantMonster.SpawnState self)
        {
            EntityStates.VagrantMonster.SpawnState.duration = 2f;
            orig(self);
        }
    }
}