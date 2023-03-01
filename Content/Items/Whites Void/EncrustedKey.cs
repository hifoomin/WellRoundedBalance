namespace WellRoundedBalance.Items.Whites
{
    public class EncrustedKey : ItemBase
    {
        public override string Name => ":: Items :::::: Voids :: Encrusted Key";
        public override string InternalPickupToken => "treasureCacheVoid";

        public override string PickupText => "Gain access to an Encrusted Cache that contains a void item. <style=cIsVoid>Corrupts all Rusted Keys</style>.";
        public override string DescText => "A <style=cIsUtility>hidden cache</style> containing an item (41.66%/<style=cIsHealing>41.66%</style>/<style=cIsHealth>10%</style>) will appear in a random location <style=cIsUtility>on each stage</style>. Opening the cache <style=cIsUtility>consumes</style> this item. <style=cIsVoid>Corrupts all Rusted Keys</style>.";

        [ConfigField("Choice Amount", 2)]
        public static int choiceAmount;

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
            var vlockbox = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/TreasureCacheVoid/LockboxVoid.prefab").WaitForCompletion().GetComponent<OptionChestBehavior>();
            vlockbox.numOptions = choiceAmount;

            var vlockDt = Addressables.LoadAssetAsync<BasicPickupDropTable>("RoR2/DLC1/TreasureCacheVoid/dtVoidLockbox.asset").WaitForCompletion();
        }
    }
}