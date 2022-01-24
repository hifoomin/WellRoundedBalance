using MonoMod.Cil;

namespace UltimateCustomRun
{
    public static class Gasoline
    {
        public static void ChangeExplosionDamage(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchAdd(),
                x => x.MatchStloc(1),
                x => x.MatchLdcR4(1.5f)
            );
            c.Index += 2;
            c.Next.Operand = Main.GasolineExplosionDamage.Value;
        }

        public static void ChangeBurnDamage(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(1.5f),
                x => x.MatchLdcR4(1.5f)
            );
            c.Next.Operand = Main.GasolineBurnDamage.Value;
            c.Index += 1;
            c.Next.Operand = Main.GasolineBurnDamage.Value;
            // how the FUCK does this item work lmao
            // with this, it'd be 300% (+150% per stack) of burn instead of the 150% (+75% per stack) as listed on the wiki...
            // is burns damage hardcoded and gasoline just extends the DoT duration for more ticks?
        }
        public static void ChangeRadius(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(8f),
                x => x.MatchLdcR4(4f)
            );
            c.Next.Operand = Main.GasolineBaseRadius.Value;
            c.Index += 1;
            c.Next.Operand = Main.GasolineStackRadius.Value;
        }
    }
}
