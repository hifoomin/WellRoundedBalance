using MonoMod.Cil;
using RoR2;
using RoR2.Projectile;
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
            IL.RoR2.MissileUtils.FireMissile_Vector3_CharacterBody_ProcChainMask_GameObject_float_bool_GameObject_DamageColorIndex_Vector3_float_bool += ChangeDamage;
            // IL.RoR2.GlobalEventManager.OnHitEnemy += ChangeMissileCount;
            On.RoR2.MissileUtils.FireMissile_Vector3_CharacterBody_ProcChainMask_GameObject_float_bool_GameObject_DamageColorIndex_Vector3_float_bool += RewriteStupidShit;
        }

        private void RewriteStupidShit(On.RoR2.MissileUtils.orig_FireMissile_Vector3_CharacterBody_ProcChainMask_GameObject_float_bool_GameObject_DamageColorIndex_Vector3_float_bool orig, UnityEngine.Vector3 position, RoR2.CharacterBody attackerBody, RoR2.ProcChainMask procChainMask, UnityEngine.GameObject victim, float missileDamage, bool isCrit, UnityEngine.GameObject projectilePrefab, RoR2.DamageColorIndex damageColorIndex, UnityEngine.Vector3 initialDirection, float force, bool addMissileProc)
        {
            int? num;
            Inventory inventory = attackerBody.inventory;
            if (attackerBody == null)
            {
                num = null;
            }
            else
            {
                num = ((inventory != null) ? new int?(inventory.GetItemCount(DLC1Content.Items.MoreMissile)) : null);
            }
            // dont really understand why nullable and null coalescing here :Thonk:
            int num2 = num ?? 0;
            float num3 = Mathf.Max(1f, 1f + Damage * (float)(num2 - 1));
            InputBankTest component = attackerBody.GetComponent<InputBankTest>();
            ProcChainMask procChainMask2 = procChainMask;
            if (addMissileProc)
            {
                procChainMask2.AddProc(ProcType.Missile);
            }
            FireProjectileInfo fireProjectileInfo = new()
            {
                projectilePrefab = projectilePrefab,
                position = position,
                rotation = Util.QuaternionSafeLookRotation(initialDirection),
                procChainMask = procChainMask2,
                target = victim,
                owner = attackerBody.gameObject,
                damage = missileDamage * num3,
                crit = isCrit,
                force = force,
                damageColorIndex = damageColorIndex
            };
            ProjectileManager.instance.FireProjectile(fireProjectileInfo);

            int MissileCount = StackMissiles > 0 ? Missiles + StackMissiles * (inventory.GetItemCount(DLC1Content.Items.MoreMissile) - 1) : Missiles;

            if (num2 > 0)
            {
                Vector3 axis = component ? component.aimDirection : attackerBody.transform.position;
                for (int i = 0; i < MissileCount; i++)
                {
                    FireProjectileInfo fireProjectileInfo2 = fireProjectileInfo;
                    float rotation;
                    if (Missiles % 2 == 0)
                    {
                        rotation = -Rotation;
                    }
                    else
                    {
                        rotation = Rotation;
                    }
                    fireProjectileInfo2.rotation = Util.QuaternionSafeLookRotation(Quaternion.AngleAxis(rotation, axis) * initialDirection);
                    ProjectileManager.instance.FireProjectile(fireProjectileInfo2);
                }
            }
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

        public static void ChangeDamage(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(1f),
                    x => x.MatchLdcR4(1f),
                    x => x.MatchLdcR4(0.5f)))
            {
                c.Index += 2;
                c.Next.Operand = Damage;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Pocket I.C.B.M. Damage hook");
            }
        }
    }
}