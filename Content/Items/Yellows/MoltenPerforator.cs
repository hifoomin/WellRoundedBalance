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

        public override string DescText => "<style=cIsDamage>10%</style> chance on hit to call forth <style=cIsDamage>3 magma balls</style> from an enemy, dealing <style=cIsDamage>190%</style> <style=cStack>(+60% per stack)</style> TOTAL damage.";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnHitEnemy += Changes;
            ChangeProcCoefficient();
            Changes();
        }

        private void Changes(ILContext il)
        {
            ILCursor c = new(il);

            float initialDamage = 1.9f - 0.6f;

            bool error = true;

            if (c.TryGotoNext(x => x.MatchLdsfld(typeof(RoR2Content.Items), "FireballsOnHit")) &&
            c.TryGotoNext(x => x.MatchLdfld<DamageInfo>("damage")))
            {
                c.Index -= 6;
                c.Next.Operand = 0.6f;
                c.Index += 4;
                c.EmitDelegate<Func<float, float>>((damageCoefficient) =>
                {
                    return damageCoefficient + initialDamage;
                });
                error = false;
            }
            if (error)
            {
                Main.WRBLogger.LogError("Failed to apply Molten Perforator hook");
            }
        }

        private void ChangeProcCoefficient()
        {
            var m = LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/firemeatball").GetComponent<ProjectileController>();
            m.procCoefficient = 0f;
        }

        private void Changes()
        {
            LanguageAPI.Add("ITEM_fireballsOnHit_NAME".ToUpper(), "Molten Peripherator");
        }
    }
}