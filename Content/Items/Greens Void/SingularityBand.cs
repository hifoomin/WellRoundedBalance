using MonoMod.Cil;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.AddressableAssets;
using WellRoundedBalance.Items.Greens;

namespace WellRoundedBalance.Items.VoidGreens
{
    public class SingularityBand : ItemBase
    {
        public override string Name => ":: Items :::::: Voids :: Singularity Band";
        public override string InternalPickupToken => "elementalRingVoid";

        public override string PickupText => "High damage hits also create unstable black holes. Recharges over time. <style=cIsVoid>Corrupts all Runald's and Kjaro's Bands</style>.";
        public override string DescText => "Hits that deal <style=cIsDamage>more than 400% damage</style> also fire a black hole that <style=cIsUtility>draws enemies within 15m into its center</style>. Lasts <style=cIsUtility>5</style> seconds before collapsing, dealing <style=cIsDamage>80%</style> <style=cStack>(+80% per stack)</style> TOTAL damage. Recharges every <style=cIsUtility>10</style> seconds. <style=cIsVoid>Corrupts all Runald's and Kjaro's Bands</style>.";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnHitEnemy += Changes;
        }

        private void Changes(ILContext il)
        {
            ILCursor c = new(il);

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchCallOrCallvirt(typeof(LegacyResourcesAPI), nameof(LegacyResourcesAPI.Load)),
                x => x.MatchStloc(out _),
                x => x.MatchLdcR4(1f)))
            {
                c.Index += 2;
                c.Next.Operand = 0.8f;
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
                c.Next.Operand = 10f;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Singularity Band Cooldown hook");
            }
        }
    }
}