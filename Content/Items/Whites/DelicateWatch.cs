using System;

namespace WellRoundedBalance.Items.Whites
{
    public class DelicateWatch : ItemBase<DelicateWatch>
    {
        public static BuffDef watchDamage;

        public override string Name => ":: Items : Whites :: Delicate Watch";
        public override ItemDef InternalPickup => DLC1Content.Items.FragileDamageBonus;

        public override string PickupText => rework ? "Deal bonus damage out of danger." : "Deal bonus damage. Breaks at low health.";

        public override string DescText =>
            rework ? StackDesc(damageIncrease, damageIncreaseStack, init => $"<style=cIsDamage>Increase base damage</style> by <style=cIsDamage>{d(init)}</style>{{Stack}} while out of danger.", d) : "Increase damage by <style=cIsDamage>" + d(damageIncrease) + "</style> <style=cStack>(+" + d(damageIncrease) + " per stack)</style>. Taking damage to below <style=cIsHealth>" + d(threshold) + " health</style> <style=cIsUtility>breaks</style> this item.";

        [ConfigField("Damage Increase", "Decimal.", 0.15f)]
        public static float damageIncrease;

        [ConfigField("Damage Increase per Stack", "Decimal.", 0.15f)]
        public static float damageIncreaseStack;

        [ConfigField("Damage Increase is Hyperbolic", "Decimal, Max value. Set to 0 to make it linear.", 0f)]
        public static float damageIncreaseIsHyperbolic;

        [ConfigField("Enable rework?", "Reverts to vanilla item and stacking behavior if false.", true)]
        public static bool rework;

        [ConfigField("Health Threshold", "Only applies if the rework is disabled.", 0.25f)]
        public static float threshold;

        public override void Init()
        {
            watchDamage = ScriptableObject.CreateInstance<BuffDef>();
            watchDamage.isHidden = false;
            watchDamage.canStack = false;
            watchDamage.isDebuff = false;
            watchDamage.buffColor = new Color32(208, 165, 136, 255);
            watchDamage.iconSprite = Main.wellroundedbalance.LoadAsset<Sprite>("texBuffDelicateWatchIcon.png");
            watchDamage.name = "Delicate Watch Damage Boost";

            ContentAddition.AddBuffDef(watchDamage);

            base.Init();
        }

        public override void Hooks()
        {
            if (rework)
            {
                IL.RoR2.HealthComponent.TakeDamageProcess += HealthCompoment_TakeDamageProcess;
                IL.RoR2.HealthComponent.UpdateLastHitTime += HealthComponent_UpdateLastHitTime;
                CharacterBody.onBodyInventoryChangedGlobal += CharacterBody_onBodyInventoryChangedGlobal;
                RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
            }
            else
            {
                IL.RoR2.HealthComponent.TakeDamageProcess += HealthComponent_TakeDamageProcess;
                IL.RoR2.HealthComponent.UpdateLastHitTime += HealthComponent_UpdateLastHitTime1;
            }
        }

        private void HealthComponent_UpdateLastHitTime1(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdfld<HealthComponent.ItemCounts>("fragileDamageBonus"),
                x => x.MatchLdcI4(0),
                x => x.MatchBle(out _),
                x => x.MatchLdarg(0),
                x => x.MatchCallOrCallvirt<HealthComponent>("get_isHealthLow")
            ))
            {
                c.Index += 5;
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<bool, HealthComponent, bool>>((Check, self) =>
                {
                    if ((self.health + self.shield) / self.fullCombinedHealth < threshold)
                    {
                        Check = true;
                    }
                    else
                    {
                        Check = false;
                    }
                    return Check;
                });
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Delicate Watch Threshold 1 hook");
            }
        }

        private void HealthComponent_TakeDamageProcess(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchBle(out _),
                    x => x.MatchLdloc(out _),
                    x => x.MatchLdcR4(1),
                    x => x.MatchLdloc(out _),
                    x => x.MatchConvR4(),
                    x => x.MatchLdcR4(0.2f)))
            {
                c.Index += 5;
                c.Next.Operand = damageIncrease;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Delicate Watch Damage 1 hook");
            }
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.inventory && sender.HasBuff(watchDamage))
            {
                args.damageMultAdd += StackAmount(damageIncrease, damageIncreaseStack,
                    sender.inventory.GetItemCount(DLC1Content.Items.FragileDamageBonus), damageIncreaseIsHyperbolic);
            }
        }

        private void CharacterBody_onBodyInventoryChangedGlobal(CharacterBody body)
        {
            if (NetworkServer.active)
            {
                var stack = body.inventory.GetItemCount(DLC1Content.Items.FragileDamageBonus);
                body.AddItemBehavior<DelicateWatchController>(stack);
            }
        }

        private void HealthComponent_UpdateLastHitTime(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.After, x => x.MatchLdfld<HealthComponent.ItemCounts>(nameof(HealthComponent.ItemCounts.fragileDamageBonus))))
            {
                c.Emit(OpCodes.Pop);
                c.Emit(OpCodes.Ldc_I4_0);
            }
            else Logger.LogError("Failed to apply Delicate Watch Threshold 2 hook");
        }

        public static void HealthCompoment_TakeDamageProcess(ILContext il)
        {
            ILCursor c = new(il);
            int idx = GetItemLoc(c, nameof(DLC1Content.Items.FragileDamageBonus));
            if (idx != -1 && c.TryGotoNext(MoveType.After, x => x.MatchLdloc(idx)))
            {
                c.Emit(OpCodes.Pop);
                c.Emit(OpCodes.Ldc_I4_0);
            }
            else Logger.LogError("Failed to apply Delicate Watch Damage 2 hook");
        }
    }

    public class DelicateWatchController : CharacterBody.ItemBehavior
    {
        public void FixedUpdate()
        {
            if (stack > 0)
            {
                if (body.HasBuff(DelicateWatch.watchDamage) && !body.outOfDanger) body.RemoveBuff(DelicateWatch.watchDamage);
                if (!body.HasBuff(DelicateWatch.watchDamage) && body.outOfDanger) body.AddBuff(DelicateWatch.watchDamage);
            }
        }

        public void OnDestroy()
        {
            if (body.HasBuff(DelicateWatch.watchDamage))
                body.RemoveBuff(DelicateWatch.watchDamage);
        }
    }
}