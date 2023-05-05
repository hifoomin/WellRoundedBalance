namespace WellRoundedBalance.Enemies.Bosses
{
    internal class StoneTitan : EnemyBase<StoneTitan>
    {
        public override string Name => "::: Bosses :: Stone Titan";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.EntityStates.TitanMonster.RechargeRocks.OnEnter += RechargeRocks_OnEnter;
            On.EntityStates.TitanMonster.FireFist.OnEnter += FireFist_OnEnter;
            On.EntityStates.TitanMonster.FireMegaLaser.OnEnter += FireMegaLaser_OnEnter;
            Changes();
        }

        private void FireMegaLaser_OnEnter(On.EntityStates.TitanMonster.FireMegaLaser.orig_OnEnter orig, EntityStates.TitanMonster.FireMegaLaser self)
        {
            EntityStates.TitanMonster.FireMegaLaser.damageCoefficient = 1.6f;
            orig(self);
        }

        private void FireFist_OnEnter(On.EntityStates.TitanMonster.FireFist.orig_OnEnter orig, EntityStates.TitanMonster.FireFist self)
        {
            if (!Main.IsInfernoDef())
            {
                EntityStates.TitanMonster.FireFist.entryDuration = 1.8f;
                EntityStates.TitanMonster.FireFist.fireDuration = 1f;
                EntityStates.TitanMonster.FireFist.fistDamageCoefficient = 1.6f;
                if (self.isAuthority)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        var fpi = new FireProjectileInfo
                        {
                            projectilePrefab = self.fistProjectilePrefab,
                            position = self.characterBody.footPosition + new Vector3(Run.instance.spawnRng.RangeFloat(-25f * i, 25f * i), 0f, Run.instance.spawnRng.RangeFloat(-25f * i, 25f * i)),
                            rotation = Quaternion.identity,
                            owner = self.gameObject,
                            damage = self.damageStat,
                            force = EntityStates.TitanMonster.FireFist.fistForce,
                            crit = self.RollCrit(),
                            fuseOverride = EntityStates.TitanMonster.FireFist.entryDuration - EntityStates.TitanMonster.FireFist.trackingDuration + (i / 2f)
                        };
                        ProjectileManager.instance.FireProjectile(fpi);
                    }
                }
            }

            orig(self);
        }

        private void RechargeRocks_OnEnter(On.EntityStates.TitanMonster.RechargeRocks.orig_OnEnter orig, EntityStates.TitanMonster.RechargeRocks self)
        {
            if (!Main.IsInfernoDef())
                EntityStates.TitanMonster.RechargeRocks.baseDuration = 3f;

            orig(self);
        }

        private void Changes()
        {
            var rockController = Utils.Paths.GameObject.TitanRockController.Load<GameObject>();
            var destroyOnTimer = rockController.GetComponent<DestroyOnTimer>();
            destroyOnTimer.duration = 20f;

            var tit = rockController.GetComponent<TitanRockController>();
            tit.startDelay = 2f;
            tit.fireInterval = 0.33f;
            tit.damageCoefficient = 0.48f;
            tit.damageForce = 2000f;

            var titanBody = Utils.Paths.GameObject.TitanBody13.Load<GameObject>().GetComponent<CharacterBody>();
            titanBody.baseDamage = 25f;
            titanBody.levelDamage = 5f;
        }
    }
}