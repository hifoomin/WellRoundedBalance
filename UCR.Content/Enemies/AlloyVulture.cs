using RoR2.Projectile;
using UnityEngine;

namespace UltimateCustomRun.Enemies
{
    public class AlloyVulture : EnemyBase
    {
        public static float ProjectileSpeed;
        public override string Name => ":::: Enemies :: Alloy Vulture";

        public override void Init()
        {
            ProjectileSpeed = ConfigOption(40f, "Windblade Projectile Speed", "Vanilla is 40.\nRecommended Value: 55");
            base.Init();
        }

        public override void Hooks()
        {
            Buff();
        }

        public void Buff()
        {
            var WindbladeProjectile = Resources.Load<GameObject>("prefabs/projectiles/WindbladeProjectile").GetComponent<ProjectileSimple>();
            WindbladeProjectile.desiredForwardSpeed = ProjectileSpeed;
        }
    }
}