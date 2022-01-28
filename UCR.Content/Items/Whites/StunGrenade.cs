using MonoMod.Cil;

namespace UltimateCustomRun
{
    public static class StunGrenade
    {
        public static void ChangeBehavior(ILContext il)
        {
            // yeah i have no clue
            ILCursor c = new ILCursor(il);
            c.GotoNext(MoveType.Before,
                x => x.MatchCallOrCallvirt(typeof(RoR2.Util), "ConvertAmplificationPercentageIntoReductionPercentage"),
                x => x.MatchLdloc(out _),
                x => x.MatchCallOrCallvirt(typeof(RoR2.Util), "CheckRoll"),
                x => x.MatchBrfalse(out _),
                x => x.MatchLdstr("Prefabs/Effects/ImpactEffects/ImpactStunGrenade")
            );
            //c.EmitDelegate<Func<>>() => { return };

            // I want it to be linear instead of hyperbolic
        }
        public static void ChangeChance(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(5f)
            );
            c.Next.Operand = Main.StunGrenadeChance;
        }
        // hooking this requires reflection?
        // PLEASE HELP TO FIX
    }
}
