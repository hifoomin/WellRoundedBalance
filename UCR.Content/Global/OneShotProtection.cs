using BepInEx;
using R2API;
using R2API.Utils;
using RoR2;
using RoR2.Skills;
using MonoMod.Cil;

namespace UltimateCustomRun.Global
{
    public static class OneShotProtection
    {
        public static void ChangeTime(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(0.1f)
            );
            // c.Next.Operand = Main.OspTime.Value;
        }
        public static void ChangeThreshold(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchCallOrCallvirt<RoR2.CharacterBody>("set_maxJumpCount"),
                x => x.MatchLdarg(0),
                x => x.MatchLdcR4(0.1f)
            );
            c.Index += 2;
            // c.Next.Operand = Main.OspThreshold.Value;
        }
    }
}
