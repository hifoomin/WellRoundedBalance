namespace WellRoundedBalance.Enemies.Minibosses
{
    internal class ElderLemurian : EnemyBase<ElderLemurian>
    {
        public override string Name => ":: Minibosses :: Elder Lemurian";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.EntityStates.LemurianBruiserMonster.SpawnState.OnEnter += SpawnState_OnEnter;
            On.EntityStates.LemurianBruiserMonster.Flamebreath.OnEnter += Flamebreath_OnEnter;
        }

        private void Flamebreath_OnEnter(On.EntityStates.LemurianBruiserMonster.Flamebreath.orig_OnEnter orig, EntityStates.LemurianBruiserMonster.Flamebreath self)
        {
            if (!Main.IsInfernoDef())
            {
                EntityStates.LemurianBruiserMonster.Flamebreath.maxSpread = 5f;
                EntityStates.LemurianBruiserMonster.Flamebreath.radius = 9f;
            }
            orig(self);
        }

        private void SpawnState_OnEnter(On.EntityStates.LemurianBruiserMonster.SpawnState.orig_OnEnter orig, EntityStates.LemurianBruiserMonster.SpawnState self)
        {
            Util.PlaySound("Play_lemurianBruiser_spawn", self.gameObject);
            Util.PlaySound("Play_lemurianBruiser_spawn", self.gameObject);
            Util.PlaySound("Play_lemurianBruiser_spawn", self.gameObject);
            orig(self);
        }
    }
}