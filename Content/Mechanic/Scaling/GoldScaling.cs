using RoR2;
using System;
using UnityEngine;
using WellRoundedBalance.Global;

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
                self.goldReward = Convert.ToUInt32(0.75f * ((self.goldReward / (1 + Run.instance.stageClearCount)) + Mathf.Sqrt(4 * (1 + (Run.instance.stageClearCount * 2.2f)))));
                orig(self, damageReport);
            };
        }
    }
}