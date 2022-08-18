using MonoMod.Cil;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UltimateCustomRun.Items.Greens;

namespace UltimateCustomRun.Items.VoidGreens
{
    public class SingularityBand : ItemBase
    {
        public static float Damage;
        public static float Range;
        public static float DetonationTime;
        public static float Cooldown;
        public static float Radius;
        public static float ProcCoefficient;
        public static float Force;

        public override string Name => ":: Items :::::: Voids :: Singularity Band";
        public override string InternalPickupToken => "elementalRingVoid";
        public override bool NewPickup => false;
        public override string PickupText => "";
        public override string DescText => "Hits that deal <style=cIsDamage>more than " + d(KjarosBand.Threshold) + " damage</style> also fire a black hole that <style=cIsUtility>draws enemies within " + Radius + "m into its center</style>. Lasts <style=cIsUtility>" + DetonationTime + "</style> seconds before collapsing, dealing <style=cIsDamage>" + d(Damage) + "</style> <style=cStack>(+" + d(Damage) + " per stack)</style> TOTAL damage. Recharges every <style=cIsUtility>" + Cooldown + "</style> seconds. <style=cIsVoid>Corrupts all Runald's and Kjaro's Bands</style>.";

        public override void Init()
        {
            Damage = ConfigOption(1f, "Damage", "Decimal. Per Stack. Vanilla is 1");
            Cooldown = ConfigOption(20f, "Cooldown", "Vanilla is 20");
            Range = ConfigOption(15f, "Pull Radius", "Vanilla is 15");
            DetonationTime = ConfigOption(5f, "Explosion Timer", "Vanilla is 5");
            ProcCoefficient = ConfigOption(1f, "Proc Coefficient", "Vanilla is 1");
            Force = ConfigOption(-1500f, "Force", "Vanilla is -1500");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnHitEnemy += Changes;
            Changes2();
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
                c.Next.Operand = Damage;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Singularity Band Damage hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchConvR4(),
                x => x.MatchLdcR4(20f)))
            {
                c.Index += 1;
                c.Next.Operand = Cooldown;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Singularity Band Cooldown hook");
            }
        }

        private void Changes2()
        {
            var bhole = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/ElementalRingVoid/ElementalRingVoidBlackHole.prefab").WaitForCompletion();
            bhole.GetComponent<ProjectileFuse>().fuse = DetonationTime;
            var be = bhole.GetComponent<ProjectileExplosion>();
            be.blastRadius = Radius;
            be.blastProcCoefficient = ProcCoefficient;
            var br = bhole.GetComponent<RadialForce>();
            br.radius = Radius;
            br.forceMagnitude = Force;
            bhole.GetComponent<ProjectileSimple>().lifetime = DetonationTime + 5f;
        }
    }
}