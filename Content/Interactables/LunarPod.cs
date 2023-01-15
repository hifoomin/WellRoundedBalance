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
            On.RoR2.ChestBehavior.ItemDrop += ChestBehavior_ItemDrop;
        }

        private void ChestBehavior_ItemDrop(On.RoR2.ChestBehavior.orig_ItemDrop orig, ChestBehavior self)
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
                var chestBehavior = self.GetComponent<ChestBehavior>();
                if (body)
                {
                    var inventory = body.inventory;
                    var pickupIndex = PickupCatalog.GetPickupDef(chestBehavior.dropPickup);
                    if (inventory)
                    {
                        if (pickupIndex.equipmentIndex != EquipmentIndex.None)
                        {
                            inventory.SetEquipmentIndex(chestBehavior.dropPickup.equipmentIndex);
                            CharacterMasterNotificationQueue.PushEquipmentNotification(body.master, chestBehavior.dropPickup.equipmentIndex);
                        }
                        else
                        {
                            inventory.GiveItem(pickupIndex.itemIndex, 1);
                            CharacterMasterNotificationQueue.PushItemNotification(body.master, chestBehavior.dropPickup.itemIndex);
                            //chestBehavior.HasRolledPickup = true;
                        }

                        self.lastActivator = null;
                    }
                }
            }
        }
    }
}