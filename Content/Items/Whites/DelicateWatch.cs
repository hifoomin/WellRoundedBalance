using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;

namespace WellRoundedBalance.Items.Whites
{
    public class DelicateWatch : ItemBase
    {
        public static BuffDef watchDamage;

        public override string Name => ":: Items : Whites :: Delicate Watch";
        public override string InternalPickupToken => "fragileDamageBonus";

        public override string PickupText => "Deal bonus damage out of danger.";
        public override string DescText => "<style=cIsDamage>Increase base damage</style> by <style=cIsDamage>" + d(damageIncrease) + "</style> <style=cStack>(+" + d(damageIncreaseStack) + " per stack)</style> while out of danger.";

        [ConfigField("Damage Increase", "Decimal.", 0.15f)]
        public static float damageIncrease;

        [ConfigField("Damage Increase per Stack", "Decimal.", 0.15f)]
        public static float damageIncreaseStack;

        public override void Init()
        {
            var damageIcon = Utils.Paths.Texture2D.texBuffFullCritIcon.Load<Texture2D>();

            watchDamage = ScriptableObject.CreateInstance<BuffDef>();
            watchDamage.isHidden = false;
            watchDamage.canStack = false;
            watchDamage.isDebuff = false;
            watchDamage.buffColor = new Color32(208, 165, 136, 255);
            watchDamage.iconSprite = Sprite.Create(damageIcon, new Rect(0f, 0f, (float)damageIcon.width, (float)damageIcon.height), new Vector2(0f, 0f));
            watchDamage.name = "Delicate Watch Damage Boost";

            ContentAddition.AddBuffDef(watchDamage);

            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.HealthComponent.TakeDamage += ChangeDamage;
            IL.RoR2.HealthComponent.UpdateLastHitTime += ChangeThreshold;
            CharacterBody.onBodyInventoryChangedGlobal += CharacterBody_onBodyInventoryChangedGlobal;
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.inventory)
            {
                var stack = sender.inventory.GetItemCount(DLC1Content.Items.FragileDamageBonus);
                if (sender.HasBuff(watchDamage))
                {
                    args.damageMultAdd += (damageIncreaseStack * (stack - 1)) + damageIncrease;
                }
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

        private void ChangeThreshold(ILContext il)
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
                    if ((self.health + self.shield) / self.fullCombinedHealth < -float.MaxValue)
                    {
                        Check = true;
                        return Check;
                    }
                    else
                    {
                        Check = false;
                        return Check;
                    }
                });
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Delicate Watch Threshold hook");
            }
        }

        public static void ChangeDamage(ILContext il)
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
                c.Next.Operand = 0f;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Delicate Watch Damage hook");
            }
        }
    }

    public class DelicateWatchController : CharacterBody.ItemBehavior
    {
        public void FixedUpdate()
        {
            if (body.HasBuff(DelicateWatch.watchDamage) && !body.outOfDanger)
            {
                body.RemoveBuff(DelicateWatch.watchDamage);
            }
            if (!body.HasBuff(DelicateWatch.watchDamage) && body.outOfDanger)
            {
                body.AddBuff(DelicateWatch.watchDamage);
            }
        }
    }
}