using UnityEngine;
using RoR2.Projectile;
using Rewired.ComponentControls.Effects;

namespace UltimateCustomRun.Enemies.Bosses
{
    public static class MithrixPhase5
    {
        public static void Buff()
        {
            On.EntityStates.BrotherHaunt.FireRandomProjectiles.OnEnter += (orig, self) =>
            {
                EntityStates.BrotherHaunt.FireRandomProjectiles.maximumCharges = 100;
                EntityStates.BrotherHaunt.FireRandomProjectiles.chargeRechargeDuration = 0.04f;
                EntityStates.BrotherHaunt.FireRandomProjectiles.chanceToFirePerSecond = 0.35f;
                orig(self);
            };
            var cool = Resources.Load<GameObject>("prefabs/projectiles/BrotherUltLineProjectileStatic");
            cool.GetComponent<ProjectileSimple>().desiredForwardSpeed = 15f;
            cool.GetComponent<RotateAroundAxis>().enabled = true;
            cool.GetComponent<RotateAroundAxis>().slowRotationSpeed = 9f;
        }
    }
}
