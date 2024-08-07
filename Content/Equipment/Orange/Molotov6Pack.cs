﻿namespace WellRoundedBalance.Equipment.Orange
{
    public class Molotov6Pack : EquipmentBase<Molotov6Pack>
    {
        public override string Name => ":: Equipment :: Molotov 6 Pack";
        public override EquipmentDef InternalPickup => DLC1Content.Equipment.Molotov;

        public override string PickupText => "Throw " + molotovCount + " flaming molotovs that ignite enemies upon shattering.";

        public override string DescText => "Throw <style=cIsDamage>" + molotovCount + "</style> molotov cocktails that <style=cIsDamage>ignites</style> enemies for <style=cIsDamage>" + d(explosionDamage) + " base damage</style>. Each molotov leaves a burning area.";

        [ConfigField("Cooldown", "", 30f)]
        public static float cooldown;

        [ConfigField("Molotov Count", "", 6)]
        public static int molotovCount;

        [ConfigField("Explosion Damage", "Decimal.", 2.5f)]
        public static float explosionDamage;

        [ConfigField("Explosion Radius", "", 7f)]
        public static float explosionRadius;

        [ConfigField("Explosion Proc Coefficient", "", 1f)]
        public static float explosionProcCoefficient;

        [ConfigField("Pool Proc Coefficient", "", 0.5f)]
        public static float poolProcCoefficient;

        [ConfigField("Pool Lifetime", "", 7f)]
        public static float poolLifetime;

        [ConfigField("Disable random rotation?", "", true)]
        public static bool disableRandomRotation;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            Changes();

            var Molotov = Utils.Paths.EquipmentDef.Molotov.Load<EquipmentDef>();
            Molotov.cooldown = cooldown;
        }

        private void Changes()
        {
            var molotov = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/Molotov/MolotovClusterProjectile.prefab").WaitForCompletion().GetComponent<ProjectileImpactExplosion>();
            molotov.childrenCount = molotovCount;
            molotov.blastProcCoefficient = explosionProcCoefficient;
            molotov.blastDamageCoefficient = explosionDamage;
            molotov.blastRadius = explosionRadius;

            /*
            if (disableRandomRotation)
            {
                molotov.minAngleOffset = new Vector3(0f, 0f, 0f);
                molotov.maxAngleOffset = new Vector3(0f, 0f, 0f);
                molotov.rangeRollDegrees = 0f;
                molotov.rangePitchDegrees = 0f;

                var torque = molotov.gameObject.GetComponent<ApplyTorqueOnStart>();
                torque.randomize = false;
                torque.localTorque = new Vector3(0f, 200f, 0f);

                var single = molotov.childrenProjectilePrefab;
                var singleImpact = single.GetComponent<ProjectileImpactExplosion>();
                singleImpact.rangeRollDegrees = 0f;

                var singleTorque = single.GetComponent<ApplyTorqueOnStart>();
                singleTorque.randomize = false;
                singleTorque.localTorque = new Vector3(0f, 200f, 0f);
            }
            */

            // TODO: Make them fire in a circle of equal slices, dunno how I'd do that yet on the hook side of things

            var pool = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/Molotov/MolotovProjectileDotZone.prefab").WaitForCompletion().GetComponent<ProjectileDotZone>();
            pool.overlapProcCoefficient = poolProcCoefficient;
            pool.lifetime = poolLifetime;

            LanguageAPI.Add("EQUIPMENT_MOLOTOV_NAME", "Molotov (" + molotovCount + "-Pack)");
        }
    }
}