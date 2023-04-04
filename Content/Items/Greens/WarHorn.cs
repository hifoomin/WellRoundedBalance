using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace WellRoundedBalance.Items.Greens
{
    public class WarHorn : ItemBase<WarHorn>
    {
        public static BuffDef warHornBuff;
        public override string Name => ":: Items :: Greens :: War Horn";
        public override ItemDef InternalPickup => RoR2Content.Items.EnergizedOnEquipmentUse;

        public override string PickupText => "Activating your Equipment gives you a burst of attack speed" +
                                             (baseAttackSpeedGain > 0 || attackSpeedGainPerStack > 0 ? " and regeneration." : ".");

        public override string DescText => "Activating your Equipment gives you <style=cIsDamage>+" + d(baseAttackSpeedGain) + " attack speed</style> <style=cStack>(+" + d(attackSpeedGainPerStack) + " per stack)</style> and <style=cIsHealing>+" + baseRegenerationGain + " hp/s</style> <style=cStack>(+" + regenerationGainPerStack + " hp/s per stack)</style> <style=cIsHealing>base health regeneration</style> for <style=cIsDamage>" + buffDuration + "s</style>.";

        [ConfigField("Base Attack Speed Gain", 0.3f)]
        public static float baseAttackSpeedGain;

        [ConfigField("Attack Speed Gain Per Stack", 0.15f)]
        public static float attackSpeedGainPerStack;

        [ConfigField("Base Regeneration Gain", 3f)]
        public static float baseRegenerationGain;

        [ConfigField("Regeneration Gain Per Stack", 1.5f)]
        public static float regenerationGainPerStack;

        [ConfigField("Buff Duration", 12f)]
        public static float buffDuration;

        public override void Init()
        {
            var warhornIcon = Utils.Paths.Texture2D.texBuffWarHornIcon.Load<Texture2D>();

            warHornBuff = ScriptableObject.CreateInstance<BuffDef>();
            warHornBuff.isDebuff = false;
            warHornBuff.canStack = false;
            warHornBuff.isHidden = false;
            warHornBuff.isCooldown = false;
            warHornBuff.buffColor = new Color32(255, 201, 14, 255);
            warHornBuff.iconSprite = Sprite.Create(warhornIcon, new Rect(0f, 0f, (float)warhornIcon.width, (float)warhornIcon.height), new Vector2(0f, 0f));

            ContentAddition.AddBuffDef(warHornBuff);

            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.EquipmentSlot.OnEquipmentExecuted += EquipmentSlot_OnEquipmentExecuted;
            On.RoR2.EquipmentSlot.OnEquipmentExecuted += EquipmentSlot_OnEquipmentExecuted1;
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            var inventory = sender.inventory;
            if (inventory && sender.HasBuff(warHornBuff))
            {
                var stack = inventory.GetItemCount(RoR2Content.Items.EnergizedOnEquipmentUse);
                args.baseAttackSpeedAdd += baseAttackSpeedGain + attackSpeedGainPerStack * (stack - 1);

                var regenStack = baseRegenerationGain + (regenerationGainPerStack * (stack - 1));
                args.baseRegenAdd += regenStack + 0.2f * regenStack * (sender.level - 1);
            }
        }

        private void EquipmentSlot_OnEquipmentExecuted1(On.RoR2.EquipmentSlot.orig_OnEquipmentExecuted orig, EquipmentSlot self)
        {
            if (NetworkServer.active)
            {
                if (self.characterBody && self.inventory)
                {
                    var stack = self.inventory.GetItemCount(RoR2Content.Items.EnergizedOnEquipmentUse);
                    if (stack > 0)
                    {
                        self.characterBody.AddTimedBuff(warHornBuff, buffDuration);
                    }
                }
                orig(self);
            }
        }

        private void EquipmentSlot_OnEquipmentExecuted(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcI4(8),
                    x => x.MatchLdcI4(4)))
            {
                c.Remove();
                c.Emit(OpCodes.Ldc_I4, 0);
                c.Remove();
                c.Emit(OpCodes.Ldc_I4, 0);
            }
            else
            {
                Logger.LogError("Failed to apply War Horn Duration hook");
            }
        }
    }
}