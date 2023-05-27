using RoR2.Achievements;
using System;

namespace WellRoundedBalance.Achievements.NonSpecific
{
    internal class WaxQuail : AchievementBase
    {
        public override string Token => "moveSpeed";

        public override string Description => "Reach +" + d((float)Math.Round(Mechanics.Movement.HyperbolicSpeedIncrease.maxValue / 7f, 2)) + " movespeed (includes sprinting).";

        public override string Name => ":: Achievements : Non Specific :: Going Fast Recommended";

        public override void Hooks()
        {
            IL.RoR2.Achievements.MoveSpeedAchievement.CheckMoveSpeed += MoveSpeedAchievement_CheckMoveSpeed;
        }

        private void MoveSpeedAchievement_CheckMoveSpeed(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(4f)))
            {
                c.Index++;
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<float, MoveSpeedAchievement, float>>((orig, self) =>
                {
                    return Mathf.Min(4f, Mechanics.Movement.HyperbolicSpeedIncrease.maxValue / self.localUser.cachedBody.baseMoveSpeed);
                });
            }
            else
            {
                Logger.LogError("Failed to apply Going Fast Recommended Speed hook");
            }
        }
    }
}