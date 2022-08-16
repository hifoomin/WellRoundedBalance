using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using RoR2.Projectile;
using System;
using UnityEngine;

namespace UltimateCustomRun.Items.Yellows
{
    public class MoltenPerforator : ItemBase
    {
        public static float Damage;
        public static float Chance;
        public static float ProcCoefficient;
        public static int Count;

        public override string Name => ":: Items :::: Yellows :: Molten Perforator";
        public override string InternalPickupToken => "fireballsOnHit";
        public override bool NewPickup => false;

        public override string PickupText => "";

        public override string DescText => "<style=cIsDamage>" + Chance + "%</style> chance on hit to call forth <style=cIsDamage>" + Count + " magma balls</style> from an enemy, dealing <style=cIsDamage>" + d(Damage) + "</style> <style=cStack>(+" + d(Damage) + " per stack)</style> TOTAL damage and <style=cIsDamage>igniting</style> all enemies.";

        public override void Init()
        {
            Damage = ConfigOption(3f, "Damage", "Decimal. Per Stack. Vanilla is 3");
            Chance = ConfigOption(10f, "Chance", "Vanilla is 10");
            ProcCoefficient = ConfigOption(1f, "Proc Coefficient", "Vanilla is 1");
            Count = ConfigOption(3, "Magma ball Count", "Vanilla is 3");
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
                    x => x.MatchStloc(115),
                    x => x.MatchLdcR4(10f)))
            {
                c.Index += 1;
                c.Next.Operand = Chance;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Molten Perforator Chance hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                    //x => x.MatchStloc(117),
                    x => x.MatchLdcR4(3f),
                    x => x.MatchLdloc(17)))
            {
                c.Next.Operand = Damage;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Molten Perforator Damage hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcI4(3),
                    x => x.MatchStloc(117)))
            {
                c.Remove();
                c.Emit(OpCodes.Ldc_I4, Count);
                /*
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<int, int>>((useless) =>
                {
                    return Count;
                });
                */
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Molten Perforator Count hook");
            }
        }

        private void ChangeProcCoefficient()
        {
            var m = LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/firemeatball").GetComponent<ProjectileController>();
            m.procCoefficient = ProcCoefficient;
        }
    }
}