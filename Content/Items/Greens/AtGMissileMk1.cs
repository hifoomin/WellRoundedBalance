using MonoMod.Cil;
using RoR2;
using RoR2.Projectile;
using System;
using UnityEngine;

namespace WellRoundedBalance.Items.Greens
{
    public class AtGMissileMk1 : ItemBase
    {
        public override string Name => ":: Items :: Greens :: AtG Missile Mk1";
        public override string InternalPickupToken => "missile";

        public override string PickupText => "Chance to fire a missile.";
        public override string DescText => "<style=cIsDamage>10%</style> chance to fire a missile that deals <style=cIsDamage>300%</style> <style=cStack>(+300% per stack)</style> TOTAL damage.";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            ChangeProc();
        }

        public static void ChangeProc()
        {
            var mp = LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/MissileProjectile").GetComponent<ProjectileController>();
            mp.procCoefficient = 0.2f;
        }
    }
}