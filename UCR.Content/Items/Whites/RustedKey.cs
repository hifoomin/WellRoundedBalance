using MonoMod.Cil;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace UltimateCustomRun.Items.Whites
{
    public class RustedKey : ItemBase
    {
        public static float WhiteChance;
        public static float GreenChance;
        public static float RedChance;

        public override string Name => ":: Items : Whites :: Rusted Key";
        public override string InternalPickupToken => "treasureCache";
        public override bool NewPickup => false;

        public override string PickupText => "";

        public override string DescText => "A <style=cIsUtility>hidden cache</style> containing an item (" + Mathf.Round(WhiteChance / (WhiteChance + GreenChance + RedChance) * 100) + "%/<style=cIsHealing>" + Mathf.Round(GreenChance / (WhiteChance + GreenChance + RedChance) * 100) + "%</style>/<style=cIsHealth>" + Mathf.Round(RedChance / (WhiteChance + GreenChance + RedChance) * 100) + "%</style>) will appear in a random location <style=cIsUtility>on each stage</style>. Opening the cache <style=cIsUtility>consumes</style> this item.";

        public override void Init()
        {
            WhiteChance = ConfigOption(0f, "White Weight", "Decimal. Vanilla is 0");
            GreenChance = ConfigOption(0.8f, "Green Weight", "Decimal. Vanilla is 0.8");
            RedChance = ConfigOption(0.2f, "Red Weight", "Decimal. Vanilla is 0.2");
            base.Init();
        }

        public override void Hooks()
        {
            Changes();
        }

        private void Changes()
        {
            var lockDt = Addressables.LoadAssetAsync<BasicPickupDropTable>("RoR2/Base/TreasureCache/dtLockbox.asset").WaitForCompletion();
            lockDt.tier1Weight = WhiteChance;
            lockDt.tier2Weight = GreenChance;
            lockDt.tier3Weight = RedChance;

            var lockbox = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/TreasureCache/Lockbox.prefab").WaitForCompletion().GetComponent<ChestBehavior>();
            lockbox.tier1Chance = WhiteChance / (WhiteChance + GreenChance + RedChance);
            lockbox.tier2Chance = GreenChance / (WhiteChance + GreenChance + RedChance);
            lockbox.tier3Chance = RedChance / (WhiteChance + GreenChance + RedChance);
        }
    }
}