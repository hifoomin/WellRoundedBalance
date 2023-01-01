using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using RoR2.Projectile;
using System;
using UnityEngine;

namespace WellRoundedBalance.Items.Reds
{
    public class PocketICBM : ItemBase
    {
        public override string Name => ":: Items ::: Reds :: Pocket ICBM";
        public override string InternalPickupToken => "moreMissile";

        public override string PickupText => "All Missile items fire an additional missile. Gain a 10% chance to fire a missile.";

        public override string DescText => "All missile items and equipment fire an additional <style=cIsDamage>missile</style>. Gain a <style=cIsDamage>10%</style> <style=cStack>(+10% per stack)</style> chance to fire a missile that deals <style=cIsDamage>300%</style> TOTAL damage.";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.MissileUtils.FireMissile_Vector3_CharacterBody_ProcChainMask_GameObject_float_bool_GameObject_DamageColorIndex_Vector3_float_bool += Changes;
            IL.RoR2.GlobalEventManager.OnHitEnemy += ChangeMissileCount;
            On.RoR2.GlobalEventManager.OnHitEnemy += GlobalEventManager_OnHitEnemy;
        }

        private void GlobalEventManager_OnHitEnemy(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo damageInfo, GameObject victim)
        {
            if (damageInfo.attacker)
            {
                var body = damageInfo.attacker.GetComponent<CharacterBody>();
                if (body)
                {
                    var inventory = body.inventory;
                    if (inventory)
                    {
                        if (!damageInfo.procChainMask.HasProc(ProcType.Missile))
                        {
                            var stack = inventory.GetItemCount(DLC1Content.Items.MoreMissile);
                            if (stack > 0)
                            {
                                if (Util.CheckRoll(10f * damageInfo.procCoefficient * stack, body.master))
                                {
                                    float damage = Util.OnHitProcDamage(damageInfo.damage, body.damage, 3f);
                                    MissileUtils.FireMissile(body.corePosition, body, damageInfo.procChainMask, victim, damage, damageInfo.crit, GlobalEventManager.CommonAssets.missilePrefab, DamageColorIndex.Item, true);
                                }
                            }
                        }
                    }
                }
            }

            orig(self, damageInfo, victim);
        }

        private void ChangeMissileCount(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
               x => x.MatchLdcI4(1),
               x => x.MatchBr(out _),
               x => x.MatchLdcI4(3)))
            {
                c.Index += 2;
                c.Next.Operand = 2;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Pocket I.C.B.M. Missile Count 1 hook");
            }
        }

        public static void Changes(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.After, x => x.MatchCallOrCallvirt(typeof(System.Nullable<Int32>).GetMethod("GetValueOrDefault", new Type[] { }))))
            {
                c.Index += 4;
                c.EmitDelegate<Func<float, float>>((orig) => 0f);
                for (int i = 0; c.TryGotoNext(x => x.MatchCallOrCallvirt(typeof(UnityEngine.Quaternion).GetMethod("AngleAxis", (System.Reflection.BindingFlags)(-1)))); i++)
                {
                    c.Index--;
                    c.EmitDelegate<Func<float, float>>((orig) => (i % 2 == 0) ? 45f : 45f * (-1));
                    c.Index += 2;
                }
                ILLabel label = c.DefineLabel();
                if (c.TryGotoPrev(MoveType.After, x => x.MatchBle(out label)))
                {
                    c.EmitDelegate<Func<bool>>(() => 1 < 2);
                    c.Emit(OpCodes.Brtrue, label);
                    c.GotoLabel(label, MoveType.Before);
                    c.MoveAfterLabels();
                    c.Emit(OpCodes.Ldloc_0);
                    c.Emit(OpCodes.Ldloc, 4);
                    c.Emit(OpCodes.Ldarg_1);
                    c.Emit(OpCodes.Ldarg, 8);
                    c.EmitDelegate<Action<int, FireProjectileInfo, CharacterBody, Vector3>>((stacks, info, body, initDir) =>
                    {
                        if (stacks > 0)
                        {
                            InputBankTest bank = body.GetComponent<InputBankTest>();
                            for (int i = 0; i < (stacks - 1) * 0 + ((1 == 1) ? 1 : (1 > 2) ? 1 - 2 : 0); i++)
                            {
                                info.rotation = Util.QuaternionSafeLookRotation(Quaternion.AngleAxis(45f * ((i % 2 == 0) ? 1 : (-1)), bank ? bank.aimDirection : body.transform.position) * initDir);
                                ProjectileManager.instance.FireProjectile(info);
                            }
                        }
                    });
                }
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Pocket I.C.B.M. Damage, Count, Stacking hook");
            }

            // BIG thanks to RandomlyAwesome
        }
    }
}