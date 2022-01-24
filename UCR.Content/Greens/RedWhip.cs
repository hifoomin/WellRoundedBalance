using MonoMod.Cil;

namespace UltimateCustomRun
{
    public static class RedWhip
    {
        public static void ChangeSpeed(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchCallOrCallvirt<RoR2.CharacterBody>("HasBuff"),
                x => x.MatchBrfalse(out _),
                x => x.MatchLdloc(out _),
                x => x.MatchLdloc(out _),
                x => x.MatchConvR4(),
                x => x.MatchLdcR4(0.3f)
            );
            c.Index += 5;
            c.Next.Operand = Main.RedWhipSpeed.Value;
        }
    }
}
