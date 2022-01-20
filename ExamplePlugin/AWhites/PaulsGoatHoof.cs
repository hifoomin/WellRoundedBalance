using MonoMod.Cil;

namespace UltimateCustomRun
{
    static class PaulsGoatHoof
    {
        public static void ChangeSpeed(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchConvR4(),
                x => x.MatchLdcR4(0.14f)
            );
            c.Index += 1;
            c.Next.Operand = Main.PoofSpeed.Value;
        }
    }
}
