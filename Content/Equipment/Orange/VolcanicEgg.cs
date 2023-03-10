namespace WellRoundedBalance.Equipment.Orange
{
    public class VolcanicEgg : EquipmentBase
    {
        public override string Name => "::: Equipment :: Volcanic Egg";
        public override EquipmentDef InternalPickup => RoR2Content.Equipment.FireBallDash;

        public override string PickupText => "Transform into a speeding draconic fireball, dealing damage as you pass through enemies.";

        public override string DescText => "Turn into a <style=cIsDamage>draconic fireball</style> for <style=cIsDamage>" + duration + "</style> seconds. Deal <style=cIsDamage>" + d(impactDamage) + " damage</style> on impact. Detonates at the end for <style=cIsDamage>" + d(explosionDamage) + " damage</style>.";

        [ConfigField("Cooldown", "", 30f)]
        public static float cooldown;

        [ConfigField("Duration", "", 5f)]
        public static float duration;

        [ConfigField("Duration Extension Per Impact", "", 5f)]
        public static float durationExtensionPerImpact;

        [ConfigField("Impact Damage", "Decimal.", 5f)]
        public static float impactDamage;

        [ConfigField("Impact Proc Coefficient", "", 1f)]
        public static float impactProcCoefficient;

        [ConfigField("Explosion Damage", "", 8f)]
        public static float explosionDamage;

        [ConfigField("Explosion Radius", "", 8f)]
        public static float explosionRadius;

        [ConfigField("Explosion Proc Coefficient", "", 1f)]
        public static float explosionProcCoefficient;

        [ConfigField("Speed", "", 30f)]
        public static float speed;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            Changes();

            var Vegg = Utils.Paths.EquipmentDef.FireBallDash.Load<EquipmentDef>();
            Vegg.cooldown = cooldown;
        }

        private void Changes()
        {
            var vegg = Utils.Paths.GameObject.FireballVehicle.Load<GameObject>().GetComponent<FireballVehicle>();
            vegg.duration = duration;
            vegg.targetSpeed = speed;
            vegg.initialSpeed = speed / 6f;
            vegg.acceleration = speed * 4f;
            vegg.blastDamageCoefficient = explosionDamage;
            vegg.blastProcCoefficient = explosionProcCoefficient;
            vegg.blastRadius = explosionRadius;
            vegg.overlapDamageCoefficient = impactDamage;
            vegg.overlapProcCoefficient = impactProcCoefficient;
            vegg.overlapVehicleDurationBonusPerHit = durationExtensionPerImpact;
        }
    }
}