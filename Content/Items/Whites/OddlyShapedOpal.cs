using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace WellRoundedBalance.Items.Whites
{
    public class OddlyShapedOpal : ItemBase
    {
        public static BuffDef opalArmor;
        public override string Name => ":: Items : Whites :: Oddly Shaped Opal";
        public override string InternalPickupToken => "outOfCombatArmor";

        public override string PickupText => "Reduce damage the first time you are hit.";

        public override string DescText =>
            StackDesc(armorGain, armorGainStack, init => $"<style=cIsHealing>Increase armor</style> by <style=cIsHealing>{init}</style>{{Stack}} while out of combat.", noop);

        [ConfigField("Armor Gain", 40f)]
        public static float armorGain;

        [ConfigField("Armor Gain per Stack", 40f)]
        public static float armorGainStack;

        [ConfigField("Armor Gain is Hyperbolic", "Decimal, Max value. Set to 0 to make it linear.", 0f)]
        public static float armorGainIsHyperbolic;

        public override void Init()
        {
            var opalIcon = Utils.Paths.Texture2D.texBuffUtilitySkillArmor.Load<Texture2D>();

            opalArmor = ScriptableObject.CreateInstance<BuffDef>();
            opalArmor.isHidden = false;
            opalArmor.canStack = false;
            opalArmor.isCooldown = false;
            opalArmor.isDebuff = false;
            opalArmor.iconSprite = Sprite.Create(opalIcon, new Rect(0f, 0f, opalIcon.width, opalIcon.height), new Vector2(0f, 0f));
            opalArmor.buffColor = new Color32(196, 194, 255, 255);
            opalArmor.name = "Oddly-shaped Opal Armor";

            ContentAddition.AddBuffDef(opalArmor);

            base.Init();
        }

        public override void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
            CharacterBody.onBodyInventoryChangedGlobal += CharacterBody_onBodyInventoryChangedGlobal;
            On.RoR2.OutOfCombatArmorBehavior.SetProvidingBuff += OutOfCombatArmorBehavior_SetProvidingBuff;
            On.RoR2.OutOfCombatArmorBehavior.FixedUpdate += OutOfCombatArmorBehavior_FixedUpdate;
            On.RoR2.CharacterBody.UpdateAllTemporaryVisualEffects += CharacterBody_UpdateAllTemporaryVisualEffects;
        }

        private void CharacterBody_UpdateAllTemporaryVisualEffects(On.RoR2.CharacterBody.orig_UpdateAllTemporaryVisualEffects orig, CharacterBody self)
        {
            self.UpdateSingleTemporaryVisualEffect(ref self.outOfCombatArmorEffectInstance, CharacterBody.AssetReferences.outOfCombatArmorEffectPrefab, self.radius, self.HasBuff(opalArmor), "");
            orig(self);
        }

        private void OutOfCombatArmorBehavior_FixedUpdate(On.RoR2.OutOfCombatArmorBehavior.orig_FixedUpdate orig, CharacterBody.ItemBehavior self)
        {
            return;
        }

        private void OutOfCombatArmorBehavior_SetProvidingBuff(On.RoR2.OutOfCombatArmorBehavior.orig_SetProvidingBuff orig, CharacterBody.ItemBehavior self, bool shouldProvideBuff)
        {
            return;
        }

        private void CharacterBody_RecalculateStats(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdsfld("RoR2.DLC1Content/Buffs", "OutOfCombatArmorBuff")))
            {
                c.Remove();
                c.Emit<Buffs.Useless>(OpCodes.Ldsfld, nameof(Buffs.Useless.oddlyShapedOpalUseless));
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Oddly-shaped Opal Removal hook");
            }
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            var inventory = sender.inventory;
            if (sender.HasBuff(opalArmor) && inventory)
                args.armorAdd += StackAmount(armorGain, armorGainStack,
                    inventory.GetItemCount(DLC1Content.Items.OutOfCombatArmor), armorGainIsHyperbolic);
        }

        private void CharacterBody_onBodyInventoryChangedGlobal(CharacterBody characterBody)
        {
            if (NetworkServer.active) characterBody.AddItemBehavior<OutOfCombatArmorBehavior>(characterBody.inventory.GetItemCount(DLC1Content.Items.OutOfCombatArmor));
        }
    }

    public class OutOfCombatArmorBehavior : CharacterBody.ItemBehavior
    {
        private void SetProvidingBuff(bool shouldProvideBuff)
        {
            if (shouldProvideBuff == providingBuff) return;
            providingBuff = shouldProvideBuff;
            if (providingBuff)
            {
                body.AddBuff(OddlyShapedOpal.opalArmor);
                return;
            }
            body.RemoveBuff(OddlyShapedOpal.opalArmor);
        }

        private void OnDisable()
        {
            SetProvidingBuff(false);
        }

        private void FixedUpdate()
        {
            SetProvidingBuff(body.outOfCombat);
        }

        private bool providingBuff;
    }
}