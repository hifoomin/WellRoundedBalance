using MonoMod.Cil;

namespace UltimateCustomRun
{
    public static class EnergyDrink
    {
        // ene is actually a 17.2% increase because its divided by the sprint mult (1.45) for some reason
        // i wanna change that later down the road but im not into cbt 
        public static void ChangeSpeed(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdloc(out _),
                x => x.MatchLdcR4(0.25f),
                x => x.MatchLdloc(out _)
            );
            c.Index += 1;
            c.Next.Operand = Main.EnergyDrinkSpeed.Value;
        }
    }
}
