namespace WellRoundedBalance.Projectiles
{
    public static class VoidBall
    {
        public static GameObject prefab;

        public static void Create()
        {
            prefab = PrefabAPI.InstantiateClone(Utils.Paths.GameObject.ImpVoidspikeProjectile.Load<GameObject>(), "VoidtouchedEliteSingleProjectile");

            prefab.transform.localScale = new Vector3(5f, 5f, 5f);

            var projectileDamage = prefab.GetComponent<ProjectileDamage>();
            projectileDamage.damageType = DamageType.Nullify;

            var projectileImpactExplosion = prefab.GetComponent<ProjectileImpactExplosion>();
            projectileImpactExplosion.blastRadius = 10f;

            PrefabAPI.RegisterNetworkPrefab(prefab);
        }
    }
}