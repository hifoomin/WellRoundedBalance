using System;

namespace WellRoundedBalance.Achievements.VoidFiend
{
    internal class DraggedBelow : AchievementBase
    {
        public override string Token => "completeVoidEnding";

        public override string Description => "Escape the Planetarium or complete wave 30 in Simulacrum.";

        public override string Name => ":: Achievements :: Survivor :: Dragged Below";

        public override void Hooks()
        {
            IL.RoR2.Achievements.CompleteVoidEndingAchievement.CompleteWave50ServerAchievement.OnAllEnemiesDefeatedServer += CompleteWave50ServerAchievement_OnAllEnemiesDefeatedServer;
        }

        private void CompleteWave50ServerAchievement_OnAllEnemiesDefeatedServer(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcI4(50)))
            {
                c.Index++;
                c.EmitDelegate<Func<int, int>>((useless) =>
                {
                    return 30;
                });
            }
            else
            {
                Logger.LogError("Failed to apply Void Fiend: Dragged Below Wave Count hook");
            }
        }
    }
}