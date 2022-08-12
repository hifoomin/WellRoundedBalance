using MonoMod.Cil;

namespace UltimateCustomRun.Global
{
    public class OneShotProtection : GlobalBase
    {
        public static float Time;
        public static float Threshold;
        public override string Name => ": Global ::: Health";

        public override void Init()
        {
            Time = ConfigOption(0.1f, "OSP Invincibility Time", "Vanilla is 0.1");
            Threshold = ConfigOption(0.1f, "OSP Health Threshold", "Decimal. Vanilla is 0.1");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.HealthComponent.TriggerOneShotProtection += ChangeTime;
            IL.RoR2.CharacterBody.RecalculateStats += ChangeThreshold;
        }

        public static void ChangeTime(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(0.1f)))
            {
                c.Next.Operand = Time;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply One Shot Protection Time hook");
            }
        }

        public static void ChangeThreshold(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchCallOrCallvirt<RoR2.CharacterBody>("set_maxJumpCount"),
                    x => x.MatchLdarg(0),
                    x => x.MatchLdcR4(0.1f)))
            {
                c.Index += 2;
                c.Next.Operand = Threshold;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply One Shot Protection Threshold hook");
            }
        }
    }
}