using RoR2;
using MonoMod.Cil;

namespace UltimateCustomRun
{
    public static class BustlingFungus
    {
        public static void ChangeRadius(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.GotoNext(MoveType.Before,
                x => x.MatchCallOrCallvirt<CharacterBody>("get_radius"),
                x => x.MatchLdcR4(1.5f),
                x => x.MatchAdd(),
                x => x.MatchLdcR4(1.5f)
            );
            c.Index += 1;
            c.Next.Operand = Main.BungusRadius.Value;
            c.Index += 2;
            c.Next.Operand = Main.BungusRadiusStack.Value;
        }

        public static void ChangeHealing(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(0.25f),
                x => x.MatchStfld<HealingWard>("interval"),
                x => x.MatchLdloc(out _),
                x => x.MatchLdcR4(0.045f),
                x => x.MatchLdcR4(0.0225f)
            );
            c.Next.Operand = Main.BungusInterval.Value;
            c.Index += 3;
            c.Next.Operand = Main.BungusHealingPercent.Value;
            c.Index += 1;
            c.Next.Operand = Main.BungusHealingPercentStack.Value;
        }
        // TODO: Add Lingering
    }
}
