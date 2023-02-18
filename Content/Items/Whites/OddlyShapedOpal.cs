using MonoMod.Cil;

namespace WellRoundedBalance.Items.Whites
{
    public class OddlyShapedOpal : ItemBase
    {
        public static BuffDef opalArmor;
        public override string Name => ":: Items : Whites :: Oddly Shaped Opal";
        public override string InternalPickupToken => "outOfCombatArmor";

        public override string PickupText => "Reduce damage the first time you are hit.";

        public override string DescText => "<style=cIsHealing>Increase armor</style> by <style=cIsHealing>40</style> <style=cStack>(+40 per stack)</style> while out of combat.";

        public override void Init()
        {
            var opalIcon = Utils.Paths.Texture2D.texBuffUtilitySkillArmor.Load<Texture2D>();

            opalArmor = ScriptableObject.CreateInstance<BuffDef>();
            opalArmor.isHidden = false;
            opalArmor.canStack = false;
            opalArmor.isCooldown = false;
            opalArmor.isDebuff = false;
            opalArmor.iconSprite = Sprite.Create(opalIcon, new Rect(0f, 0f, (float)opalIcon.width, (float)opalIcon.height), new Vector2(0f, 0f));
            opalArmor.buffColor = new Color32(196, 194, 255, 255);
            opalArmor.name = "Oddly-shaped Opal Armor";

            ContentAddition.AddBuffDef(opalArmor);

            base.Init();
        }

        public override void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
            CharacterBody.onBodyInventoryChangedGlobal += CharacterBody_onBodyInventoryChangedGlobal;
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            var inventory = sender.inventory;
            if (sender.HasBuff(opalArmor) && inventory)
            {
                var stack = inventory.GetItemCount(DLC1Content.Items.OutOfCombatArmor);
                args.armorAdd += 40f * stack;
            }
        }

        private void CharacterBody_onBodyInventoryChangedGlobal(CharacterBody characterBody)
        {
            if (NetworkServer.active)
            {
                characterBody.AddItemBehavior<OutOfCombatArmorBehavior>(characterBody.inventory.GetItemCount(DLC1Content.Items.OutOfCombatArmor));
            }
        }
    }

    public class OutOfCombatArmorBehavior : CharacterBody.ItemBehavior
    {
        private void SetProvidingBuff(bool shouldProvideBuff)
        {
            if (shouldProvideBuff == providingBuff)
            {
                return;
            }
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