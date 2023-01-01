using RoR2;
using UnityEngine;
using WellRoundedBalance.Global;

namespace WellRoundedBalance.Mechanic.Scaling
{
    public class GoldScaling : GlobalBase
    {
        public override string Name => ": Global : Scaling";

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
                self.goldReward /= 1 + (uint)Run.instance.stageClearCount + 2 * (uint)Run.instance.loopClearCount;
                orig(self, damageReport);
            };
        }
    }
}