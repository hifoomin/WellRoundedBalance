using MonoMod.Cil;

namespace UltimateCustomRun
{
    static class Bandolier
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

            c.GotoNext(MoveType.Before
                // to be done
                // which 1 is the 1 in 1 + item count lmao
            );
            c.Next.Operand = Main.BandolierBase.Value;
        }
    }
}
