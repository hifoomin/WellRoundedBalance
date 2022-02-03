using BepInEx;
using R2API;
using R2API.Utils;
using RoR2;
using RoR2.Skills;
using MonoMod.Cil;

namespace UltimateCustomRun
{
    public static class HardlightAfterburner
    {
        public static void ChangeCharges(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdfld<RoR2.SkillLocator>("utility"),
                x => x.MatchLdloc(out _),
                x => x.MatchLdcI4(2)
            );
            c.Index += 2;
            c.Next.Operand = 1f - Main.HardlightCharges.Value;
        }
        public static void ChangeCDR(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(0.6666667f)
            );
            c.Next.Operand = 1f - Main.HardlightCDR.Value;
        }
    }
}
