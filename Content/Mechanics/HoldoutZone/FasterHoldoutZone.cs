namespace WellRoundedBalance.Mechanics.HoldoutZone
{
    internal class FasterHoldoutZone : MechanicBase
    {
        public override string Name => ":: Mechanics :::::: Faster Holdout Zone";

        [ConfigField("Charge Rate Multiplier", "", 1.6f)]
        public static float chargeRateMultiplier;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.RoR2.HoldoutZoneController.Awake += HoldoutZoneController_Awake;
        }

        private void HoldoutZoneController_Awake(On.RoR2.HoldoutZoneController.orig_Awake orig, HoldoutZoneController self)
        {
            orig(self);
            self.calcChargeRate += Self_calcChargeRate;
        }

        private void Self_calcChargeRate(ref float rate)
        {
            rate *= chargeRateMultiplier;
        }
    }
}