using System.Collections;

namespace WellRoundedBalance.Mechanics.Teleporter
{
    internal class RadiusShrinking : MechanicBase<RadiusShrinking>
    {
        public override string Name => ":: Mechanics ::::::::::::::::: Teleporter Charge Radius Shrinking";

        [ConfigField("Maximum Teleporter Radius Shrink Percent", "Decimal.", 35f)]
        public static float maxRadiusShrink;

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
                self.chargeRadiusDelta = -maxRadiusShrink;
            }
        }
    }
}