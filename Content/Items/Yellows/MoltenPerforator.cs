using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using RoR2.Projectile;
using System;
using UnityEngine;

namespace WellRoundedBalance.Items.Yellows
{
    public class MoltenPerforator : ItemBase
    {
        public override string Name => ":: Items :::: Yellows :: Molten Perforator";
        public override string InternalPickupToken => "fireballsOnHit";

        public override string PickupText => "Chance on hit to fire magma balls.";

        public override string DescText => "<style=cIsDamage>10%</style> chance on hit to call forth <style=cIsDamage>3 magma balls</style> from an enemy, dealing <style=cIsDamage>170%</style> <style=cStack>(+170% per stack)</style> TOTAL damage and <style=cIsDamage>igniting</style> all enemies.";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnHitEnemy += Changes;
            ChangeProcCoefficient();
        }

        private void Changes(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    //x => x.MatchStloc(117),
                    x => x.MatchLdcR4(3f),
                    x => x.MatchLdloc(17)))
            {
                c.Next.Operand = 1.7f;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Molten Perforator Damage hook");
            }
        }

        private void ChangeProcCoefficient()
        {
            var m = LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/firemeatball").GetComponent<ProjectileController>();
            m.procCoefficient = 0f;
        }
    }
}