using BepInEx.Configuration;
using System;
using static Mono.Security.X509.X520;
using System.Reflection;

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

        private static void ReplaceWithDefense(string itemName)
        {
            FieldInfo[] fields = typeof(Utils.Paths.ItemDef).GetFields();
            FieldInfo field = fields.FirstOrDefault(x => (x.GetValue(null) as string).Contains(itemName));
            if (!field.Equals(default(FieldInfo)))
            {
                ItemDef def = (field.GetValue(null) as string).Load<ItemDef>();
                if (def.ContainsTag(ItemTag.Healing) || (def.ContainsTag(ItemTag.Healing) && def.ContainsTag(ItemTag.Utility)))
                {
                    List<ItemTag> tags = def.tags.ToList();
                    tags.Remove(ItemTag.Healing);
                    tags.Remove(ItemTag.Utility);
                    def.tags = tags.ToArray();

                    ItemAPI.ApplyTagToItem("Defense", def);
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
            // repulsionArmorPlate.tags = new ItemTag[] { };
            // ItemAPI.ApplyTagToItem("Defense", repulsionArmorPlate);
            ReplaceWithDefense("ArmorPlate");

            Main.WRBLogger.LogFatal("repulsion armor plate has " + repulsionArmorPlate.tags.Length + " tags");
            /*
            var topazBrooch = Utils.Paths.ItemDef.BarrierOnKill.Load<ItemDef>();
            topazBrooch.tags = new ItemTag[] { ItemTag.OnKillEffect };
            ItemAPI.ApplyTagToItem("Defense", topazBrooch);

            var aegis = Utils.Paths.ItemDef.BarrierOnOverHeal.Load<ItemDef>();
            aegis.tags = new ItemTag[] { };
            ItemAPI.ApplyTagToItem("Defense", aegis);

            var tougherTimes = Utils.Paths.ItemDef.Bear.Load<ItemDef>();
            tougherTimes.tags = new ItemTag[] { ItemTag.BrotherBlacklist };
            ItemAPI.ApplyTagToItem("Defense", tougherTimes);
            */
        }
    }
}