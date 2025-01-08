namespace WellRoundedBalance.Mechanics.HoldoutZone
{
    internal class FasterHoldoutZone : MechanicBase<FasterHoldoutZone>
    {
        public override string Name => ":: Mechanics :::::: Faster Holdout Zone";

        [ConfigField("Holdout Zone Charge Rate Multiplier", "", 1.6f)]
        public static float holdoutZoneChargeRateMultiplier;

        [ConfigField("Teleporter Charge Rate Multiplier", "", 1f)]
        public static float teleporterChargeRateMultiplier;

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
            if (self.GetComponent<TeleporterInteraction>() == null)
            {
                self.calcChargeRate += HoldoutZone;
            }
            else
            {
                self.calcChargeRate += Teleporter;
            }
        }

        private void Teleporter(ref float rate)
        {
            rate *= teleporterChargeRateMultiplier;
        }

        private void HoldoutZone(ref float rate)
        {
            rate *= teleporterChargeRateMultiplier;
        }
    }
}