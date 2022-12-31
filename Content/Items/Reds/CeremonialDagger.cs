using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using RoR2.Projectile;
using System;
using UnityEngine;

namespace WellRoundedBalance.Items.Reds
{
    public class CeremonialDagger : ItemBase
    {
        public override string Name => ":: Items ::: Reds :: Ceremonial Dagger";
        public override string InternalPickupToken => "dagger";

        public override string PickupText => "Killing an enemy releases homing daggers.";

        public override string DescText => "Killing an enemy fires out <style=cIsDamage>3</style> <style=cIsDamage>homing daggers</style> that deal <style=cIsDamage>200%</style> <style=cStack>(+200% per stack)</style> base damage.";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnCharacterDeath += Changes;
            ChangeProc();
        }

        public static void Changes(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchBle(out ILLabel IL_067A),
                    x => x.MatchLdcR4(1.5f)))
            {
                c.Index += 1;
                c.Next.Operand = 2f;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Ceremonial Dagger Damage hook");
            }
        }

        public static void ChangeProc()
        {
            var c = LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/daggerprojectile").GetComponent<ProjectileController>();
            c.procCoefficient = 0f;
        }
    }
}