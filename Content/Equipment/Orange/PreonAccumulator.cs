namespace WellRoundedBalance.Equipment
{
    public class PreonAccumulator : EquipmentBase
    {
        public override string Name => "::: Equipment :: Preon Accumulator";
        public override string InternalPickupToken => "bfg";

        public override string PickupText => "Fire a ball of energy that electrocutes nearby enemies before detonating.";

        public override string DescText => "Fires preon tendrils, zapping enemies within " + tendrilRange + "m for up to <style=cIsDamage>" + d(tendrilDamage * tendrilFireRate) + " damage/second</style>. On contact, detonate in an enormous " + bigBallRange + "m explosion for <style=cIsDamage>" + d(bigBallDamage) + " damage</style>.";

        [ConfigField("Cooldown", "", 120f)]
        public static float cooldown;

        [ConfigField("Big Ball Damage", "Decimal.", 80f)]
        public static float bigBallDamage;

        [ConfigField("Big Ball Range", "", 20f)]
        public static float bigBallRange;

        [ConfigField("Big Ball Proc Coefficient", "", 1f)]
        public static float bigBallProcCoefficient;

        [ConfigField("Big Ball Speed", "", 20f)]
        public static float bigBallSpeed;

        [ConfigField("Tendril Range", "", 20f)]
        public static float tendrilRange;

        [ConfigField("Tendril Damage", "Decimal.", 3.99f)]
        public static float tendrilDamage;

        [ConfigField("Tendril Fire Rate", "", 3f)]
        public static float tendrilFireRate;

        [ConfigField("Tendril Proc Coefficient", "", 0.1f)]
        public static float tendrilProcCoefficient;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            Changes();

            var BFG = Addressables.LoadAssetAsync<EquipmentDef>("RoR2/Base/BFG/BFG.asset").WaitForCompletion();
            BFG.cooldown = cooldown;
        }

        private void Changes()
        {
            var bfg = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/BFG/BeamSphere.prefab").WaitForCompletion();
            bfg.GetComponent<ProjectileSimple>().desiredForwardSpeed = bigBallSpeed;
            var bfg1 = bfg.GetComponent<ProjectileImpactExplosion>();
            var bfg2 = bfg.GetComponent<ProjectileProximityBeamController>();
            bfg1.blastRadius = bigBallRange;
            bfg1.blastDamageCoefficient = bigBallDamage / 2f;
            bfg1.blastProcCoefficient = bigBallProcCoefficient;
            bfg2.attackRange = tendrilRange;
            bfg2.listClearInterval = 1f / tendrilFireRate;
            bfg2.damageCoefficient = tendrilDamage / 2f;
            bfg2.procCoefficient = tendrilProcCoefficient;
        }
    }
}