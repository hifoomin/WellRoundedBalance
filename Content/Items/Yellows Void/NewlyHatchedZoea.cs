namespace WellRoundedBalance.Items.Yellows
{
    public class NewlyHatchedZoea : ItemBase<NewlyHatchedZoea>
    {
        public override string Name => ":: Items :::::: Voids :: Newly Hatched Zoea";
        public override ItemDef InternalPickup => DLC1Content.Items.VoidMegaCrabItem;

        public override string PickupText => "Periodically recruit allies from the <style=cIsVoid>Void</style>. <style=cIsVoid>Corrupts all boss items</style>.";

        public override string DescText => "Every <style=cIsUtility>60</style><style=cStack> (-50% per stack)</style> seconds, gain a random <style=cIsVoid>Void</style> ally. Can have up to <style=cIsUtility>1</style><style=cStack> (+1 per stack)</style> allies at a time. <style=cIsVoid>Corrupts all boss items</style>.";

        [ConfigField("Void Ally Damage Bonus", "1 = 10%", 3)]
        public static int voidAllyDamageBonus;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            CharacterBody.onBodyStartGlobal += CharacterBody_onBodyStartGlobal;
        }

        private void CharacterBody_onBodyStartGlobal(CharacterBody body)
        {
            var inventory = body.inventory;
            if (!inventory)
            {
                return;
            }
            var stack = inventory.GetItemCount(RoR2Content.Items.BoostDamage);
            {
                switch (body.name)
                {
                    case "VoidJailerAllyBody(Clone)":
                        if (stack == 0) inventory.GiveItem(RoR2Content.Items.BoostDamage, voidAllyDamageBonus);
                        break;

                    case "NullifierAllyBody(Clone)":
                        if (stack == 0) inventory.GiveItem(RoR2Content.Items.BoostDamage, voidAllyDamageBonus);
                        break;

                    case "VoidMegaCrabAllyBody(Clone)":
                        if (stack == 0) inventory.GiveItem(RoR2Content.Items.BoostDamage, voidAllyDamageBonus);
                        break;
                }
            }
        }
    }
}