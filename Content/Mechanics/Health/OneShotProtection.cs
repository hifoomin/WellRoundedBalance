using MonoMod.Cil;

namespace WellRoundedBalance.Mechanics.Health
{
    public class OneShotProtection : MechanicBase
    {
        public override string Name => ":: Mechanics :: One Shot Protection";

        [ConfigField("Percent Threshold", "Decimal.", 0.1f)]
        public static float percentThreshold;

        [ConfigField("Invincibility Duration", "", 0.5f)]
        public static float invincibilityDuration;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.HealthComponent.TriggerOneShotProtection += ChangeTime;
            CharacterBody.onBodyStartGlobal += CharacterBody_onBodyStartGlobal;
        }

        private void CharacterBody_onBodyStartGlobal(CharacterBody characterBody)
        {
            if (characterBody.oneShotProtectionFraction > 0)
            {
                characterBody.oneShotProtectionFraction = percentThreshold;
            }
        }

        public static void ChangeTime(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(0.1f)))
            {
                c.Next.Operand = invincibilityDuration;
            }
            else
            {
                Logger.LogError("Failed to apply One Shot Protection Time hook");
            }
        }
    }
}