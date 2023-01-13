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

            var teamAreaIndicator = prefab.transform.GetChild(0);

            teamAreaIndicator.transform.localScale = new Vector3(9f, 9f, 9f);

            var objectScaleCurve = teamAreaIndicator.GetComponent<ObjectScaleCurve>();

            objectScaleCurve.timeMax = 0.2f;

            // var constantForce = prefab.AddComponent<ConstantForce>();
            // constantForce.force = new Vector3(0f, -800f, 0f);

            // WHY DOES IT IGNORE COLLISION????????

            var rigidBody = prefab.GetComponent<Rigidbody>();
            rigidBody.collisionDetectionMode = CollisionDetectionMode.Continuous;
            rigidBody.useGravity = true;

            var projectileStickOnImpact = prefab.AddComponent<ProjectileStickOnImpact>();
            projectileStickOnImpact.ignoreCharacters = true;
            projectileStickOnImpact.ignoreWorld = false;
            projectileStickOnImpact.alignNormals = false;

            var ghost = PrefabAPI.InstantiateClone(Utils.Paths.GameObject.NullifierPreBombGhost.Load<GameObject>(), "VoidtouchedEliteSingleProjectileGhost", false);
            ghost.transform.localScale = new Vector3(5f, 5f, 5f);

            projectileController.ghostPrefab = ghost;

            PrefabAPI.RegisterNetworkPrefab(prefab);
        }
    }
}