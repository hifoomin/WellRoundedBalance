using MonoMod.Cil;
using RoR2;

namespace UltimateCustomRun.Global
{
    public class CritDamageMultiplier : GlobalBase
    {
        public static float CritDamageMarketPlier;
        public override string Name => ": Global :::: Damage";

        public override void Init()
        {
            CritDamageMarketPlier = ConfigOption(2f, "Crit Damage Multiplier", "Vanilla is 2");
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
            c.Next.Operand = CritDamageMarketPlier;
        }
    }
}