using RoR2;
using UnityEngine;

namespace UltimateCustomRun.Global
{
    public class GoldScaling : GlobalBase
    {
        public static float Exponent;
        public static float basediv;
        public static float stagediv;
        public static bool gsguide;
        public override string Name => ": Global :: Gold Scaling";

        public override void Init()
        {
            Exponent = ConfigOption(1f, "Gold Reward Exponent", "Vanilla is 1");
            basediv = ConfigOption(1f, "Base Gold Reward Divisor", "Vanilla is 1");
            stagediv = ConfigOption(1f, "Base Stage Divisor", "Vanilla is 1");
            gsguide = ConfigOption(true, "Gold Scaling Guide", "Gold Scaling Formula:\n(Gold Reward ^ Gold Reward Exponent) / Base Gold Reward Divisor ^\n(Stage Clear Count / Base Stage Divisor)");
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
                self.goldReward = (uint)Mathf.CeilToInt(Mathf.Pow(self.goldReward, Exponent) / Mathf.Pow(basediv, Run.instance.stageClearCount / stagediv));
                orig(self, damageReport);
            };
        }
    }
}