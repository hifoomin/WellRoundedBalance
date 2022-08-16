using RoR2.Projectile;
using UnityEngine;
using UnityEngine.AddressableAssets;
using R2API;

namespace UltimateCustomRun.Equipment
{
    public class Molotov6Pack : EquipmentBase
    {
        public override string Name => "::: Equipment :: Molotov 6 Pack";
        public override string InternalPickupToken => "molotov";

        public override bool NewPickup => true;

        public override bool NewDesc => true;

        public override string PickupText => "Throw " + Count + " flaming molotovs that ignite enemies upon shattering.";

        public override string DescText => "Throw <style=cIsDamage>" + Count + "</style> molotov cocktails that <style=cIsDamage>ignites</style> enemies for <style=cIsDamage>" + d(ExplosionDamage) + " base damage</style>. Each molotov leaves a burning area.";

        public static int Count;
        public static float ExplosionDamage;
        public static float ExplosionProcCoefficient;
        public static float ExplosionRadius;
        public static float BurnDamage;
        public static float BurnProcCoefficient;
        public static float BurnDuration;

        public override void Init()
        {
            Count = ConfigOption(6, "Molotov Count", "Vanilla is 6");
            ExplosionDamage = ConfigOption(5f, "Explosion Damage", "Decimal. Vanilla is 5");
            ExplosionRadius = ConfigOption(7f, "Explosion Radius", "Vanilla is 7");
            ExplosionProcCoefficient = ConfigOption(1f, "Explosion Proc Coefficient", "Vanilla is 1");
            BurnProcCoefficient = ConfigOption(0.5f, "Fire Pool Proc Coefficient", "Vanilla is 0.5");
            BurnDuration = ConfigOption(7f, "Fire Pool Duration", "Vanilla is 7");
            base.Init();
        }

        public override void Hooks()
        {
            Changes();
        }

        private void Changes()
        {
            var molotov = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/Molotov/MolotovClusterProjectile.prefab").WaitForCompletion().GetComponent<ProjectileImpactExplosion>();
            molotov.childrenCount = Count;
            molotov.blastProcCoefficient = ExplosionProcCoefficient;
            molotov.blastDamageCoefficient = ExplosionDamage;
            molotov.blastRadius = ExplosionRadius;

            var pool = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/Molotov/MolotovProjectileDotZone.prefab").WaitForCompletion().GetComponent<ProjectileDotZone>();
            pool.overlapProcCoefficient = BurnProcCoefficient;
            pool.lifetime = BurnDuration;

            LanguageAPI.Add("EQUIPMENT_MOLOTOV_NAME", "Molotov (" + Count + "-Pack)");
        }
    }
}