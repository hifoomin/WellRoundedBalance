namespace UltimateCustomRun.Items.Reds
{
    public class DefensiveMicrobots : ItemBase
    {
        public static float Radius;
        public static float MinimumFireFrequency;
        public static float BaseRechargeFrequency;
        public override string Name => ":: Items ::: Reds :: Defensive Microbots";
        public override string InternalPickupToken => "captainDefenseMatrix";
        public override bool NewPickup => false;
        public override string PickupText => "";

        public override string DescText => "Shoot down <style=cIsDamage>1</style> <style=cStack>(+1 per stack)</style> projectiles within <style=cIsDamage>" + Radius + "m</style> every <style=cIsDamage>" + BaseRechargeFrequency + " seconds</style>. <style=cIsUtility>Recharge rate scales with attack Speed</style>.";

        public override void Init()
        {
            Radius = ConfigOption(20f, "Range", "Vanilla is 20");
            MinimumFireFrequency = ConfigOption(10f, "Minimum Fire Frequency", "Vanilla is 10");
            BaseRechargeFrequency = ConfigOption(2f, "Base Recharge Frequency", "Vanilla is 2");
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
                EntityStates.CaptainDefenseMatrixItem.DefenseMatrixOn.baseRechargeFrequency = BaseRechargeFrequency;
                EntityStates.CaptainDefenseMatrixItem.DefenseMatrixOn.minimumFireFrequency = MinimumFireFrequency;
                EntityStates.CaptainDefenseMatrixItem.DefenseMatrixOn.projectileEraserRadius = Radius;
                orig(self);
            };
        }
    }
}