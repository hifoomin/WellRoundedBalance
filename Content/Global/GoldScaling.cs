using RoR2;
using UnityEngine;

namespace WellRoundedBalance.Global
{
    public class GoldScaling : GlobalBase
    {
        public override string Name => ": Global :: Gold Scaling";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            ChangeBehavior();
        }

        public static void ChangeBehavior()
        {
            On.RoR2.DeathRewards.OnKilledServer += (orig, self, damageReport) =>
            {
                self.goldReward = (uint)self.goldReward / (1 + (uint)0.25f * (uint)Run.instance.stageClearCount);
                orig(self, damageReport);
            };
        }
    }
}