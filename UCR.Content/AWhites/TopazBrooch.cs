using MonoMod.Cil;

namespace UltimateCustomRun
{
    static class TopazBrooch
    {
        public static void ChangeBarrier(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchCallOrCallvirt<RoR2.CharacterBody>("get_healthComponent"),
                x => x.MatchLdcR4(15f)
            );
            c.Index += 1;
            c.Next.Operand = Main.TopazBroochBarrier.Value;
        }
    }
}
