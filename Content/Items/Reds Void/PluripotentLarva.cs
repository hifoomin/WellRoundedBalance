using System.Collections;
using System.Collections.Generic;

namespace WellRoundedBalance.Items.Reds
{
    public class PluripotentLarva : ItemBase<PluripotentLarva>
    {
        public override string Name => ":: Items :::::: Voids :: Pluripotent Larva";
        public override ItemDef InternalPickup => DLC1Content.Items.ExtraLifeVoid;

        public override string PickupText => "Shuffle your inventory, and get a <style=cIsVoid>corrupted</style> extra life. Consumed on use. <style=cIsVoid>Corrupts all Dio's Best Friends.</style>.";

        public override string DescText => "<style=cIsUtility>Shuffle your inventory</style>. <style=cIsUtility>Upon death</style>, this item will be <style=cIsUtility>consumed</style> and you will <style=cIsHealing>return to life</style> with <style=cIsHealing>3 seconds of invulnerability</style>, and all of your items that can be <style=cIsUtility>corrupted</style> will be. <style=cIsVoid>Corrupts all Dio's Best Friends</style>.";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.RoR2.Inventory.GiveItem_ItemIndex_int += Inventory_GiveItem_ItemIndex_int;
        }

        private void Inventory_GiveItem_ItemIndex_int(On.RoR2.Inventory.orig_GiveItem_ItemIndex_int orig, Inventory self, ItemIndex itemIndex, int count)
        {
            orig(self, itemIndex, count);
            if (itemIndex == DLC1Content.Items.ExtraLifeVoid.itemIndex)
            {
                List<ItemIndex> tier1Indices = new();
                List<int> stacks = new();

                // Store the indices and stack counts of all Tier 1 items in the inventory
                for (int i = 0; i < self.itemAcquisitionOrder.Count; i++)
                {
                    var index = self.itemAcquisitionOrder[i];
                    var itemDef = ItemCatalog.GetItemDef(index);

                    if (itemDef.tier == ItemTier.Tier1 || itemDef.deprecatedTier == ItemTier.Tier1)
                    {
                        tier1Indices.Add(index);
                        stacks.Add(self.GetItemCount(index));
                    }
                }

                // Shuffle the stack counts using Fisher-Yates shuffle algorithm
                int n = stacks.Count;
                while (n > 1)
                {
                    n--;
                    int k = Random.Range(0, n + 1);
                    int temp = stacks[k];
                    stacks[k] = stacks[n];
                    stacks[n] = temp;
                }

                // Assign the shuffled stack counts to the Tier 1 items
                for (int i = 0; i < tier1Indices.Count; i++)
                {
                    var index = tier1Indices[i];
                    var stackCount = stacks[i];
                    self.RemoveItem(index, self.GetItemCount(index));
                    self.GiveItem(index, stackCount);
                }

                Chat.SendBroadcastChat(new Chat.SimpleChatMessage { baseToken = "<style=cWorldEvent>You have been... corrupted.</color>" });
            }
        }
    }
}