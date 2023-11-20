using System;
using System.Collections;

namespace WellRoundedBalance.Enemies.Bosses.Vagrant {
    public class ChainDashes : BaseSkillState {
        public int TotalDashes;
        public int CurDashes = 1;
        public float DashDelay = 0.9f;
        public float DashDamage = 4f;
        public float DashForce = 60f;
        public OverlapAttack attack;

        public override void OnEnter()
        {
            base.OnEnter();
            base.characterBody.SetAimTimer(0.2f);
            DashDelay /= base.attackSpeedStat;
            TotalDashes = base.healthComponent.isHealthLow ? 5 : 3;

            attack = new();
            attack.attacker = base.gameObject;
            attack.damage = base.damageStat * DashDamage;
            attack.hitBoxGroup = FindHitBoxGroup("VagrantChainDash");
            attack.isCrit = base.RollCrit();
            attack.teamIndex = base.GetTeam();
            attack.attackerFiltering = AttackerFiltering.NeverHitSelf;
        }

        public override void FixedUpdate()
        {
            base.fixedAge -= Time.fixedDeltaTime;

            attack.Fire();

            if (CurDashes > TotalDashes) {
                outer.SetNextStateToMain();
            }

            if (base.fixedAge <= 0f) {
                CurDashes++;
                base.fixedAge = DashDelay;
                base.characterBody.SetAimTimer(0.1f);
                base.rigidbodyMotor.ApplyForceImpulse(new PhysForceInfo() {
                    force = base.inputBank.aimDirection * DashForce,
                    disableAirControlUntilCollision = false,
                    ignoreGroundStick = true,
                    massIsOne = true
                });
                for (int i = 0; i < 5; i++) { AkSoundEngine.PostEvent(Events.Play_vagrant_attack1_shoot, base.gameObject); }
                attack.ResetIgnoredHealthComponents();
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Death;
        }
    }
}