using RoR2.Skills;

namespace WellRoundedBalance.Enemies.Bosses
{
    internal class WanderingVagrant : EnemyBase<WanderingVagrant>
    {
        public static GameObject explosionVFX;
        public override string Name => "::: Bosses :: Wandering Vagrant";

        public override void Init()
        {
            base.Init();
            explosionVFX = Utils.Paths.GameObject.VagrantTrackingBombExplosion.Load<GameObject>();
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
            if (!Main.IsInfernoDef() && self.gameObject.name == "VagrantTrackingBomb(Clone)" && !self.projectileHealthComponent.alive)
            {
                EffectManager.SimpleEffect(explosionVFX, self.transform.position, Quaternion.identity, true);
                Object.Destroy(self.gameObject);
                return;
            }
            orig(self);
        }

        private void JellyBarrage_OnEnter(On.EntityStates.VagrantMonster.Weapon.JellyBarrage.orig_OnEnter orig, EntityStates.VagrantMonster.Weapon.JellyBarrage self)
        {
            if (!Main.IsInfernoDef())
            {
                EntityStates.VagrantMonster.Weapon.JellyBarrage.maxSpread = 35f;
                EntityStates.VagrantMonster.Weapon.JellyBarrage.baseDuration = 5f;
                EntityStates.VagrantMonster.Weapon.JellyBarrage.missileSpawnFrequency = 6f;
                EntityStates.VagrantMonster.Weapon.JellyBarrage.damageCoefficient = 3.3f;
            }

            orig(self);
        }

        private void ChargeMegaNova_OnEnter(On.EntityStates.VagrantMonster.ChargeMegaNova.orig_OnEnter orig, EntityStates.VagrantMonster.ChargeMegaNova self)
        {
            self.duration = Mathf.Max(3.5f, EntityStates.VagrantMonster.ChargeMegaNova.baseDuration / self.attackSpeedStat);
            var childLocator = self.GetComponent<ChildLocator>();
            if (childLocator)
            {
                var nova = childLocator.FindChild("NovaCenter");
                if (nova && EntityStates.VagrantMonster.ChargeMegaNova.areaIndicatorPrefab)
                {
                    self.areaIndicatorInstance.GetComponent<ObjectScaleCurve>().timeMax = self.duration;
                }
            }
            // make nova enrage consistent kirn
            orig(self);
        }

        private void Changes()
        {
            var fastProj = Utils.Paths.GameObject.VagrantCannon.Load<GameObject>();
            var projectileSimple = fastProj.GetComponent<ProjectileSimple>();
            projectileSimple.desiredForwardSpeed = 30f;

            var projectileImpactExplosion = fastProj.GetComponent<ProjectileImpactExplosion>();
            projectileImpactExplosion.falloffModel = BlastAttack.FalloffModel.None;
            projectileImpactExplosion.blastRadius = 6f;

            var slowProj = Utils.Paths.GameObject.VagrantTrackingBomb.Load<GameObject>();
            var projectileSimple2 = slowProj.GetComponent<ProjectileSimple>();
            projectileSimple2.desiredForwardSpeed = 15f;
            var cb = slowProj.GetComponent<CharacterBody>();
            cb.baseMaxHealth = 90f;
            cb.levelMaxHealth = 27f;

            var projectileImpactExplosion2 = slowProj.GetComponent<ProjectileImpactExplosion>();
            projectileImpactExplosion2.falloffModel = BlastAttack.FalloffModel.None;

            var vagrantExplosion = Utils.Paths.SkillDef.VagrantBodyChargeMegaNova.Load<SkillDef>();
            vagrantExplosion.baseRechargeInterval = 25f;

            var vagrantBarrage = Utils.Paths.SkillDef.VagrantBodyJellyBarrage.Load<SkillDef>();
            vagrantBarrage.baseRechargeInterval = 11f;

            var vagrantBomb = Utils.Paths.SkillDef.VagrantBodyTrackingBomb.Load<SkillDef>();
            vagrantBomb.baseRechargeInterval = 11f;
        }
    }
}