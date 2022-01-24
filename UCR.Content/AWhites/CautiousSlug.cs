using MonoMod.Cil;

namespace UltimateCustomRun
{
    public static class CautiousSlug
    {
        public static void ChangeHealing(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(0),
                x => x.MatchBr(out _),
                x => x.MatchLdcR4(3)
            );
            c.Index += 2;
            c.Next.Operand = Main.CautiousSlugHealing.Value;
        }
    }
}
