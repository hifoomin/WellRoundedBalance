using MonoMod.Cil;

namespace UltimateCustomRun.Global
{
    public static class Luck
    {
        public static void ChangeLuck(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(0.0f)
            );
            c.Next.Operand = Main.LuckBase.Value;
        }
    }
}
