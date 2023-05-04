using RoR2.Skills;

namespace WellRoundedBalance.Enemies.Bosses
{
    internal class BeetleQueen : EnemyBase<BeetleQueen>
    {
        public override string Name => "::: Bosses :: Beetle Queen";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.EntityStates.BeetleQueenMonster.SummonEggs.OnEnter += SummonEggs_OnEnter;
            On.EntityStates.BeetleQueenMonster.FireSpit.OnEnter += FireSpit_OnEnter;
            On.EntityStates.BeetleQueenMonster.SpawnWards.OnEnter += SpawnWards_OnEnter;
            Changes();
        }

        private void SpawnWards_OnEnter(On.EntityStates.BeetleQueenMonster.SpawnWards.orig_OnEnter orig, EntityStates.BeetleQueenMonster.SpawnWards self)
        {
            if (!Main.IsInfernoDef())
                EntityStates.BeetleQueenMonster.SpawnWards.baseDuration = 3f;

            orig(self);
        }

        private void FireSpit_OnEnter(On.EntityStates.BeetleQueenMonster.FireSpit.orig_OnEnter orig, EntityStates.BeetleQueenMonster.FireSpit self)
        {
            if (!Main.IsInfernoDef())
            {
                EntityStates.BeetleQueenMonster.FireSpit.damageCoefficient = 0.3f;
                EntityStates.BeetleQueenMonster.FireSpit.force = 1200f;
                EntityStates.BeetleQueenMonster.FireSpit.yawSpread = 0f;
                EntityStates.BeetleQueenMonster.FireSpit.minSpread = 8f;
                EntityStates.BeetleQueenMonster.FireSpit.maxSpread = 13f;
                EntityStates.BeetleQueenMonster.FireSpit.projectileHSpeed = 40f;
            }
            orig(self);
        }

        private void SummonEggs_OnEnter(On.EntityStates.BeetleQueenMonster.SummonEggs.orig_OnEnter orig, EntityStates.BeetleQueenMonster.SummonEggs self)
        {
            if (!Main.IsInfernoDef())
            {
                EntityStates.BeetleQueenMonster.SummonEggs.summonInterval = 2f;
                EntityStates.BeetleQueenMonster.SummonEggs.randomRadius = 13f;
                EntityStates.BeetleQueenMonster.SummonEggs.baseDuration = 3f;
            }

            orig(self);
        }

        private void Changes()
        {
            var summonBeetleGuards = Utils.Paths.SkillDef.BeetleQueen2BodySummonEggs.Load<SkillDef>();
            summonBeetleGuards.baseRechargeInterval = 60f;

            var spitProjectile = Utils.Paths.GameObject.BeetleQueenSpit.Load<GameObject>();
            var projectileImpactExplosion = spitProjectile.GetComponent<ProjectileImpactExplosion>();
            projectileImpactExplosion.falloffModel = BlastAttack.FalloffModel.None;

            var spitDoT = Utils.Paths.GameObject.BeetleQueenAcid.Load<GameObject>();
            var projectileDotZone = spitDoT.GetComponent<ProjectileDotZone>();
            projectileDotZone.lifetime = 10f;
            projectileDotZone.damageCoefficient = 1.45f;
            spitDoT.transform.localScale = new Vector3(1.8f, 1.8f, 1.8f);

            var beetleWard = Utils.Paths.GameObject.BeetleWard.Load<GameObject>();
            var buffWard = beetleWard.GetComponent<BuffWard>();
            buffWard.radius = 9f;
            buffWard.interval = 0.5f;
            buffWard.buffDuration = 3f;

            var egg = Utils.Paths.SkillDef.BeetleQueen2BodySpawnWards.Load<SkillDef>();
            egg.baseRechargeInterval = 12f;
        }
    }
}