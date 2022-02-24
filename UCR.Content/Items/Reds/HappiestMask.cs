using MonoMod.Cil;

namespace UltimateCustomRun.Items.Reds
{
    public static class HappiestMask
    {
        public static void ChangeChance(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchBrfalse(out _),
                x => x.MatchLdcR4(7f)
            );
            c.Index += 1;
            c.Next.Operand = Main.HappiestMaskChance.Value;
        }

        public static void ChangeDuration(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcI4(30),
                x => x.MatchMul()
            );
            c.Next.Operand = Main.HappiestMaskDuration.Value;
        }

        // WHY DO THESE NOT WORK WHAT
    }
}