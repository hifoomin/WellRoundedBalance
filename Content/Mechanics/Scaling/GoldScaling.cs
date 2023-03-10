using System;

namespace WellRoundedBalance.Mechanics.Scaling
{
    public class GoldScaling : MechanicBase
    {
        public override string Name => ":: Mechanics : Gold Scaling";

        [ConfigField("Gold Scaling", "Formula for gold reward: Minimum value between Vanilla Gold Reward * Base Multiplier and Base Multiplier * ((Vanilla Gold Reward / (Stage Divisor + Stages Cleared)) + Square Root(Square Root Multiplier * (Stage And Loop Multiplier + (Stage Clear Count * Stage Multiplier + Loop Clear Count * Loop Multiplier))))", 0.75f)]
        public static float duhDoesNothing;

        [ConfigField("Base Multiplier", "", 0.75f)]
        public static float baseMultiplier;

        [ConfigField("Loop Multiplier", "", 100f)]
        public static float loopMultiplier;

        [ConfigField("Stage Divisor", "", 2f)]
        public static float stageDivisor;

        [ConfigField("Stage Multiplier", "", 50f)]
        public static float stageMultiplier;

        [ConfigField("Square Root Multiplier", "", 8f)]
        public static float squareRootMultiplier;

        [ConfigField("Stage and Loop Multiplier", "", 400f)]
        public static float stageAndLoopMultiplier;

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
                self.goldReward = Convert.ToUInt32(Mathf.Min(self.goldReward * baseMultiplier, baseMultiplier * ((self.goldReward / (stageDivisor + Run.instance.stageClearCount)) + Mathf.Sqrt(squareRootMultiplier * (stageAndLoopMultiplier + (Run.instance.stageClearCount * stageMultiplier + Run.instance.loopClearCount * loopMultiplier))))));
                orig(self, damageReport);
            };
        }
    }
}