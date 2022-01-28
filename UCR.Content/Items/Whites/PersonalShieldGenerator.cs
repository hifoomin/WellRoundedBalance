using MonoMod.Cil;

namespace UltimateCustomRun
{
    public static class PersonalShieldGenerator
    {
        public static void ChangeShieldPercent(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchConvR4(),
                x => x.MatchLdcR4(0.08f)
            );
            c.Index += 1;
            c.Next.Operand = Main.PSGPercent.Value;
        }
    }
}
