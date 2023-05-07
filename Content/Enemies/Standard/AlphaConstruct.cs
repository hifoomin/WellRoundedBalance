namespace WellRoundedBalance.Enemies.Standard
{
    internal class AlphaConstruct : EnemyBase<AlphaConstruct>
    {
        public override string Name => ":: Enemies :: Alpha Construct";

        [ConfigField("Should replace Blind Pests on Siphoned Forest?", "", false)]
        public static float shouldReplace;

        [ConfigField("Should spawn on Commencement?", "", false)]
        public static float shouldSpawn;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.EntityStates.MinorConstruct.Weapon.FireConstructBeam.OnEnter += FireConstructBeam_OnEnter;
            Changes();
        }

        private void FireConstructBeam_OnEnter(On.EntityStates.MinorConstruct.Weapon.FireConstructBeam.orig_OnEnter orig, EntityStates.MinorConstruct.Weapon.FireConstructBeam self)
        {
            if (!Main.IsInfernoDef())
                self.baseDuration = 0.3f;
            orig(self);
        }

        private void Changes()
        {
        }
    }
}