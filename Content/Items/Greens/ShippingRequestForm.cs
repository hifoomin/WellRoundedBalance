namespace WellRoundedBalance.Items.Greens
{
    public class ShippingRequestForm : ItemBase<ShippingRequestForm>
    {
        public override string Name => ":: Items :: Greens :: Shipping Request Form";
        public override ItemDef InternalPickup => DLC1Content.Items.FreeChest;

        public override string PickupText => "Get a delivery each stage that contains powerful items.";

        public override string DescText => "A <style=cIsUtility>delivery</style> containing 2 items (" + d(whiteItemWeight / (whiteItemWeight + greenItemWeight + redItemWeight)) + "/<style=cIsHealing>" + d(greenItemWeight / (whiteItemWeight + greenItemWeight + redItemWeight)) + "</style>/<style=cIsHealth>" + d(redItemWeight / (whiteItemWeight + greenItemWeight + redItemWeight)) + "</style>) will appear in a random location <style=cIsUtility>on each stage</style>. <style=cStack>(Increases rarity chances of the items per stack).</style>";

        [ConfigField("White Item Weight", "Decimal.", 0.6f)]
        public static float whiteItemWeight;

        [ConfigField("Green Item Weight", "Decimal.", 0.38f)]
        public static float greenItemWeight;

        [ConfigField("Red Item Weight", "Decimal.", 0.02f)]
        public static float redItemWeight;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            Changes();
        }

        private void Changes()
        {
            var shipDt = Addressables.LoadAssetAsync<FreeChestDropTable>("RoR2/DLC1/FreeChest/dtFreeChest.asset").WaitForCompletion();
            shipDt.tier1Weight = whiteItemWeight;
            shipDt.tier2Weight = greenItemWeight;
            shipDt.tier3Weight = redItemWeight;
        }
    }
}