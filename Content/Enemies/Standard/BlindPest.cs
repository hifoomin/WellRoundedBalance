namespace WellRoundedBalance.Enemies.Standard
{
    internal class BlindPest : EnemyBase<BlindPest>
    {
        public override string Name => ":: Enemies :: Blind Pest";

        [ConfigField("Base Damage", "Disabled if playing Inferno.", 10f)]
        public static float baseDamage;

        [ConfigField("Director Credit Cost", "", 35)]
        public static int directorCreditCost;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            Changes();
        }

        private void Changes()
        {
            var blindPest = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/FlyingVermin/FlyingVerminBody.prefab").WaitForCompletion();
            var blindPestBody = blindPest.GetComponent<CharacterBody>();
            blindPestBody.baseDamage = baseDamage;
            blindPestBody.levelDamage = baseDamage * 0.2f;

            var blindPestSC = Utils.Paths.CharacterSpawnCard.cscFlyingVermin.Load<CharacterSpawnCard>();
            blindPestSC.directorCreditCost = directorCreditCost;

            var blindPestSC2 = Utils.Paths.CharacterSpawnCard.cscFlyingVerminSnowy.Load<CharacterSpawnCard>();
            blindPestSC2.directorCreditCost = directorCreditCost;

            var spit = Utils.Paths.GameObject.VerminSpitProjectile.Load<GameObject>();
            var projectileSimple = spit.GetComponent<ProjectileSimple>();
            projectileSimple.desiredForwardSpeed = 30f;
            projectileSimple.lifetime = 10f;

            spit.transform.localScale = new Vector3(0.15f, 0.15f, 3f);

            var projectileController = spit.GetComponent<ProjectileController>();
            projectileController.ghostPrefab.transform.localScale = new Vector3(5f, 5f, 5f);

            spit.AddComponent<ProjectileTargetComponent>();
            var steer = spit.AddComponent<ProjectileSteerTowardTarget>();
            steer.enabled = true;
            steer.rotationSpeed = 25f;
            steer.yAxisOnly = false;

            var finder = spit.AddComponent<ProjectileDirectionalTargetFinder>();
            finder.enabled = true;
            finder.lookRange = 13f;
            finder.lookCone = 35f;
            finder.targetSearchInterval = 0.1f;
            finder.onlySearchIfNoTarget = true;
        }
    }
}