using MonoMod.Cil;
using UnityEngine;
using RoR2;
using UnityEngine.AddressableAssets;
using RoR2.Projectile;

namespace UltimateCustomRun.Equipment
{
    public class VolcanicEgg : EquipmentBase
    {
        public override string Name => "::: Equipment :: Volcanic Egg";
        public override string InternalPickupToken => "fireballDash";

        public override bool NewPickup => false;

        public override bool NewDesc => true;

        public override string PickupText => "";

        public override string DescText => "Turn into a <style=cIsDamage>draconic fireball</style> for <style=cIsDamage>5</style> seconds. Deal <style=cIsDamage>500% damage</style> on impact. Detonates at the end for <style=cIsDamage>800% damage</style>.";

        public static float Duration;
        public static float DurationExtendPerImpact;
        public static float ImpactDamage;
        public static float ImpactProcCoefficient;
        public static float ExplosionDamage;
        public static float ExplosionRadius;
        public static float ExplosionProcCoefficient;
        public static int ExplosionFalloff;
        public static float Speed;

        public override void Init()
        {
            Duration = ConfigOption(5f, "Duration", "Vanilla is 5");
            DurationExtendPerImpact = ConfigOption(5f, "Duration Extension Per Impact", "Vanilla is 5");
            ImpactDamage = ConfigOption(5f, "Impact Damage", "Decimal. Vanilla is 5");
            ImpactProcCoefficient = ConfigOption(1f, "Impact Proc Coefficient", "Vanilla is 1");
            ExplosionDamage = ConfigOption(8f, "Explosion Damage", "Decimal. Vanilla is 8");
            ExplosionRadius = ConfigOption(8f, "Explosion Radius", "Vanilla is 8");
            ExplosionProcCoefficient = ConfigOption(1f, "Explosion Proc Coefficient", "Vanilla is 1");
            ExplosionFalloff = ConfigOption(3, "Falloff Type", "1 - Sweetspot, 2 - Linear, 3 - None.\nVanilla is 3");
            Speed = ConfigOption(30f, "Maximum Speed", "Vanilla is 30");
            base.Init();
        }

        public override void Hooks()
        {
            Changes();
        }

        private void Changes()
        {
            var vegg = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/FireBallDash/FireballVehicle.prefab").WaitForCompletion().GetComponent<FireballVehicle>();
            vegg.duration = Duration;
            vegg.targetSpeed = Speed;
            vegg.initialSpeed = Speed / 6f;
            vegg.acceleration = Speed * 4f;
            vegg.blastDamageCoefficient = ExplosionDamage;
            vegg.blastProcCoefficient = ExplosionProcCoefficient;
            vegg.blastRadius = ExplosionRadius;
            vegg.blastFalloffModel = ExplosionFalloff switch
            {
                1 => BlastAttack.FalloffModel.SweetSpot,
                2 => BlastAttack.FalloffModel.Linear,
                3 => BlastAttack.FalloffModel.None,
                _ => BlastAttack.FalloffModel.SweetSpot,
            };
            vegg.overlapDamageCoefficient = ImpactDamage;
            vegg.overlapProcCoefficient = ImpactProcCoefficient;
            vegg.overlapVehicleDurationBonusPerHit = DurationExtendPerImpact;
        }
    }
}