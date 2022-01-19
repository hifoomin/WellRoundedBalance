using RoR2;
using MonoMod.Cil;

namespace UltimateCustomRun
{
    static class LensMakersGlasses
    {
        public static void ChangeCrit(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.GotoNext(MoveType.Before,
                x => x.MatchLdfld<CharacterBody>("levelCrit")
            );
            c.Index += 8;
            c.Next.Operand = Main.LensMakersCrit.Value;
            // I dont actually know why the standard method doesnt work
            // Thanks to uhh someone for this instead
        }
    }
}
