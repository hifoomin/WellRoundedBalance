using MonoMod.Cil;

namespace WellRoundedBalance.Items.VoidGreens
{
    public class SingularityBand : ItemBase
    {
        public override string Name => ":: Items :::::: Voids :: Singularity Band";
        public override string InternalPickupToken => "elementalRingVoid";

        public override string PickupText => "High damage hits also create unstable black holes. Recharges over time. <style=cIsVoid>Corrupts all Runald's and Kjaro's Bands</style>.";
        public override string DescText => "Hits that deal <style=cIsDamage>more than 400% damage</style> also fire a black hole that <style=cIsUtility>draws enemies within 15m into its center</style>. Lasts <style=cIsUtility>5</style> seconds before collapsing, dealing <style=cIsDamage>" + d(totalDamage) + "</style> <style=cStack>(+" + d(totalDamage) + " per stack)</style> TOTAL damage. Recharges every <style=cIsUtility>" + cooldown + "</style> seconds. <style=cIsVoid>Corrupts all Runald's and Kjaro's Bands</style>.";

        [ConfigField("Cooldown", 10f)]
        public static float cooldown;

        [ConfigField("TOTAL Damage", "Decimal.", 0.5f)]
        public static float totalDamage;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnHitEnemy += GlobalEventManager_OnHitEnemy;
        }

        private void GlobalEventManager_OnHitEnemy(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchCallOrCallvirt(typeof(LegacyResourcesAPI), nameof(LegacyResourcesAPI.Load)),
                x => x.MatchStloc(out _),
                x => x.MatchLdcR4(1f)))
            {
                c.Index += 2;
                c.Next.Operand = totalDamage;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Singularity Band Damage hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchConvR4(),
                x => x.MatchLdcR4(20f)))
            {
                c.Index += 1;
                c.Next.Operand = cooldown;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Singularity Band Cooldown hook");
            }
        }
    }
}