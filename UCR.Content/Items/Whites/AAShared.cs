using RoR2;
using MonoMod.Cil;

namespace UltimateCustomRun
{
    public static class AAShared
    {
        public static void ChangeDangerDelay(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdfld<CharacterBody>("outOfDangerStopwatch"),
                x => x.MatchLdcR4(7f)
            );
            c.Index += 1;
            c.Next.Operand = Main.SharedDangerDelay.Value;
        }
        public static void ChangeCombatDelay(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
               x => x.MatchLdfld<CharacterBody>("outOfCombatStopwatch"),
               x => x.MatchLdcR4(5f)
            );
            c.Index += 1;
            c.Next.Operand = Main.SharedCombatDelay.Value;
        }
    }
}
