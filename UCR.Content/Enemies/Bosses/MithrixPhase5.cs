using UnityEngine;
using RoR2.Projectile;
using Rewired.ComponentControls.Effects;

namespace UltimateCustomRun.Enemies.Bosses
{
    public class MithrixPhase5 : EnemyBase
    {
        public static bool walltw;
        public static bool rottw;
        public static bool movetw;
        public override string Name => ":::: Enemies :::: Mithrix Escape Sequence";

        public override void Init()
        {
            walltw = ConfigOption(false, "Make Walls spammier?", "Vanilla is false. Recommended Value: True");
            rottw = ConfigOption(false, "Make Walls rotate?", "Vanilla is false. Recommended Value: True");
            movetw = ConfigOption(false, "Make Walls move?", "Vanilla is false. Recommended Value: True");
            base.Init();
        }

        public override void Hooks()
        {
            Buff();
        }
        public static void Buff()
        {
            var cool = Resources.Load<GameObject>("prefabs/projectiles/BrotherUltLineProjectileStatic");
            if (walltw)
            {
                On.EntityStates.BrotherHaunt.FireRandomProjectiles.OnEnter += (orig, self) =>
                {
                    EntityStates.BrotherHaunt.FireRandomProjectiles.maximumCharges = 100;
                    EntityStates.BrotherHaunt.FireRandomProjectiles.chargeRechargeDuration = 0.04f;
                    EntityStates.BrotherHaunt.FireRandomProjectiles.chanceToFirePerSecond = 0.35f;
                    orig(self);
                };
            }

            if (rottw || movetw)
            {
                cool.GetComponent<RotateAroundAxis>().enabled = true;
            }

            if (rottw)
            {

                cool.GetComponent<RotateAroundAxis>().slowRotationSpeed = 15f;
            }
            if (movetw)
            {
                cool.GetComponent<ProjectileSimple>().desiredForwardSpeed = 20f;
            }

        }
    }
}
