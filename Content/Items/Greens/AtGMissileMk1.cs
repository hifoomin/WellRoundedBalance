using Mono.Cecil.Cil;
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
        public override string DescText => "<style=cIsDamage>10%</style> chance to fire a missile that deals <style=cIsDamage>300%</style> <style=cStack>(+200% per stack)</style> TOTAL damage.";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnHitEnemy += GlobalEventManager_OnHitEnemy;
            ChangeProc();
        }

        private void GlobalEventManager_OnHitEnemy(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchBrfalse(out _),
                x => x.MatchLdcR4(3f),
                x => x.MatchLdloc(out _),
                x => x.MatchConvR4(),
                x => x.MatchMul()))
            {
                c.Index += 1;
                c.Next.Operand = 2f;
                c.Index += 4;
                c.EmitDelegate<Func<float, float>>((self) =>
                {
                    return self + 1f;
                });
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply AtG Damage hook");
            }
        }

        public static void ChangeProc()
        {
            var mp = LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/MissileProjectile").GetComponent<ProjectileController>();
            mp.procCoefficient = 0f;
        }
    }
}