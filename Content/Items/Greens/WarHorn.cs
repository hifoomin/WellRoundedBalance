using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace WellRoundedBalance.Items.Greens
{
    public class WarHorn : ItemBase
    {
        public static BuffDef warHornBuff;
        public override string Name => ":: Items :: Greens :: War Horn";
        public override string InternalPickupToken => "energizedOnEquipmentUse";

        public override string PickupText => "Activating your Equipment gives you a burst of attack speed and regeneration.";

        public override string DescText => "Activating your Equipment gives you <style=cIsDamage>+30% attack speed</style> <style=cStack>(+15% per stack)</style> and <style=cIsHealing>+3 hp/s</style> <style=cStack>(+1.5 hp/s per stack)</style> <style=cIsHealing>base health regeneration</style> for <style=cIsDamage>12s</style>.";

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
                args.baseAttackSpeedAdd += 0.3f + 0.15f * (stack - 1);

                var regenStack = 3f + (1.5f * (stack - 1));
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
                        self.characterBody.AddTimedBuff(warHornBuff, 12f);
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
                Main.WRBLogger.LogError("Failed to apply War Horn Duration hook");
            }
        }
    }
}