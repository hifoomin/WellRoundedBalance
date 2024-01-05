namespace WellRoundedBalance.Enemies.Minibosses
{
    internal class GreaterWisp : EnemyBase<GreaterWisp>
    {
        public override string Name => ":: Minibosses :: Greater Wisp";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.EntityStates.GreaterWispMonster.ChargeCannons.OnEnter += ChargeCannons_OnEnter;
            On.EntityStates.GreaterWispMonster.FireCannons.OnEnter += FireCannons_OnEnter;
            Changes();
        }

        private void FireCannons_OnEnter(On.EntityStates.GreaterWispMonster.FireCannons.orig_OnEnter orig, EntityStates.GreaterWispMonster.FireCannons self)
        {
            if (!Main.IsInfernoDef())
                self.damageCoefficient = 3.7f;
            orig(self);
        }

        private void ChargeCannons_OnEnter(On.EntityStates.GreaterWispMonster.ChargeCannons.orig_OnEnter orig, EntityStates.GreaterWispMonster.ChargeCannons self)
        {
            if (!Main.IsInfernoDef())
                self.baseDuration = 1.75f;
            orig(self);
        }

        private void Changes()
        {
        }
    }
}