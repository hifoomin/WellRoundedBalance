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
            Inventory.onServerItemGiven += Inventory_onServerItemGiven;
        }

        private void Inventory_onServerItemGiven(Inventory inventory, ItemIndex itemIndex, int count)
        {
            if (itemIndex != DLC1Content.Items.ExtraLifeVoid.itemIndex) return;
            /*
            List<ItemIndex> tier1Indices = new();

            for (int i = 0; i < inventory.itemAcquisitionOrder.Count; i++)
            {
                var index = inventory.itemAcquisitionOrder[i];
                var itemDef = ItemCatalog.GetItemDef(index);

                if (itemDef.tier == ItemTier.Tier1 || itemDef.deprecatedTier == ItemTier.Tier1)
                {
                    tier1Indices.Add(index);
                }
            }
            for (int i = 0; i < tier1Indices.Count; i++)
            {
                var index = tier1Indices[i];
                index = Run.instance.stageRng.RangeInt(tier1Indices, tier1Indices.Count);
            }
            // what am i doing uhhh this is dumber than i thought
            Chat.SendBroadcastChat(new Chat.SimpleChatMessage { baseToken = "<style=cWorldEvent>You have been... corrupted.</color>" });
            */

            int[] idx = { };
            int[] value = { };
            for (var i = 0; i < inventory.itemStacks.Length; i++) if (inventory.itemStacks[i] > 0 && ItemCatalog.GetItemDef((ItemIndex)i).tier == ItemTier.Tier1)
                {
                    HG.ArrayUtils.ArrayAppend(ref idx, i);
                    HG.ArrayUtils.ArrayAppend(ref value, inventory.itemStacks[i]);
                }
            idx = idx.OrderBy(x => Run.instance.runRNG.Next()).ToArray();
            for (var i = 0; i < idx.Length; i++) inventory.itemStacks[idx[i]] = value[i];
            if (LocalUserManager.GetFirstLocalUser()?.cachedBody?.inventory == inventory) Chat.SendBroadcastChat(new Chat.SimpleChatMessage { baseToken = "<style=cWorldEvent>You have been... corrupted.</color>" });
        }
    }
}