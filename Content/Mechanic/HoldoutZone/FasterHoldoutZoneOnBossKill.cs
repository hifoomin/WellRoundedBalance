using WellRoundedBalance.Global;

namespace WellRoundedBalance.Mechanic.HoldoutZone
{
    internal class FasterHoldoutZoneOnBossKill : GlobalBase
    {
        private bool shouldRun;
        public override string Name => "Global :::: Holdout Zone";

        public override void Hooks()
        {
            On.RoR2.HoldoutZoneController.OnDisable += HoldoutZoneController_OnDisable;
            On.RoR2.BossGroup.OnDefeatedServer += BossGroup_OnDefeatedServer;
            On.RoR2.HoldoutZoneController.Awake += HoldoutZoneController_Awake;
        }

        private void HoldoutZoneController_OnDisable(On.RoR2.HoldoutZoneController.orig_OnDisable orig, HoldoutZoneController self)
        {
            shouldRun = false;
            orig(self);
        }

        private void HoldoutZoneController_Awake(On.RoR2.HoldoutZoneController.orig_Awake orig, RoR2.HoldoutZoneController self)
        {
            orig(self);
            if (shouldRun) self.calcChargeRate += Self_calcChargeRate;
            else self.calcChargeRate -= Self_calcChargeRate;

            if (self.name.Contains("Battery"))
            {
                self.calcChargeRate += Self_calcChargeRate1;
            }
        }

        private void Self_calcChargeRate1(ref float rate)
        {
            Main.WRBLogger.LogFatal("SHOULD RUN IS TRUE");
            rate *= 1.3f;
        }

        private void Self_calcChargeRate(ref float rate)
        {
            rate *= 3f;
        }

        private void BossGroup_OnDefeatedServer(On.RoR2.BossGroup.orig_OnDefeatedServer orig, RoR2.BossGroup self)
        {
            shouldRun = true;
            orig(self);
        }
    }
}