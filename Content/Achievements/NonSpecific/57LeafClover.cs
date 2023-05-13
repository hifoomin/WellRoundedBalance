using RoR2.Achievements;
using System;

namespace WellRoundedBalance.Achievements.NonSpecific
{
    internal class _57LeafClover : AchievementBase
    {
        public override string Token => "complete20Stages";

        public override string Description => "Complete 11 stages in a single run.";

        public override string Name => ":: Achievements : Non Specific :: The Long Road";

        public override void Hooks()
        {
            IL.RoR2.Achievements.Complete20StagesAchievement.OnStatsReceived += Complete20StagesAchievement_OnStatsReceived;
        }

        private void Complete20StagesAchievement_OnStatsReceived(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcI4(20)))
            {
                c.Index++;
                c.EmitDelegate<Func<int, int>>((useless) =>
                {
                    return 11;
                });
            }
            else
            {
                Logger.LogError("Failed to apply The Long Road Stage Count hook");
            }
        }
    }
}