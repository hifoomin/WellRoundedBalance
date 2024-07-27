using System;

namespace WellRoundedBalance.Enemies.Bosses.Vagrant {
    public class OrbSpread : BaseState {
        public static float BaseDuration = 2f;
        public static int BaseOrbs = 6;
        public static float BaseDamageCoefficient = 1f;
        public static float BaseOrbSpread = 4.5f;

        public override void OnEnter()
        {
            base.OnEnter();

            for (int i = 0; i < BaseOrbs; i++) {
                Ray aimRay = GetAimRay();
                Vector3 forward = Util.ApplySpread(aimRay.direction, -BaseOrbSpread, BaseOrbSpread, 1f, 1f);

                FireProjectileInfo info = new();
                info.damage = base.damageStat * BaseDamageCoefficient;
                info.crit = base.RollCrit();
                info.position = base.transform.position + UnityEngine.Random.onUnitSphere * 10f;
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
            base.characterBody.SetAimTimer(0.6f);
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