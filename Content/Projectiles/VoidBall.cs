namespace WellRoundedBalance.Projectiles
{
    public static class VoidBall
    {
        public static GameObject prefab;

        public static void Create()
        {
            prefab = PrefabAPI.InstantiateClone(Utils.Paths.GameObject.NullifierPreBombProjectile.Load<GameObject>(), "VoidtouchedEliteSingleProjectile");
            var projectileImpactExplosion = prefab.GetComponent<ProjectileImpactExplosion>();
            var projectileController = prefab.GetComponent<ProjectileController>();

            projectileImpactExplosion.blastRadius = 9f;
            projectileImpactExplosion.blastProcCoefficient = 0f;
            projectileImpactExplosion.lifetime = 2f;
            projectileImpactExplosion.lifetimeExpiredSound = Utils.Paths.NetworkSoundEventDef.nseLunarSecondaryProjectileBounce.Load<NetworkSoundEventDef>();

            var teamAreaIndicator = prefab.transform.GetChild(0);

            teamAreaIndicator.transform.localScale = new Vector3(9f, 9f, 9f);

            var objectScaleCurve = teamAreaIndicator.GetComponent<ObjectScaleCurve>();

            objectScaleCurve.timeMax = 0.2f;

            var ghost = PrefabAPI.InstantiateClone(Utils.Paths.GameObject.NullifierPreBombGhost.Load<GameObject>(), "VoidtouchedEliteSingleProjectileGhost", false);
            ghost.transform.localScale = new Vector3(9f, 9f, 9f);

            var line = ghost.transform.GetChild(3);
            line.gameObject.SetActive(false);

            var sphere = ghost.transform.GetChild(2);
            var objectScaleCurve2 = sphere.GetComponent<ObjectScaleCurve>();
            objectScaleCurve2.timeMax = 2;

            projectileController.ghostPrefab = ghost;

            PrefabAPI.RegisterNetworkPrefab(prefab);
        }
    }
}