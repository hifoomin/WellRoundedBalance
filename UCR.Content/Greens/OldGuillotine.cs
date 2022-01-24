using MonoMod.Cil;

namespace UltimateCustomRun
{
    public static class OldGuillotine
    {
        public static void ChangeThreshold(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(13f)
            );
            c.Next.Operand = Main.OldGThreshold.Value;
        }
    }
}
