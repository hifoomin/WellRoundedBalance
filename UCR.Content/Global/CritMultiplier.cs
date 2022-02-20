using RoR2;
using MonoMod.Cil;

namespace UltimateCustomRun.Global
{
    public class CritMultiplier : GlobalBase
    {
        public static float cdm;
        public override string Name => ": Global ::: Crit Damage Multiplier";

        public override void Init()
        {
            cdm = ConfigOption(2f, "Crit Damage Multiplier", "Vanilla is 2");
            base.Init();
        }
        public override void Hooks()
        {
            IL.RoR2.HealthComponent.TakeDamage += ChangeDamage;
        }

        public static void ChangeDamage(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.GotoNext(MoveType.Before,
                x => x.MatchLdfld<DamageInfo>("crit"),
                x => x.MatchBrfalse(out _),
                x => x.MatchLdloc(out _),
                x => x.MatchLdcR4(2f)
            );
            c.Index += 3;
            c.Next.Operand = cdm;
        }
    }
}
