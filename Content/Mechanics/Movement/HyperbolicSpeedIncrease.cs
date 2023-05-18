using MonoMod.Cil;

namespace WellRoundedBalance.Mechanics.Movement
{
    public class HyperbolicSpeedIncrease : MechanicBase<HyperbolicSpeedIncrease>
    {
        public override string Name => ":: Mechanics ::: Hyperbolic Speed Increase";

        [ConfigField("Max value", "This is the value that all speed increases will approach, but never reach.", 300f)]
        public static float notMovingInterval;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            // IL.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
        }

        private void CharacterBody_RecalculateStats(ILContext il)
        {
            
        }
    }
}