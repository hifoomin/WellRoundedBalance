﻿namespace WellRoundedBalance.Enemies.Minibosses
{
    internal class StoneGolem : EnemyBase<StoneGolem>
    {
        public override string Name => ":: Minibosses :: Stone Golem";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.EntityStates.GolemMonster.ChargeLaser.OnEnter += ChargeLaser_OnEnter;
            On.EntityStates.GolemMonster.ClapState.OnEnter += ClapState_OnEnter;
            On.EntityStates.GolemMonster.FireLaser.OnEnter += FireLaser_OnEnter;
            Changes();
        }

        private void FireLaser_OnEnter(On.EntityStates.GolemMonster.FireLaser.orig_OnEnter orig, EntityStates.GolemMonster.FireLaser self)
        {
            if (!Main.IsInfernoDef())
            {
                EntityStates.GolemMonster.FireLaser.damageCoefficient = 3f;
                EntityStates.GolemMonster.FireLaser.blastRadius = 4.5f;
            }

            orig(self);
        }

        private void ClapState_OnEnter(On.EntityStates.GolemMonster.ClapState.orig_OnEnter orig, EntityStates.GolemMonster.ClapState self)
        {
            if (!Main.IsInfernoDef())
            {
                EntityStates.GolemMonster.ClapState.duration = 1.6f;
                EntityStates.GolemMonster.ClapState.damageCoefficient = 5f;
                EntityStates.GolemMonster.ClapState.radius = 6f;
                self.attack.falloffModel = BlastAttack.FalloffModel.None;
            }

            orig(self);
        }

        private void ChargeLaser_OnEnter(On.EntityStates.GolemMonster.ChargeLaser.orig_OnEnter orig, EntityStates.GolemMonster.ChargeLaser self)
        {
            if (!Main.IsInfernoDef())
                EntityStates.GolemMonster.ChargeLaser.baseDuration = 2.4f;
            orig(self);
        }

        private void Changes()
        {
            var golemBody = Utils.Paths.GameObject.GolemBody28.Load<GameObject>().GetComponent<CharacterBody>();
            golemBody.baseDamage = 14f;
            golemBody.levelDamage = 2.8f;
        }
    }
}