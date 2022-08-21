using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using RoR2.Projectile;
using System;
using UnityEngine;

namespace UltimateCustomRun.Items.Reds
{
    public class PocketICBM : ItemBase
    {
        public static int Missiles;
        public static int StackMissiles;
        public static float Damage;
        public static float Rotation;
        public override string Name => ":: Items ::: Reds :: Pocket ICBM";
        public override string InternalPickupToken => "moreMissile";
        public override bool NewPickup => true;
        public override string PickupText => "All Missile items" + (Damage > 0f ? " deal more damage and" : "") + " fire an additional" + (Missiles == 1 ? " missile." : Missiles + " missiles.");

        public override string DescText => "All missile items and equipment fire an additional <style=cIsDamage>" + Missiles + "</style> " + (StackMissiles > 0 ? " <style=cStack>(+" + StackMissiles + " per stack)</style>" : "") + "<style=cIsDamage>" + (Missiles == 1 ? " missile" : " missiles") + "</style>." + (Damage > 0f ? " Increase missile damage by <style=cIsDamage>0%</style> <style=cStack>(+" + d(Damage) + " per stack)</style>." : "");

        public override void Init()
        {
            Damage = ConfigOption(0.5f, "Missile Damage Increase", "Decimal. Per Stack. Vanilla is 0.5");
            Missiles = ConfigOption(2, "Base Missiles Per Missile", "Vanilla is 3");
            StackMissiles = ConfigOption(0, "Stack Missiles Per Missile", "Per Stack. Vanilla is 0");
            Rotation = ConfigOption(45f, "Missile Rotation", "Vanilla is 45");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.MissileUtils.FireMissile_Vector3_CharacterBody_ProcChainMask_GameObject_float_bool_GameObject_DamageColorIndex_Vector3_float_bool += Changes;
            // IL.RoR2.GlobalEventManager.OnHitEnemy += ChangeMissileCount;
        }

        private void ChangeMissileCount(ILContext il)
        {
            ILCursor c = new(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchCallOrCallvirt<CharacterBody>("get_inventory"));
            Inventory inv = (Inventory)c.Next.Operand;

            if (c.TryGotoNext(MoveType.Before,
               x => x.MatchLdcI4(1),
               x => x.MatchLdcR4(1f),
               x => x.MatchLdcI4(3)))
            {
                c.Index += 2;
                c.Next.Operand = StackMissiles > 0 ? Missiles + StackMissiles * (inv.GetItemCount(DLC1Content.Items.MoreMissile) - 1) : Missiles;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Pocket I.C.B.M. Missile Count 1 hook");
            }
        }

        public static void Changes(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.After, x => x.MatchCallOrCallvirt(typeof(System.Nullable<Int32>).GetMethod("GetValueOrDefault", new Type[] { }))))
            {
                c.Index += 4;
                c.EmitDelegate<Func<float, float>>((orig) => Damage);

                for (int i = 0; c.TryGotoNext(x => x.MatchCallOrCallvirt(typeof(UnityEngine.Quaternion).GetMethod("AngleAxis", (System.Reflection.BindingFlags)(-1)))); i++)
                {
                    c.Index--;
                    c.EmitDelegate<Func<float, float>>((orig) => Rotation * ((i % 2 == 0) ? 1 : (-1)));
                    c.Index += 2;
                }

                ILLabel label = c.DefineLabel();

                if (c.TryGotoPrev(MoveType.After, x => x.MatchBle(out label)))
                {
                    c.EmitDelegate<Func<bool>>(() => Missiles < 2);
                    c.Emit(OpCodes.Brtrue, label);
                    c.GotoLabel(label, MoveType.Before);
                    c.MoveAfterLabels();
                    c.Emit(OpCodes.Ldloc_0);
                    c.Emit(OpCodes.Ldloc, 4);
                    c.Emit(OpCodes.Ldarg_1);
                    c.Emit(OpCodes.Ldarg, 8);
                    c.EmitDelegate<Action<int, FireProjectileInfo, CharacterBody, Vector3>>((stacks, info, body, initDir) =>
                    {
                        InputBankTest bank = body.GetComponent<InputBankTest>();
                        for (int i = 0; i < (stacks - 1) * StackMissiles + ((Missiles == 1) ? 1 : (Missiles > 2) ? Missiles - 2 : 0); i++)
                        {
                            info.rotation = Util.QuaternionSafeLookRotation(Quaternion.AngleAxis(Rotation * ((i % 2 == 0) ? 1 : (-1)), bank ? bank.aimDirection : body.transform.position) * initDir);
                            ProjectileManager.instance.FireProjectile(info);
                        }
                    });
                }
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Pocket I.C.B.M. Damage hook");
            }

            // big thanks to RandomlyAwesome
        }
    }
}