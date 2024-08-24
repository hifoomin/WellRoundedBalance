namespace WellRoundedBalance.Mechanics.Teleporter
{
    internal class Discharge : MechanicBase<Discharge>
    {
        public override string Name => ":: Mechanics ::::::::::::::::: Teleporter Discharge";

        [ConfigField("Teleporter Discharge Rate", "Decimal.", 0.06f)]
        public static float dischargeRate;

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
            if (self.GetComponent<TeleporterInteraction>())
            {
                self.dischargeRate = dischargeRate;
            }
        }
    }
}