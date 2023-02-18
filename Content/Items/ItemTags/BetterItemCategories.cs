using BepInEx.Configuration;
using System;

namespace WellRoundedBalance.Items.ConsistentCategories
{
    public static class BetterItemCategories
    {
        /* Makes item categories more consistent and differentiates them a bit more, mainly:
           - Attack speed items being damage
           - Cooldown reduction being both damage and utility
           - Ally items being both damage and defense
           - Non-damaging status effects being utility, etc
           - Defense category is defensive vanilla items tagged as Utility and or Healing
        */

        // also changes AI blacklist to accomodate for item changes

        public static ConfigEntry<bool> enable { get; set; }

        private void ReplaceWithDefense(string itemName)
        {
            var item = Utils.Paths.ItemDef.itemName.Load<ItemDef>();
            for (int i = 0; i < item.tags.Length; i++)
            {
                var itemTag = item.tags[i];
                if (itemTag == ItemTag.Utility || itemTag == ItemTag.Healing)
                {
                    // uhh remove
                }
            }
        }

        public static void Init()
        {
            // general changes
            var alienHead = Utils.Paths.ItemDef.AlienHead.Load<ItemDef>();
            alienHead.tags = new ItemTag[] { ItemTag.Damage, ItemTag.Utility };

            var gestureOfTheDrowned = Utils.Paths.ItemDef.AutoCastEquipment.Load<ItemDef>();
            gestureOfTheDrowned.tags = new ItemTag[] { ItemTag.Damage, ItemTag.Utility, ItemTag.EquipmentRelated };

            var bandolier = Utils.Paths.ItemDef.Bandolier.Load<ItemDef>();
            bandolier.tags = new ItemTag[] { ItemTag.Utility, ItemTag.OnKillEffect };

            // removals and defense additions

            var repulsionArmorPlate = Utils.Paths.ItemDef.ArmorPlate.Load<ItemDef>();
            repulsionArmorPlate.tags = new ItemTag[] { };
            ItemAPI.ApplyTagToItem("Defense", repulsionArmorPlate);

            var topazBrooch = Utils.Paths.ItemDef.BarrierOnKill.Load<ItemDef>();
            topazBrooch.tags = new ItemTag[] { ItemTag.OnKillEffect };
            ItemAPI.ApplyTagToItem("Defense", topazBrooch);

            var aegis = Utils.Paths.ItemDef.BarrierOnOverHeal.Load<ItemDef>();
            aegis.tags = new ItemTag[] { };
            ItemAPI.ApplyTagToItem("Defense", aegis);

            var tougherTimes = Utils.Paths.ItemDef.Bear.Load<ItemDef>();
            tougherTimes.tags = new ItemTag[] { ItemTag.BrotherBlacklist };
            ItemAPI.ApplyTagToItem("Defense", tougherTimes);
        }
    }
}