namespace UltimateCustomRun
{
    public class DefensiveMicrobots : ItemBase
    {
        public static float range;
        public static float minfirefreq;
        public static float baserechargefreq;
        public override string Name => ":: Items ::: Reds :: Defensive Microbots";
        public override string InternalPickupToken => "captainDefenseMatrix";
        public override bool NewPickup => false;
        public override string PickupText => "";

        public override string DescText => "Shoot down <style=cIsDamage>1</style> <style=cStack>(+1 per stack)</style> projectiles within <style=cIsDamage>" + range + "m</style> every <style=cIsDamage>" + baserechargefreq + " seconds</style>. <style=cIsUtility>Recharge rate scales with attack speed</style>.";
        public override void Init()
        {
            range = ConfigOption(20f, "Range", "Vanilla is 20");
            minfirefreq = ConfigOption(10f, "Minimum Fire Frequency", "Vanilla is 10");
            baserechargefreq = ConfigOption(2f, "Base Recharge Frequency", "Vanilla is 2");
            base.Init();
        }

        public override void Hooks()
        {
            Changes();
        }
        public static void Changes()
        {
            On.EntityStates.CaptainDefenseMatrixItem.DefenseMatrixOn.OnEnter += (orig, self) =>
            {
                EntityStates.CaptainDefenseMatrixItem.DefenseMatrixOn.baseRechargeFrequency = baserechargefreq;
                EntityStates.CaptainDefenseMatrixItem.DefenseMatrixOn.minimumFireFrequency = minfirefreq;
                EntityStates.CaptainDefenseMatrixItem.DefenseMatrixOn.projectileEraserRadius = range;
                orig(self);
            };
        }
    }
}
