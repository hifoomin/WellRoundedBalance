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
        }

        private void SpawnState_OnEnter(On.EntityStates.LemurianBruiserMonster.SpawnState.orig_OnEnter orig, EntityStates.LemurianBruiserMonster.SpawnState self)
        {
            Util.PlaySound("Play_lemurianBruiser_spawn", self.gameObject);
            Util.PlaySound("Play_lemurianBruiser_spawn", self.gameObject);
            orig(self);
        }
    }
}