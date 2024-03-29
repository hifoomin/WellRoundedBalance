﻿using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;

namespace WellRoundedBalance.Items.Reds
{
    public class LaserScope : ItemBase<LaserScope>
    {
        public override string Name => ":: Items ::: Reds :: Laser Scope";
        public override ItemDef InternalPickup => DLC1Content.Items.CritDamage;

        public override string PickupText => (removeBulletFalloff ? "Remove bullet falloff. " : "") +
                                             "Your 'Critical Strikes' deal an additional 50% damage.";

        public override string DescText => (removeBulletFalloff ? "Remove <style=cIsDamage>bullet falloff</style>. " : "") +
                                           (criticalChance > 0 ? "Gain <style=cIsDamage>" + criticalChance + "% critical chance</style>. " : "") +
                                           "<style=cIsDamage>Critical Strikes</style> deal an additional <style=cIsDamage>50%</style> <style=cStack>(+50% per stack)</style> damage.";

        [ConfigField("Remove bullet falloff?", true)]
        public static bool removeBulletFalloff;

        [ConfigField("Critical Chance", 10f)]
        public static float criticalChance;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
            IL.RoR2.BulletAttack.DefaultHitCallbackImplementation += BulletAttack_DefaultHitCallbackImplementation;
        }

        private void BulletAttack_DefaultHitCallbackImplementation(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdfld(typeof(BulletAttack), "damage"),
                x => x.MatchLdloc(out _)))
            {
                c.Index += 2;
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<float, DamageInfo, float>>((self, damageInfo) =>
                {
                    var attackerBody = damageInfo.attacker.GetComponent<CharacterBody>();
                    if (attackerBody && removeBulletFalloff)
                    {
                        var inventory = attackerBody.inventory;
                        if (inventory)
                        {
                            return inventory.GetItemCount(DLC1Content.Items.CritDamage) > 0 ? 1f : self;
                        }
                        else
                        {
                            return self;
                        }
                    }
                    else
                    {
                        return self;
                    }
                });
            }
            else
            {
                Logger.LogError("Failed to apply Laser Scope Bullet Falloff Removal 1 hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdfld(typeof(BulletAttack), "force"),
                x => x.MatchLdloc(out _)))
            {
                c.Index += 2;
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<float, DamageInfo, float>>((self, damageInfo) =>
                {
                    var attackerBody = damageInfo.attacker.GetComponent<CharacterBody>();
                    if (attackerBody && removeBulletFalloff)
                    {
                        var inventory = attackerBody.inventory;
                        if (inventory)
                        {
                            return inventory.GetItemCount(DLC1Content.Items.CritDamage) > 0 ? 1f : self;
                        }
                        else
                        {
                            return self;
                        }
                    }
                    else
                    {
                        return self;
                    }
                });
            }
            else
            {
                Logger.LogError("Failed to apply Laser Scope Bullet Falloff Removal 2 hook");
            }
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.inventory)
            {
                var stack = sender.inventory.GetItemCount(DLC1Content.Items.CritDamage);
                if (stack > 0)
                {
                    args.critAdd += criticalChance;
                }
            }
        }
    }
}