using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using R2API.Utils;
using RoR2;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace UltimateCustomRun.Items.Greens
{
    public class ShippingRequestForm : ItemBase
    {
        public static float WhiteChance;
        public static float GreenChance;
        public static float RedChance;

        public override string Name => ":: Items :: Greens :: Shipping Request Form";
        public override string InternalPickupToken => "freeChest";
        public override bool NewPickup => false;
        public override string PickupText => "";

        public override string DescText => "A <style=cIsUtility>delivery</style> containing 2 items (" + Mathf.Round(WhiteChance / (WhiteChance + GreenChance + RedChance) * 100) + "%/<style=cIsHealing>" + Mathf.Round(GreenChance / (WhiteChance + GreenChance + RedChance) * 100) + "%</style>/<style=cIsHealth>" + Mathf.Round(RedChance / (WhiteChance + GreenChance + RedChance) * 100) + "%</style>) will appear in a random location <style=cIsUtility>on each stage</style>. <style=cStack>(Increases rarity chances of the items per stack).</style>";

        public override void Init()
        {
            WhiteChance = ConfigOption(0.79f, "White Weight", "Decimal. Vanilla is 0.79");
            GreenChance = ConfigOption(0.2f, "Green Weight", "Decimal. Vanilla is 0.2");
            RedChance = ConfigOption(0.01f, "Red Weight", "Decimal. Vanilla is 0.01");
            base.Init();
        }

        public override void Hooks()
        {
            Changes();
        }

        private void Changes()
        {
            var shipDt = Addressables.LoadAssetAsync<FreeChestDropTable>("RoR2/DLC1/FreeChest/dtFreeChest.asset").WaitForCompletion();
            shipDt.tier1Weight = WhiteChance;
            shipDt.tier2Weight = GreenChance;
            shipDt.tier3Weight = RedChance;
        }
    }
}