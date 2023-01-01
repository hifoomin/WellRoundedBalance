using WellRoundedBalance.Global;

namespace WellRoundedBalance.Mechanic.HoldoutZone
{
    internal class FasterHoldoutZoneOnBossKill : GlobalBase
    {
        public static bool shouldRun;
        public override string Name => "Global :::: Holdout Zone";

        public override void Hooks()
        {
            On.RoR2.BossGroup.OnEnable += BossGroup_OnEnable;
            On.RoR2.BossGroup.OnDefeatedServer += BossGroup_OnDefeatedServer;
            On.RoR2.HoldoutZoneController.Awake += HoldoutZoneController_Awake;
        }

        private void HoldoutZoneController_Awake(On.RoR2.HoldoutZoneController.orig_Awake orig, RoR2.HoldoutZoneController self)
        {
            orig(self);
            if (shouldRun) self.calcChargeRate += Self_calcChargeRate;
            else self.calcChargeRate -= Self_calcChargeRate;
        }

        private void Self_calcChargeRate(ref float rate)
        {
            rate *= 2f;
        }

        private void BossGroup_OnEnable(On.RoR2.BossGroup.orig_OnEnable orig, RoR2.BossGroup self)
        {
            shouldRun = false;
            orig(self);
        }

        private void BossGroup_OnDefeatedServer(On.RoR2.BossGroup.orig_OnDefeatedServer orig, RoR2.BossGroup self)
        {
            shouldRun = true;
            orig(self);
        }
    }
}