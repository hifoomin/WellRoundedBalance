using System;
using System.Collections;

namespace WellRoundedBalance.Enemies.Bosses.Vagrant {
    public class ChainDashes : BaseSkillState {
        public int TotalDashes;
        public int CurDashes = 1;
        public float DashDelay = 2f;
        public float DashDamage = 4f;
        public float DashForce = 90f;
        public OverlapAttack attack;
        private float stopwatch = 0f;

        public override void OnEnter()
        {
            base.OnEnter();
            base.characterBody.SetAimTimer(0.2f);
            DashDelay /= base.attackSpeedStat;
            TotalDashes = base.healthComponent.isHealthLow ? 5 : 3;
        }

        public override void FixedUpdate()
        {
            base.fixedAge -= Time.fixedDeltaTime;

            if (!base.isAuthority) {
                return;
            }

            if (CurDashes > TotalDashes && base.fixedAge <= 0f) {
                outer.SetNextStateToMain();
            }

            if (base.fixedAge <= 0f) {
                CurDashes++;
                base.fixedAge = DashDelay;
                base.characterBody.SetAimTimer(0.1f);
                base.rigidbodyDirection.aimDirection = base.inputBank.aimDirection;
                base.rigidbodyMotor.ApplyForceImpulse(new PhysForceInfo() {
                    force = base.inputBank.aimDirection * DashForce,
                    disableAirControlUntilCollision = false,
                    ignoreGroundStick = true,
                    massIsOne = true
                });
                for (int i = 0; i < 5; i++) { AkSoundEngine.PostEvent(Events.Play_vagrant_attack1_shoot, base.gameObject); }

                int count = UnityEngine.Random.Range(7, 14);

                for (int i = 0; i < count; i++) {
                    FireProjectileInfo info = new();
                    info.owner = base.gameObject;
                    info.position = base.transform.position;
                    info.crit = base.RollCrit();
                    info.rotation = Quaternion.LookRotation(UnityEngine.Random.onUnitSphere);
                    info.damage = base.damageStat * 1.5f;
                    info.projectilePrefab = WanderingVagrant.VagrantSeekerOrb;

                    ProjectileManager.instance.FireProjectile(info);
                }
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Death;
        }
    }
}