namespace WellRoundedBalance.Enemies.Standard
{
    internal class AlloyVulture : EnemyBase<AlloyVulture>
    {
        public override string Name => ":: Enemies :: Alloy Vulture";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.EntityStates.Vulture.Weapon.FireWindblade.OnEnter += FireWindblade_OnEnter;
            Changes();
        }

        private void Changes()
        {
            var wind = Utils.Paths.GameObject.WindbladeProjectile.Load<GameObject>();
            var projectileSimple = wind.GetComponent<ProjectileSimple>();
            projectileSimple.lifetime = 10f;
            projectileSimple.desiredForwardSpeed = 50f;
            wind.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);

            var projectileController = wind.GetComponent<ProjectileController>();
            projectileController.ghostPrefab.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);

            var projectileSingleTargetImpact = wind.GetComponent<ProjectileSingleTargetImpact>();
            projectileSingleTargetImpact.destroyOnWorld = false;

            var boxCollider = wind.GetComponent<BoxCollider>();
            boxCollider.material = Utils.Paths.PhysicMaterial.physmatSuperBouncy.Load<PhysicMaterial>();
        }

        private void FireWindblade_OnEnter(On.EntityStates.Vulture.Weapon.FireWindblade.orig_OnEnter orig, EntityStates.Vulture.Weapon.FireWindblade self)
        {
            if (!Main.IsInfernoDef())
                EntityStates.Vulture.Weapon.FireWindblade.damageCoefficient = 2f;
            orig(self);
        }
    }
}