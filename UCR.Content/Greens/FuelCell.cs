using MonoMod.Cil;

namespace UltimateCustomRun
{
    public static class FuelCell
    {
        public static void ChangeCDR(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchStloc(out _),
                x => x.MatchLdcR4(0.85f)
            );
            c.Index += 1;
            c.Next.Operand = 1 - Main.FuelCellCDR.Value;
        }
    }
}
