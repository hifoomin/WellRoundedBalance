using System;

// UNFINISHED AND UNUSED

namespace WellRoundedBalance.Enemies.Bosses.Vagrant {
    public class ShockOrb : BaseState {
        public static float BaseDuration = 2f;
        public static float BaseDamageCoefficient = 9f;

        public override void OnEnter()
        {
            base.OnEnter();

            FireProjectileInfo info = new();
            info.damage = base.damageStat * BaseDamageCoefficient;
            info.crit = base.RollCrit();
            info.position = base.transform.position;
            info.owner = base.gameObject;
            info.rotation = Util.QuaternionSafeLookRotation(base.inputBank.aimDirection);
            info.projectilePrefab = Utils.Paths.GameObject.VagrantCannon.Load<GameObject>();
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