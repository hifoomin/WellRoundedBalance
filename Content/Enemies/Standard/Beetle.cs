﻿namespace WellRoundedBalance.Enemies.Standard
{
    internal class Beetle : EnemyBase<Beetle>
    {
        public override string Name => ":: Enemies ::::: Beetle";

        [ConfigField("Should Lunge?", "Disabled if playing Inferno.", true)]
        public static bool shouldLunge;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            if (shouldLunge)
            {
                On.EntityStates.BeetleMonster.HeadbuttState.FixedUpdate += HeadbuttState_FixedUpdate;
                CharacterMaster.onStartGlobal += CharacterMaster_onStartGlobal;
            }
        }

        private void CharacterMaster_onStartGlobal(CharacterMaster master)
        {
            if (Main.IsInfernoDef())
            {
                // pass
            }
            else
            {
                switch (master.name)
                {
                    case "BeetleMaster(Clone)":
                        AISkillDriver BeetleHeadbutt = (from x in master.GetComponents<AISkillDriver>()
                                                        where x.customName == "HeadbuttOffNodegraph"
                                                        select x).First();
                        BeetleHeadbutt.maxDistance = 8f;
                        BeetleHeadbutt.selectionRequiresOnGround = true;
                        BeetleHeadbutt.activationRequiresAimTargetLoS = true;
                        break;
                }
            }
        }

        private void HeadbuttState_FixedUpdate(On.EntityStates.BeetleMonster.HeadbuttState.orig_FixedUpdate orig, EntityStates.BeetleMonster.HeadbuttState self)
        {
            if (Main.IsInfernoDef())
            {
                // pass
            }
            else
            {
                EntityStates.BeetleMonster.HeadbuttState.baseDuration = 2.5f;
                if (self.isAuthority)
                {
                    if (self.modelAnimator && self.modelAnimator.GetFloat("Headbutt.hitBoxActive") > 0.5f)
                    {
                        var direction = self.GetAimRay().direction;
                        var ahead = direction.normalized * 2f * self.moveSpeedStat;
                        var up = Vector3.up * 5f;
                        var normalized = new Vector3(direction.x, 0f, direction.z).normalized;
                        self.characterMotor.Motor.ForceUnground();
                        self.characterMotor.velocity = ahead + up + normalized;
                    }
                }
            }

            orig(self);
        }
    }
}