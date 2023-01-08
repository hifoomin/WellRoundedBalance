using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using R2API.Utils;
using RoR2;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace WellRoundedBalance.Items.Greens
{
    public class ShippingRequestForm : ItemBase
    {
        public override string Name => ":: Items :: Greens :: Shipping Request Form";
        public override string InternalPickupToken => "freeChest";

        public override string PickupText => "Get a delivery each stage that contains powerful items.";

        public override string DescText => "A <style=cIsUtility>delivery</style> containing 2 items (65%/<style=cIsHealing>33%</style>/<style=cIsHealth>2%</style>) will appear in a random location <style=cIsUtility>on each stage</style>. <style=cStack>(Increases rarity chances of the items per stack).</style>";

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
            shipDt.tier1Weight = 0.65f;
            shipDt.tier2Weight = 0.33f;
            shipDt.tier3Weight = 0.02f;
        }
    }
}