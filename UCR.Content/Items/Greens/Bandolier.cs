using MonoMod.Cil;

namespace UltimateCustomRun
{
    public static class Bandolier
    {
        public static void ChangeExponent(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchAdd(),
                x => x.MatchConvR4(),
                x => x.MatchLdcR4(0.33f)
            );
            c.Index += 2;
            c.Next.Operand = Main.BandolierExponent.Value;
        }
        public static void ChangeBase(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(1),
                x => x.MatchLdcR4(1),
                x => x.MatchLdloc(out _),
                x => x.MatchLdcI4(1)
            );
            c.Index += 3;
            c.Next.Operand = Main.BandolierBase.Value;
        }
    }
}
