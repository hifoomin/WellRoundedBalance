namespace WellRoundedBalance.Projectiles
{
    public static class EarthQuakeWave
    {
        public static GameObject prefab;

        public static void Create()
        {
            prefab = PrefabAPI.InstantiateClone(Utils.Paths.GameObject.BrotherSunderWave.Load<GameObject>(), "EarthquakeWave", false);
            var projectileDamage = prefab.GetComponent<ProjectileDamage>();
            projectileDamage.damageType = DamageType.Generic;

            var newGhost = Utils.Paths.GameObject.BrotherSunderWaveGhost.Load<GameObject>();

            var projectileController = prefab.GetComponent<ProjectileController>();
            projectileController.flightSoundLoop = null;

            PrefabAPI.RegisterNetworkPrefab(prefab);
        }
    }
}