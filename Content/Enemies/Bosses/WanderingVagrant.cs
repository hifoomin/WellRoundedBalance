namespace WellRoundedBalance.Enemies.Bosses
{
    internal class WanderingVagrant : EnemyBase<WanderingVagrant>
    {
        public override string Name => "::: Bosses :::::: Wandering Vagrant";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.EntityStates.VagrantMonster.ChargeMegaNova.OnEnter += ChargeMegaNova_OnEnter;
        }

        private void ChargeMegaNova_OnEnter(On.EntityStates.VagrantMonster.ChargeMegaNova.orig_OnEnter orig, EntityStates.VagrantMonster.ChargeMegaNova self)
        {
            self.duration = Mathf.Max(3.5f, EntityStates.VagrantMonster.ChargeMegaNova.baseDuration / self.attackSpeedStat);
            // make nova enrage consistent kirn
            orig(self);
        }
    }
}