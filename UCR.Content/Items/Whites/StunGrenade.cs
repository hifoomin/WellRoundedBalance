using MonoMod.Cil;

namespace UltimateCustomRun
{
    public class StunGrenade : Based
    {

        public static float chance;
        public static bool stacking;

        public override string Name => ":: Items : Whites :: Stun Grenade";
        public override string InternalPickupToken => "stunChanceOnHit";
        public override bool NewPickup => false;

        public override string PickupText => "";

        public override string DescText => "<style=cIsUtility>5%</style> <style=cStack>(+5% on stack)</style> chance on hit to <style=cIsUtility>stun</style> enemies for <style=cIsUtility>2 seconds</style>.";
        public override void Init()
        {
            /*
            chance = ConfigOption(5f, "Chance", "Vanilla is 5");
            stacking = ConfigOption(false, "Stacking Type", "If set to true, use Linear\nIf set to false, use Hyperbolic.\nVanilla is false");
            */
            base.Init();
        }

        public override void Hooks()
        {
            // IL.RoR2.SetStateOnHurt.OnTakeDamageServer += ChangeBehavior;
            // IL.RoR2.SetStateOnHurt += ChangeChance;
        }
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
