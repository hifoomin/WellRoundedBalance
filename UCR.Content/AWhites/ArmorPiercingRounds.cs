using MonoMod.Cil;

namespace UltimateCustomRun
{
    static class ArmorPiercingRounds
    {
        public static void ChangeDamage(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(1),
                x => x.MatchLdcR4(0.2f)
            );
            c.Index += 1;
            c.Next.Operand = Main.AprDamage.Value;
        }
    }
}
