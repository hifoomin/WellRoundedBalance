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
        public override string DescText => StackDesc(damageIncrease, damageIncreaseStack,
            init => $"<style=cIsDamage>Increase base damage</style> by <style=cIsDamage>{d(init)}</style>{{Stack}} while out of danger.",
            stack => d(stack));

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
            watchDamage.iconSprite = Sprite.Create(damageIcon, new Rect(0f, 0f, damageIcon.width, damageIcon.height), new Vector2(0f, 0f));
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
            if (sender.inventory && sender.HasBuff(watchDamage))
            {
                args.damageMultAdd += StackAmount(damageIncrease, damageIncreaseStack,
                    sender.inventory.GetItemCount(DLC1Content.Items.FragileDamageBonus));
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

            if (c.TryGotoNext(x => x.MatchLdfld<HealthComponent.ItemCounts>(nameof(HealthComponent.ItemCounts.fragileDamageBonus))) && c.TryGotoNext(x => x.MatchBle(out _)))
            {
                c.Emit(OpCodes.Pop);
                c.Emit(OpCodes.Ldc_I4, int.MaxValue); // try being bigger than this lol
            }
            else Main.WRBLogger.LogError("Failed to apply Delicate Watch Threshold hook");
        }

        public static void ChangeDamage(ILContext il)
        {
            ILCursor c = new(il);
            if (c.TryGotoNext(x => x.MatchLdloc(25)) && c.TryGotoNext(x => x.MatchBle(out _)))
            {
                c.Emit(OpCodes.Pop);
                c.Emit(OpCodes.Ldc_I4, int.MaxValue);
            }
            else Main.WRBLogger.LogError("Failed to apply Delicate Watch Damage hook");
        }
    }

    public class DelicateWatchController : CharacterBody.ItemBehavior
    {
        public void FixedUpdate()
        {
            if (body.HasBuff(DelicateWatch.watchDamage) && !body.outOfDanger) body.RemoveBuff(DelicateWatch.watchDamage);
            if (!body.HasBuff(DelicateWatch.watchDamage) && body.outOfDanger) body.AddBuff(DelicateWatch.watchDamage);
        }
    }
}