using System;

// UNFINISHED AND UNUSED

namespace WellRoundedBalance.Enemies.Bosses.Vagrant {
    public class OrbSpread : BaseState {
        public static float BaseDuration = 2f;
        public static int BaseOrbs = 3;
        public static float BaseDamageCoefficient = 3f;
        public static float BaseOrbSpread = 90f;

        public override void OnEnter()
        {
            base.OnEnter();

            for (int i = 0; i < BaseOrbs; i++) {
                Ray aimRay = GetAimRay();
                float bonusYaw = Mathf.Floor((i - (BaseOrbs - 1) / 2f) / (BaseOrbs - 1)) * BaseOrbSpread;
                Vector3 forward = Util.ApplySpread(aimRay.direction, 0f, 0f, 1f, 1f, bonusYaw);

                FireProjectileInfo info = new();
                info.damage = base.damageStat * BaseDamageCoefficient;
                info.crit = base.RollCrit();
                info.position = base.transform.position;
                info.owner = base.gameObject;
                info.rotation = Util.QuaternionSafeLookRotation(forward);
                info.projectilePrefab = Utils.Paths.GameObject.VagrantCannon.Load<GameObject>();
                
                if (base.isAuthority) {
                    ProjectileManager.instance.FireProjectile(info);
                }

                AkSoundEngine.PostEvent(Events.Play_vagrant_attack1_shoot, base.gameObject);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.fixedAge >= BaseDuration / base.attackSpeedStat) {
                outer.SetNextStateToMain();
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}