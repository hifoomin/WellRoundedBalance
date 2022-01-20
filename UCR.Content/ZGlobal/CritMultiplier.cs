using RoR2;
using MonoMod.Cil;

namespace UltimateCustomRun
{
    static class CritMultiplier
    {
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
            c.Next.Operand = Main.GlobalCritDamageMultiplier.Value;
        }
    }
}
