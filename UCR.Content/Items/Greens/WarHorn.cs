using MonoMod.Cil;

namespace UltimateCustomRun
{
    public static class Warhorn
    {
        public static void ChangeAS(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                //x => x.MatchLdsfld("RoR2.RoR2Content/Buffs", "Energized"),
                //x => x.MatchCallOrCallvirt<RoR2.CharacterBody>("HasBuff"),
                //x => x.MatchBrfalse(out _),
                //x => x.MatchLdloc(out _),
                x => x.MatchLdcR4(0.7f)
            );
            //c.Index += 4;
            c.Next.Operand = Main.WarhornAS.Value;
        }
        public static void ChangeDuration(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcI4(8),
                x => x.MatchLdcI4(4)
            );
            c.Next.Operand = Main.WarhornDuration.Value;
            c.Index += 1;
            c.Next.Operand = Main.WarhornDurationStack.Value;
        }
    }
}
