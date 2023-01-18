using System;
using UnityEngine;

namespace WellRoundedBalance.Mechanic.Scaling
{
    public class GoldScaling : MechanicBase
    {
        public override string Name => ":: Mechanics : Scaling";

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
                self.goldReward = Convert.ToUInt32(Mathf.Min(self.goldReward * 0.9f, 0.9f * ((self.goldReward / (2 + Run.instance.stageClearCount)) + Mathf.Sqrt(8f * (400f + (Run.instance.stageClearCount * 50f + Run.instance.loopClearCount * 100f))))));
                orig(self, damageReport);
            };
        }
    }
}