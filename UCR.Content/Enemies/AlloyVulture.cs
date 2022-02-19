using RoR2;
using UnityEngine;
using System.Linq;
using RoR2.CharacterAI;
using RoR2.Projectile;

namespace UltimateCustomRun.Enemies
{
    public class AlloyVulture : EnemyBase
    {
        public static float pspd;
        public override string Name => ":::: Enemies :: Alloy Vulture";

        public override void Init()
        {
            pspd = ConfigOption(40f, "Windblade Projectile Speed", "Vanilla is 40. Recommended Value: 55");
            base.Init();
        }

        public override void Hooks()
        {
            Buff();
        }

        public void Buff()
        {
            var w = Resources.Load<GameObject>("prefabs/projectiles/WindbladeProjectile").GetComponent<ProjectileSimple>();
            w.desiredForwardSpeed = pspd;
        }
    }
}
