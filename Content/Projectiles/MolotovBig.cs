namespace WellRoundedBalance.Projectiles
{
    public static class MolotovBig
    {
        public static GameObject singlePrefab;

        public static void Create()
        {
            var ghostPrefab = BlazingProjectileVFX.prefab;

            var molotovChild = PrefabAPI.InstantiateClone(Utils.Paths.GameObject.MolotovSingleProjectile.Load<GameObject>(), "BlazingEliteSingleBigProjectile");
            var projectileControllerChild = molotovChild.GetComponent<ProjectileController>();
            var projectileImpactExplosionChild = molotovChild.GetComponent<ProjectileImpactExplosion>();
            var projectileSimpleChild = molotovChild.GetComponent<ProjectileSimple>();

            projectileControllerChild.ghostPrefab = ghostPrefab;
            projectileControllerChild.startSound = "Play_fireballsOnHit_shoot";

            projectileSimpleChild.lifetime = 35f;
            projectileSimpleChild.desiredForwardSpeed = 25f;

            projectileImpactExplosionChild.blastDamageCoefficient = 0f;
            projectileImpactExplosionChild.blastProcCoefficient = 1f;
            projectileImpactExplosionChild.dotIndex = DotController.DotIndex.None;
            projectileImpactExplosionChild.destroyOnEnemy = false;

            projectileImpactExplosionChild.impactEffect.GetComponent<EffectComponent>().soundName = "Play_fireballsOnHit_impact";

            var firePool = PrefabAPI.InstantiateClone(Utils.Paths.GameObject.MolotovProjectileDotZone.Load<GameObject>(), "BlazingElitePoolBigProjectile");
            var projectileDotZonePool = firePool.GetComponent<ProjectileDotZone>();
            var projectileControllerPool = firePool.GetComponent<ProjectileController>();
            projectileControllerPool.startSound = "Play_fireballsOnHit_impact";

            firePool.transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);

            projectileDotZonePool.damageCoefficient = 1f;
            projectileDotZonePool.overlapProcCoefficient = 1f;
            projectileDotZonePool.lifetime = 8f;
            projectileDotZonePool.fireFrequency = 12f;
            projectileDotZonePool.resetFrequency = 5f;

            projectileImpactExplosionChild.childrenProjectilePrefab = firePool;

            var hitboxGroup = firePool.GetComponent<HitBoxGroup>();

            var hitbox = firePool.transform.GetChild(0).GetChild(2);
            hitbox.localScale = new Vector3(1f, 0.1f, 1.5f);

            var hitbox2 = Object.Instantiate(hitbox, firePool.transform.GetChild(0));
            hitbox2.localEulerAngles = new Vector3(0f, 15f, 0f);

            var hitbox3 = Object.Instantiate(hitbox, firePool.transform.GetChild(0));
            hitbox3.localEulerAngles = new Vector3(0f, 30f, 0f);

            var hitbox4 = Object.Instantiate(hitbox, firePool.transform.GetChild(0));
            hitbox4.localEulerAngles = new Vector3(0f, 45f, 0f);

            var hitbox5 = Object.Instantiate(hitbox, firePool.transform.GetChild(0));
            hitbox5.localEulerAngles = new Vector3(0f, 60f, 0f);

            var hitbox6 = Object.Instantiate(hitbox, firePool.transform.GetChild(0));
            hitbox6.localEulerAngles = new Vector3(0f, 75f, 0f);

            var hitbox7 = Object.Instantiate(hitbox, firePool.transform.GetChild(0));
            hitbox7.localEulerAngles = new Vector3(0f, 90f, 0f);

            var hitbox8 = Object.Instantiate(hitbox, firePool.transform.GetChild(0));
            hitbox8.localEulerAngles = new Vector3(0f, 105f, 0f);

            hitboxGroup.hitBoxes = new HitBox[] { hitbox.GetComponent<HitBox>(), hitbox2.GetComponent<HitBox>(), hitbox3.GetComponent<HitBox>(), hitbox4.GetComponent<HitBox>(), hitbox5.GetComponent<HitBox>(), hitbox6.GetComponent<HitBox>(), hitbox7.GetComponent<HitBox>(), hitbox8.GetComponent<HitBox>() };

            PrefabAPI.RegisterNetworkPrefab(firePool);
            PrefabAPI.RegisterNetworkPrefab(molotovChild);

            singlePrefab = molotovChild;
        }
    }
}