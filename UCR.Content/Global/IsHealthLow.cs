using MonoMod.Cil;

namespace UltimateCustomRun
{
    public static class IsHealthLow
    {
        public static void ChangeThreshold(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(0.25f)
            );
            c.Next.Operand = Main.GlobalLowHealthThreshold.Value;
        }
        // HOOKING THIS REQUIRES REFLECTION?
        // PLEASE HELP TO FIX
    }
}
