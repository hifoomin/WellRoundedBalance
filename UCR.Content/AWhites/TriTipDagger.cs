using MonoMod.Cil;

namespace UltimateCustomRun
{
    static class TriTipDagger
    {
        public static void ChangeDamage(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before
                // to be done
            );
        }
        public static void ChangeChance(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdloc(out _),
                x => x.MatchBrtrue(out _),
                x => x.MatchLdcR4(10)
            );
            c.Index += 2;
            c.Next.Operand = Main.TriTipChance.Value;
        }
        public static void ChangeDuration(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdfld(typeof(RoR2.DamageInfo).GetField("attacker")),
                // not sure if this is the right way to do it
                x => x.MatchLdcI4(0),
                x => x.MatchLdcR4(3)
            );
            c.Index += 2;
            c.Next.Operand = Main.TriTipDuration.Value;
        }
    }
}
