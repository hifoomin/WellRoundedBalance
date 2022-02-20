using BepInEx;
using R2API;
using R2API.Utils;
using RoR2;
using RoR2.Skills;
using MonoMod.Cil;

namespace UltimateCustomRun.Global
{
    public class OneShotProtection : GlobalBase
    {
        public static float time;
        public static float threshold;
        public override string Name => ": Global ::::::: One Shot Protection";

        public override void Init()
        {
            time = ConfigOption(0.1f, "Invincibility Time", "Vanilla is 0.1");
            threshold = ConfigOption(0.1f, "Health Threshold", "Decimal. Vanilla is 0.1");
            base.Init();
        }
        public override void Hooks()
        {
            IL.RoR2.HealthComponent.TriggerOneShotProtection += ChangeTime;
            IL.RoR2.CharacterBody.RecalculateStats += ChangeThreshold;
        }
        public static void ChangeTime(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(0.1f)
            );
            c.Next.Operand = time;
        }
        public static void ChangeThreshold(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchCallOrCallvirt<RoR2.CharacterBody>("set_maxJumpCount"),
                x => x.MatchLdarg(0),
                x => x.MatchLdcR4(0.1f)
            );
            c.Index += 2;
            c.Next.Operand = threshold;
        }
    }
}
