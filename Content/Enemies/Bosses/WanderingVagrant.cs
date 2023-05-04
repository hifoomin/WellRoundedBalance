namespace WellRoundedBalance.Enemies.Bosses
{
    internal class WanderingVagrant : EnemyBase<WanderingVagrant>
    {
        public override string Name => "::: Bosses :: Wandering Vagrant";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.EntityStates.VagrantMonster.ChargeMegaNova.OnEnter += ChargeMegaNova_OnEnter;
            On.EntityStates.VagrantMonster.Weapon.JellyBarrage.OnEnter += JellyBarrage_OnEnter;
            On.RoR2.Projectile.ProjectileImpactExplosion.FixedUpdate += ProjectileImpactExplosion_FixedUpdate;
            Changes();
        }

        private void ProjectileImpactExplosion_FixedUpdate(On.RoR2.Projectile.ProjectileImpactExplosion.orig_FixedUpdate orig, ProjectileImpactExplosion self)
        {
            if (self.gameObject.name == "VagrantTrackingBomb(Clone)" && !self.projectileHealthComponent.alive)
                return;
            orig(self);
        }

        private void JellyBarrage_OnEnter(On.EntityStates.VagrantMonster.Weapon.JellyBarrage.orig_OnEnter orig, EntityStates.VagrantMonster.Weapon.JellyBarrage self)
        {
            EntityStates.VagrantMonster.Weapon.JellyBarrage.maxSpread = 20f;
            EntityStates.VagrantMonster.Weapon.JellyBarrage.baseDuration = 5f;
            EntityStates.VagrantMonster.Weapon.JellyBarrage.missileSpawnFrequency = 5f;
            EntityStates.VagrantMonster.Weapon.JellyBarrage.damageCoefficient = 3.5f;
            orig(self);
        }

        private void ChargeMegaNova_OnEnter(On.EntityStates.VagrantMonster.ChargeMegaNova.orig_OnEnter orig, EntityStates.VagrantMonster.ChargeMegaNova self)
        {
            self.duration = Mathf.Max(3.5f, EntityStates.VagrantMonster.ChargeMegaNova.baseDuration / self.attackSpeedStat);
            // make nova enrage consistent kirn
            orig(self);
        }

        private void Changes()
        {
            var fastProj = Utils.Paths.GameObject.VagrantCannon.Load<GameObject>();
            var projectileSimple = fastProj.GetComponent<ProjectileSimple>();
            projectileSimple.desiredForwardSpeed = 60f;

            var projectileImpactExplosion = fastProj.GetComponent<ProjectileImpactExplosion>();
            projectileImpactExplosion.falloffModel = BlastAttack.FalloffModel.None;
            projectileImpactExplosion.blastRadius = 7f;

            var slowProj = Utils.Paths.GameObject.VagrantTrackingBomb.Load<GameObject>();
            var projectileSimple2 = slowProj.GetComponent<ProjectileSimple>();
            projectileSimple2.desiredForwardSpeed = 12f;
        }
    }
}