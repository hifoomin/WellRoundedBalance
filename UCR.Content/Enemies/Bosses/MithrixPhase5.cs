using Rewired.ComponentControls.Effects;
using RoR2.Projectile;
using UnityEngine;

namespace UltimateCustomRun.Enemies.Bosses
{
    public class MithrixPhase5 : EnemyBase
    {
        public static bool WallTweaks;
        public static bool RotationTweaks;
        public static bool MoveTweaks;
        public override string Name => ":::: Enemies :::: Mithrix Escape Sequence";

        public override void Init()
        {
            WallTweaks = ConfigOption(false, "Make Walls spammier?", "Vanilla is false.\nRecommended Value: True");
            RotationTweaks = ConfigOption(false, "Make Walls rotate?", "Vanilla is false.\nRecommended Value: True");
            MoveTweaks = ConfigOption(false, "Make Walls move?", "Vanilla is false.\nRecommended Value: True");
            base.Init();
        }

        public override void Hooks()
        {
            Buff();
        }

        public static void Buff()
        {
            var cool = Resources.Load<GameObject>("prefabs/projectiles/BrotherUltLineProjectileStatic");
            if (WallTweaks)
            {
                On.EntityStates.BrotherHaunt.FireRandomProjectiles.OnEnter += (orig, self) =>
                {
                    EntityStates.BrotherHaunt.FireRandomProjectiles.maximumCharges = 100;
                    EntityStates.BrotherHaunt.FireRandomProjectiles.chargeRechargeDuration = 0.04f;
                    EntityStates.BrotherHaunt.FireRandomProjectiles.chanceToFirePerSecond = 0.35f;
                    orig(self);
                };
            }

            if (RotationTweaks || MoveTweaks)
            {
                cool.GetComponent<RotateAroundAxis>().enabled = true;
            }

            if (RotationTweaks)
            {
                cool.GetComponent<RotateAroundAxis>().slowRotationSpeed = 15f;
            }
            if (MoveTweaks)
            {
                cool.GetComponent<ProjectileSimple>().desiredForwardSpeed = 20f;
            }
        }
    }
}