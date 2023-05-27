namespace WellRoundedBalance.Projectiles
{
    public static class Molotov
    {
        public static GameObject prefab;
        public static GameObject singlePrefab;

        public static void Create()
        {
            var ghostPrefab = BlazingProjectileVFX.prefab;
            prefab = PrefabAPI.InstantiateClone(Utils.Paths.GameObject.MolotovClusterProjectile.Load<GameObject>(), "BlazingEliteClusterProjectile");
            var projectileController = prefab.GetComponent<ProjectileController>();
            var projectileImpactExplosion = prefab.GetComponent<ProjectileImpactExplosion>();
            var projectileSimple = prefab.GetComponent<ProjectileSimple>();
            var applyTorqueOnStart = prefab.AddComponent<ApplyTorqueOnStart>();

            projectileSimple.lifetime = 35f;
            projectileSimple.desiredForwardSpeed = 60f;

            applyTorqueOnStart.localTorque = new Vector3(0f, 50f, 0f);
            applyTorqueOnStart.randomize = false;

            projectileController.ghostPrefab = ghostPrefab;
            projectileController.startSound = "Play_fireballsOnHit_shoot";
            //prefab = Utils.Paths.GameObject.MolotovProjectileDotZone.Load<GameObject>();

            projectileImpactExplosion.blastDamageCoefficient = 0f;
            projectileImpactExplosion.blastProcCoefficient = 0f;
            projectileImpactExplosion.dotIndex = DotController.DotIndex.None;
            projectileImpactExplosion.childrenCount = 3;
            projectileImpactExplosion.destroyOnEnemy = false;

            var molotovChild = PrefabAPI.InstantiateClone(Utils.Paths.GameObject.MolotovSingleProjectile.Load<GameObject>(), "BlazingEliteSingleProjectile");
            var projectileControllerChild = molotovChild.GetComponent<ProjectileController>();
            var projectileImpactExplosionChild = molotovChild.GetComponent<ProjectileImpactExplosion>();
            var projectileSimpleChild = molotovChild.GetComponent<ProjectileSimple>();
            // var applyTorqueOnStartChild = molotovChild.AddComponent<ApplyTorqueOnStart>();

            projectileControllerChild.ghostPrefab = ghostPrefab;
            projectileControllerChild.startSound = "Play_fireballsOnHit_shoot";

            projectileSimpleChild.lifetime = 35f;
            projectileSimpleChild.desiredForwardSpeed = 25f;

            // applyTorqueOnStartChild.localTorque = new Vector3(0f, 50f, 0f);
            // applyTorqueOnStartChild.randomize = false;

            projectileImpactExplosionChild.blastDamageCoefficient = 0f;
            projectileImpactExplosionChild.blastProcCoefficient = 0f;
            projectileImpactExplosionChild.dotIndex = DotController.DotIndex.None;
            projectileImpactExplosionChild.destroyOnEnemy = false;

            projectileImpactExplosionChild.impactEffect.GetComponent<EffectComponent>().soundName = "Play_fireballsOnHit_impact";

            var firePool = PrefabAPI.InstantiateClone(Utils.Paths.GameObject.MolotovProjectileDotZone.Load<GameObject>(), "BlazingElitePoolProjectile");
            var projectileDotZonePool = firePool.GetComponent<ProjectileDotZone>();
            var projectileControllerPool = firePool.GetComponent<ProjectileController>();
            projectileControllerPool.startSound = "Play_fireballsOnHit_impact";

            firePool.transform.localScale = new Vector3(1.75f, 1.75f, 1.75f);

            projectileDotZonePool.damageCoefficient = 1f;
            projectileDotZonePool.overlapProcCoefficient = 0f;
            projectileDotZonePool.lifetime = 6f;
            projectileDotZonePool.fireFrequency = 12f;
            projectileDotZonePool.resetFrequency = 5f;

            projectileImpactExplosionChild.childrenProjectilePrefab = firePool;

            projectileImpactExplosion.childrenProjectilePrefab = molotovChild;

            var hitboxGroup = firePool.GetComponent<HitBoxGroup>();

            var hitbox = firePool.transform.GetChild(0).GetChild(2);
            hitbox.localScale = new Vector3(1.23375f, 0.48125f, 1.23375f);

            var hitbox2 = Object.Instantiate(hitbox, firePool.transform.GetChild(0));
            hitbox2.localEulerAngles = new Vector3(0f, 45f, 0f);

            hitboxGroup.hitBoxes = new HitBox[] { hitbox.GetComponent<HitBox>(), hitbox2.GetComponent<HitBox>() };

            PrefabAPI.RegisterNetworkPrefab(firePool);
            PrefabAPI.RegisterNetworkPrefab(molotovChild);
            PrefabAPI.RegisterNetworkPrefab(prefab);

            singlePrefab = molotovChild;
        }
    }
}