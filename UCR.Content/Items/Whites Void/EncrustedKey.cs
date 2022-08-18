using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace UltimateCustomRun.Items.Whites
{
    public class EncrustedKey : ItemBase
    {
        public static int OptionCount;
        public static float VoidWhiteChance;
        public static float VoidGreenChance;
        public static float VoidRedChance;

        public override string Name => ":: Items :::::: Voids :: Encrusted Key";
        public override string InternalPickupToken => "treasureCacheVoid";
        public override bool NewPickup => false;
        public override string PickupText => "";
        public override string DescText => "A <style=cIsUtility>hidden cache</style> containing an item (" + d(Mathf.Round(VoidWhiteChance / (VoidWhiteChance + VoidGreenChance + VoidRedChance))) + "/<style=cIsHealing>" + d(Mathf.Round(VoidGreenChance / (VoidWhiteChance + VoidGreenChance + VoidRedChance))) + "</style>/<style=cIsHealth>" + d(Mathf.Round(VoidRedChance / (VoidWhiteChance + VoidGreenChance + VoidRedChance))) + "</style>) will appear in a random location <style=cIsUtility>on each stage</style>. Opening the cache <style=cIsUtility>consumes</style> this item. <style=cIsVoid>Corrupts all Rusted Keys</style>.";

        public override void Init()
        {
            OptionCount = ConfigOption(3, "Items to Choose From", "Vanilla is 3");
            VoidWhiteChance = ConfigOption(5f, "Void White Weight", "Decimal. Vanilla is 5");
            VoidGreenChance = ConfigOption(5f, "Void Green Weight", "Decimal. Vanilla is 5");
            VoidRedChance = ConfigOption(2f, "Void Red Weight", "Decimal. Vanilla is 2");
            base.Init();
        }

        public override void Hooks()
        {
            Changes();
        }

        private void Changes()
        {
            var vlockbox = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/TreasureCacheVoid/LockboxVoid.prefab").WaitForCompletion().GetComponent<OptionChestBehavior>();
            vlockbox.numOptions = OptionCount;

            var vlockDt = Addressables.LoadAssetAsync<BasicPickupDropTable>("RoR2/DLC1/TreasureCacheVoid/dtVoidLockbox.asset").WaitForCompletion();

            vlockDt.voidTier1Weight = VoidWhiteChance;
            vlockDt.voidTier2Weight = VoidGreenChance;
            vlockDt.voidTier3Weight = VoidRedChance;
        }
    }
}