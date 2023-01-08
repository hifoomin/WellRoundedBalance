using System;
using UnityEngine;

namespace WellRoundedBalance.Mechanic.Scaling
{
    public class GoldScaling : GlobalBase
    {
        public override string Name => ":: Mechanic : Scaling";

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
                self.goldReward = Convert.ToUInt32(Mathf.Min(self.goldReward * 0.75f, 0.75f * ((self.goldReward / (1 + Run.instance.stageClearCount)) + Mathf.Sqrt(4 * (1 + (Run.instance.stageClearCount * 15f + Run.instance.loopClearCount * 200f))))));
                orig(self, damageReport);
            };
        }
    }
}