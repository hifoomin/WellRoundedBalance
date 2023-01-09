using RoR2;
using UnityEngine.AddressableAssets;

namespace WellRoundedBalance.Interactables
{
    public class LunarPod : InteractableBase
    {
        public override string Name => ":: Interactables ::::::: Lunar Pod";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            var lunarPod = Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/Base/LunarChest/iscLunarChest.asset").WaitForCompletion();
            lunarPod.maxSpawnsPerStage = 1;
            lunarPod.directorCreditCost = 15;

            On.RoR2.PurchaseInteraction.OnInteractionBegin += PurchaseInteraction_OnInteractionBegin;
            On.RoR2.ShopTerminalBehavior.DropPickup += ShopTerminalBehavior_DropPickup;
        }

        private void ShopTerminalBehavior_DropPickup(On.RoR2.ShopTerminalBehavior.orig_DropPickup orig, ShopTerminalBehavior self)
        {
            if (NetworkServer.active)
            {
                var purchaseInteraction = self.GetComponent<PurchaseInteraction>();
                if (purchaseInteraction)
                {
                    if (purchaseInteraction.displayNameToken == "LUNAR_CHEST_NAME" && purchaseInteraction.lastActivator == null)
                    {
                        return;
                    }
                }
            }
            orig(self);
        }

        private void PurchaseInteraction_OnInteractionBegin(On.RoR2.PurchaseInteraction.orig_OnInteractionBegin orig, PurchaseInteraction self, Interactor activator)
        {
            orig(self, activator);
            if (self.displayNameToken == "LUNAR_CHEST_NAME")
            {
                var body = activator.GetComponent<CharacterBody>();
                var shopTerminalBehavior = self.GetComponent<ShopTerminalBehavior>();
                if (body)
                {
                    var inventory = body.inventory;
                    var pickupIndex = PickupCatalog.GetPickupDef(shopTerminalBehavior.pickupIndex);
                    if (inventory)
                    {
                        inventory.GiveItem(pickupIndex.itemIndex, 1);
                        shopTerminalBehavior.hasBeenPurchased = true;
                        self.lastActivator = null;
                    }
                }
            }
        }
    }
}