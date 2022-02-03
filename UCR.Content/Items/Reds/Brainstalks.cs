using MonoMod.Cil;

namespace UltimateCustomRun
{
    public static class Brainstalks
    {
        public static void ChangeDuration(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdsfld("RoR2.RoR2Content/Buffs", "NoCooldowns"),
                x => x.MatchLdloc(out _),
                x => x.MatchConvR4(),
                x => x.MatchLdcR4(4f)
            );
            c.Index += 3;
            c.Next.Operand = Main.BrainstalksDuration.Value;
        }
    }
}
