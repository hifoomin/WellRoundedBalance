using MonoMod.Cil;

namespace UltimateCustomRun
{
    public static class SoldiersSyringe
    {
        public static void ChangeAS(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchConvR4(),
                x => x.MatchLdcR4(0.15f)
            );
            c.Index += 1;
            c.Next.Operand = Main.SoldiersSyringeAS.Value;
        }
    }
}
