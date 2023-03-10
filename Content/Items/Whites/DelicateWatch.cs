using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace WellRoundedBalance.Items.Whites
{
    public class DelicateWatch : ItemBase
    {
        public static BuffDef watchDamage;

        public override string Name => ":: Items : Whites :: Delicate Watch";
        public override ItemDef InternalPickup => DLC1Content.Items.FragileDamageBonus;

        public override string PickupText => "Deal bonus damage out of danger.";

        public override string DescText =>
            StackDesc(damageIncrease, damageIncreaseStack, init => $"<style=cIsDamage>Increase base damage</style> by <style=cIsDamage>{d(init)}</style>{{Stack}} while out of danger.", d);

        [ConfigField("Damage Increase", "Decimal.", 0.15f)]
        public static float damageIncrease;

        [ConfigField("Damage Increase per Stack", "Decimal.", 0.15f)]
        public static float damageIncreaseStack;

        [ConfigField("Damage Increase is Hyperbolic", "Decimal, Max value. Set to 0 to make it linear.", 0f)]
        public static float damageIncreaseIsHyperbolic;

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
            IL.RoR2.HealthComponent.TakeDamage += HealthCompoment_TakeDamage;
            IL.RoR2.HealthComponent.UpdateLastHitTime += HealthComponent_UpdateLastHitTime;
            CharacterBody.onBodyInventoryChangedGlobal += CharacterBody_onBodyInventoryChangedGlobal;
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
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
            else Logger.LogError("Failed to apply Delicate Watch Threshold hook");
        }

        public static void HealthCompoment_TakeDamage(ILContext il)
        {
            ILCursor c = new(il);
            int idx = GetItemLoc(c, nameof(DLC1Content.Items.FragileDamageBonus));
            if (idx != -1 && c.TryGotoNext(MoveType.After, x => x.MatchLdloc(idx)))
            {
                c.Emit(OpCodes.Pop);
                c.Emit(OpCodes.Ldc_I4_0);
            }
            else Logger.LogError("Failed to apply Delicate Watch Damage hook");
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