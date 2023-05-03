namespace WellRoundedBalance.Enemies.Minibosses
{
    internal class BeetleGuard : EnemyBase<BeetleGuard>
    {
        public override string Name => ":: Minibosses :: Beetle Guard";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.EntityStates.BeetleGuardMonster.GroundSlam.OnEnter += GroundSlam_OnEnter;
            On.EntityStates.BeetleGuardMonster.FireSunder.OnEnter += FireSunder_OnEnter;
        }

        private void FireSunder_OnEnter(On.EntityStates.BeetleGuardMonster.FireSunder.orig_OnEnter orig, EntityStates.BeetleGuardMonster.FireSunder self)
        {
            if (!Main.IsInfernoDef())
                EntityStates.BeetleGuardMonster.FireSunder.baseDuration = 1.9f;
            orig(self);
        }

        private void GroundSlam_OnEnter(On.EntityStates.BeetleGuardMonster.GroundSlam.orig_OnEnter orig, EntityStates.BeetleGuardMonster.GroundSlam self)
        {
            if (!Main.IsInfernoDef())
            {
                EntityStates.BeetleGuardMonster.GroundSlam.baseDuration = 2.2f;
                EntityStates.BeetleGuardMonster.GroundSlam.damageCoefficient = 4f;
                EntityStates.BeetleGuardMonster.GroundSlam.forceMagnitude = 1800f;
            }

            orig(self);
        }
    }
}