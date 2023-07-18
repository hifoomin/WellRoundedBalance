using System;

namespace WellRoundedBalance.Mechanics.Scaling
{
    public class GoldScaling : MechanicBase<GoldScaling>
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

        [ConfigField("Enable New Multiplayer Gold Scaling?", "Gold Cost goes from Base Cost * Difficulty Coefficient ^ 1.25 to Base Cost * Difficulty Coefficient ^ (1 + (0.25 / Square Root(Player Count)))", true)]
        public static bool enableNew;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.RoR2.DeathRewards.OnKilledServer += DeathRewards_OnKilledServer;

            if (enableNew)
                IL.RoR2.Run.GetDifficultyScaledCost_int_float += Run_GetDifficultyScaledCost_int_float1;
        }

        private void DeathRewards_OnKilledServer(On.RoR2.DeathRewards.orig_OnKilledServer orig, DeathRewards self, DamageReport damageReport)
        {
            self.goldReward = Convert.ToUInt32(Mathf.Min(self.goldReward * baseMultiplier, baseMultiplier * ((self.goldReward / (stageDivisor + Run.instance.stageClearCount)) + Mathf.Sqrt(squareRootMultiplier * (stageAndLoopMultiplier + (Run.instance.stageClearCount * stageMultiplier + Run.instance.loopClearCount * loopMultiplier))))));
            orig(self, damageReport);
        }

        private void Run_GetDifficultyScaledCost_int_float1(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(1.25f)))
            {
                c.EmitDelegate<Func<float, float>>((orig) =>
                {
                    var players = Run.instance.participatingPlayerCount;
                    var newScaling = 1f + (0.25f / Mathf.Sqrt(players));
                    return players <= 1 ? orig : newScaling;
                });
            }
            else
            {
                Logger.LogError("Failed to apply Multiplayer Gold Scaling hook");
            }
        }
    }
}