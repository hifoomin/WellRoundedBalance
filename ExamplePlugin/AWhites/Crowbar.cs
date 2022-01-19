using RoR2;
using MonoMod.Cil;

namespace UltimateCustomRun
{
    static class Crowbar
    {
        public static void ChangeDamage(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(1f),
                x => x.MatchLdcR4(0.75f)
            );
            c.Index += 1;
            c.Next.Operand = Main.CrowbarDamage.Value;
        }
        public static void ChangeThreshold(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.GotoNext(MoveType.Before,
                x => x.MatchLdarg(0),
                x => x.MatchCallOrCallvirt<HealthComponent>("get_fullCombinedHealth"),
                x => x.MatchLdcR4(0.9f)
            );
            c.Index += 2;
            c.Next.Operand = Main.CrowbarThreshold.Value;
        }
    }
}
