using System;

namespace WellRoundedBalance.Mechanics.Movement
{
    public class HyperbolicSpeedScaling : MechanicBase<HyperbolicSpeedScaling>
    {
        public override string Name => ":: Mechanics ::: Hyperbolic Speed Scaling";

        [ConfigField("Max Value", "This is the value that all speed increases will approach, but never reach. It is not the percentage value, but a total value in meters per second. Most survivors walk at 7m/s and sprint at 10.15m/s.", 40f)]
        public static float maxValue;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
        }

        private void CharacterBody_RecalculateStats(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchCallOrCallvirt<CharacterBody>("set_moveSpeed"),
                x => x.MatchLdarg(0),
                x => x.MatchLdarg(0),
                x => x.MatchCallOrCallvirt<CharacterBody>("get_moveSpeed")))
            {
                c.EmitDelegate<Func<float, float>>((orig) =>
                {
                    orig = Math.Min(orig, GetHyperbolic(orig, maxValue, orig));
                    return orig;
                });
            }
            else
            {
                Logger.LogError("Failed to apply Hyperbolic Speed Increase hook");
            }
        }
    }
}