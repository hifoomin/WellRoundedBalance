using MonoMod.Cil;
using R2API;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace UltimateCustomRun.Equipment
{
    public class PreonAccumulator : EquipmentBase
    {
        public override string Name => "::: Equipment :: Preon Accumulator";
        public override string InternalPickupToken => "bfg";

        public override bool NewPickup => false;

        public override bool NewDesc => true;

        public override string PickupText => "";

        public override string DescText => "Fires preon tendrils, zapping enemies within " + ZapRange + "m for up to <style=cIsDamage>" + d(ZapDamage * ZapRate) + " damage/second</style>. On contact, detonate in an enormous " + BeegRange + "m explosion for <style=cIsDamage>" + d(BeegDamage) + " damage</style>.";

        public static float ZapRange;
        public static float ZapDamage;
        public static float ZapRate;
        public static float ZapProcCoefficient;
        public static float BeegRange;
        public static float BeegDamage;
        public static float BeegSpeed;
        public static int BeegFalloff;
        public static float BeegProcCoefficient;
        public static float AnimationSpeed;
        public static float Subcooldown;

        public override void Init()
        {
            BeegRange = ConfigOption(20f, "Explosion Radius", "Vanilla is 20");
            BeegDamage = ConfigOption(80f, "Explosion Damage", "Decimal. Vanilla is 80");
            BeegProcCoefficient = ConfigOption(1f, "Explosion Proc Coefficient", "Vanilla is 1");
            BeegSpeed = ConfigOption(20f, "Projectile Speed", "Vanilla is 20");
            BeegFalloff = ConfigOption(1, "Falloff Type", "1 - Sweetspot, 2 - Linear, 3 - None.\nVanilla is 1");
            ZapRange = ConfigOption(35f, "Tendril Radius", "Vanilla is 35");
            ZapDamage = ConfigOption(4f, "Tendril Damage", "Decimal. Vanilla is 12");
            ZapRate = ConfigOption(3f, "Tendril Fire Rate", "Vanilla is 3");
            ZapProcCoefficient = ConfigOption(0.1f, "Tendril Proc Coefficient", "Vanilla is 0.1");
            AnimationSpeed = ConfigOption(2f, "Animation Duration", "Vanilla is 2");
            Subcooldown = ConfigOption(2.2f, "Subcooldown Duration", "Vanilla is 2.2");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.EquipmentSlot.FireBfg += ChangeCooldowns;
            Changes();
        }

        private void ChangeCooldowns(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(2f),
                x => x.MatchStfld<EquipmentSlot>("bfgChargeTimer"),
                x => x.MatchLdarg(0),
                x => x.MatchLdcR4(2.2f)))
            {
                c.Next.Operand = AnimationSpeed;
                c.Index += 3;
                c.Next.Operand = Subcooldown;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Preon Accumulator Animation and SubcooldOwOn hook");
            }
        }

        private void Changes()
        {
            var bfg = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/BFG/BeamSphere.prefab").WaitForCompletion();
            bfg.GetComponent<ProjectileSimple>().desiredForwardSpeed = BeegSpeed;
            var bfg1 = bfg.GetComponent<ProjectileImpactExplosion>();
            var bfg2 = bfg.GetComponent<ProjectileProximityBeamController>();
            bfg1.blastRadius = BeegRange;
            bfg1.blastDamageCoefficient = BeegDamage / 2f;
            bfg1.blastProcCoefficient = BeegProcCoefficient;
            bfg1.falloffModel = BeegFalloff switch
            {
                1 => BlastAttack.FalloffModel.SweetSpot,
                2 => BlastAttack.FalloffModel.Linear,
                3 => BlastAttack.FalloffModel.None,
                _ => BlastAttack.FalloffModel.SweetSpot,
            };
            bfg2.attackRange = ZapRange;
            bfg2.listClearInterval = 1f / ZapRate;
            bfg2.damageCoefficient = ZapDamage / 2f;
            bfg2.procCoefficient = ZapProcCoefficient;
        }
    }
}