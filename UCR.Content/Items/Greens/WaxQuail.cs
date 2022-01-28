using MonoMod.Cil;

namespace UltimateCustomRun
{
    public static class WaxQuail
    {
        public static void ChangeDistance(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(10f)
            );
            c.Next.Operand = Main.WaxQuailDistance.Value;
        }
    }
}
