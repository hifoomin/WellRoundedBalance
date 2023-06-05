namespace WellRoundedBalance.Enemies.Standard
{
    internal class AlphaConstruct : EnemyBase<AlphaConstruct>
    {
        public override string Name => ":: Enemies :: Alpha Construct";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.EntityStates.MinorConstruct.Weapon.FireConstructBeam.OnEnter += FireConstructBeam_OnEnter;
        }

        private void FireConstructBeam_OnEnter(On.EntityStates.MinorConstruct.Weapon.FireConstructBeam.orig_OnEnter orig, EntityStates.MinorConstruct.Weapon.FireConstructBeam self)
        {
            if (!Main.IsInfernoDef())
                self.baseDuration = 0.3f;
            orig(self);
        }
    }
}