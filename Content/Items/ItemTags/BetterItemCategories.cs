using BepInEx.Configuration;
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
                if (def.ContainsTag(ItemTag.Healing) || def.ContainsTag(ItemTag.Utility))
                {
                    List<ItemTag> tags = def.tags.ToList();
                    tags.Remove(ItemTag.Healing);
                    tags.Remove(ItemTag.Utility);
                    def.tags = tags.ToArray();

                    ItemAPI.ApplyTagToItem("Defense", def);
                }
                else
                {
                    Main.WRBLogger.LogError(def.nameToken + " tried to assign Defense category, but doesn't have Healing or Utility tag!");
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

            var queensGland = Utils.Paths.ItemDef.BeetleGland.Load<ItemDef>();
            queensGland.tags = new ItemTag[] { ItemTag.Damage, ItemTag.Utility, ItemTag.CannotCopy };

            var fiftySevenLeafClover = Utils.Paths.ItemDef.Clover.Load<ItemDef>();
            fiftySevenLeafClover.tags = new ItemTag[] { ItemTag.Damage, ItemTag.Utility };

            var runaldsBand = Utils.Paths.ItemDef.IceRing.Load<ItemDef>();
            runaldsBand.tags = new ItemTag[] { ItemTag.Damage, ItemTag.Utility };

            var fuelCell = Utils.Paths.ItemDef.EquipmentMagazine.Load<ItemDef>();
            fuelCell.tags = new ItemTag[] { ItemTag.Damage, ItemTag.Utility, ItemTag.EquipmentRelated };

            var frostRelic = Utils.Paths.ItemDef.Icicle.Load<ItemDef>();
            frostRelic.tags = new ItemTag[] { ItemTag.Damage, ItemTag.Utility, ItemTag.OnKillEffect };

            var brainstalks = Utils.Paths.ItemDef.KillEliteFrenzy.Load<ItemDef>();
            brainstalks.tags = new ItemTag[] { ItemTag.Damage, ItemTag.Utility, ItemTag.AIBlacklist, ItemTag.OnKillEffect };

            var knurl = Utils.Paths.ItemDef.Knurl.Load<ItemDef>();
            knurl.tags = new ItemTag[] { ItemTag.Damage, ItemTag.Utility };

            var purity = Utils.Paths.ItemDef.LunarBadLuck.Load<ItemDef>();
            purity.tags = new ItemTag[] { ItemTag.Damage, ItemTag.Utility };

            var hooksOfHeresy = Utils.Paths.ItemDef.LunarSecondaryReplacement.Load<ItemDef>();
            hooksOfHeresy.tags = new ItemTag[] { ItemTag.Damage, ItemTag.Utility, ItemTag.AIBlacklist, ItemTag.CannotSteal };

            var nkuhanasOpinion = Utils.Paths.ItemDef.NovaOnHeal.Load<ItemDef>();
            nkuhanasOpinion.tags = new ItemTag[] { ItemTag.Damage, ItemTag.AIBlacklist };

            var empathyCores = Utils.Paths.ItemDef.RoboBallBuddy.Load<ItemDef>();
            empathyCores.tags = new ItemTag[] { ItemTag.Damage, ItemTag.Utility, ItemTag.CannotCopy };

            var backupMagazine = Utils.Paths.ItemDef.SecondarySkillMagazine.Load<ItemDef>();
            backupMagazine.tags = new ItemTag[] { ItemTag.Damage, ItemTag.Utility };

            var unstableTeslaCoil = Utils.Paths.ItemDef.ShockNearby.Load<ItemDef>();
            unstableTeslaCoil.tags = new ItemTag[] { ItemTag.Damage, ItemTag.AIBlacklist };

            var soulboundCatalyst = Utils.Paths.ItemDef.Talisman.Load<ItemDef>();
            soulboundCatalyst.tags = new ItemTag[] { ItemTag.Damage, ItemTag.Utility, ItemTag.OnKillEffect, ItemTag.EquipmentRelated };

            var hardlightAfterburner = Utils.Paths.ItemDef.UtilitySkillMagazine.Load<ItemDef>();
            hardlightAfterburner.tags = new ItemTag[] { ItemTag.Damage, ItemTag.Utility };

            var berzerkersPauldron = Utils.Paths.ItemDef.WarCryOnMultiKill.Load<ItemDef>();
            berzerkersPauldron.tags = new ItemTag[] { ItemTag.Damage, ItemTag.Utility, ItemTag.OnKillEffect };

            var warbanner = Utils.Paths.ItemDef.WardOnLevel.Load<ItemDef>();
            warbanner.tags = new ItemTag[] { ItemTag.Damage, ItemTag.Utility, ItemTag.CannotCopy, ItemTag.AIBlacklist };

            var lysateCell = Utils.Paths.ItemDef.EquipmentMagazineVoid.Load<ItemDef>();
            lysateCell.tags = new ItemTag[] { ItemTag.Damage, ItemTag.Utility };

            var delicateWatch = Utils.Paths.ItemDef.FragileDamageBonus.Load<ItemDef>();
            delicateWatch.tags = new ItemTag[] { ItemTag.Damage };

            var egocentrism = Utils.Paths.ItemDef.LunarSun.Load<ItemDef>();
            egocentrism.tags = new ItemTag[] { ItemTag.Damage, ItemTag.AIBlacklist, ItemTag.BrotherBlacklist };

            var bottledChaos = Utils.Paths.ItemDef.RandomEquipmentTrigger.Load<ItemDef>();
            bottledChaos.tags = new ItemTag[] { ItemTag.Damage, ItemTag.Utility, ItemTag.EquipmentRelated };

            var planula = Utils.Paths.ItemDef.ParentEgg.Load<ItemDef>();
            planula.tags = new ItemTag[] { ItemTag.Damage, ItemTag.CannotCopy, ItemTag.BrotherBlacklist };

            var rollofPennies = Utils.Paths.ItemDef.GoldOnHurt.Load<ItemDef>();
            rollofPennies.tags = new ItemTag[] { ItemTag.Utility, ItemTag.CannotDuplicate, ItemTag.OnStageBeginEffect };

            var bisonSteak = Utils.Paths.ItemDef.FlatHealth.Load<ItemDef>();
            bisonSteak.tags = new ItemTag[] { ItemTag.Utility };

            var shuriken = Utils.Paths.ItemDef.PrimarySkillShuriken.Load<ItemDef>();
            shuriken.tags = new ItemTag[] { ItemTag.Damage, ItemTag.AIBlacklist, ItemTag.BrotherBlacklist };

            var defenseTag = ItemAPI.FindItemTagByName("Defense");
            Main.WRBLogger.LogDebug("ItemAPI.FindItemTagByName(\"Defense\") tag returns " + defenseTag);

            // special cases (NREs at any point)

            var irradiantPearl = Utils.Paths.ItemDef.ShinyPearl.Load<ItemDef>();
            irradiantPearl.tags = new ItemTag[] { ItemTag.Damage, ItemTag.Utility };
            ItemAPI.ApplyTagToItem("Defense", irradiantPearl);

            var oldWarStealthkit = Utils.Paths.ItemDef.Phasing.Load<ItemDef>();
            oldWarStealthkit.tags = new ItemTag[] { ItemTag.Utility, ItemTag.LowHealth };
            ItemAPI.ApplyTagToItem("Defense", oldWarStealthkit);

            var miredUrn = Utils.Paths.ItemDef.SiphonOnLowHealth.Load<ItemDef>();
            miredUrn.tags = new ItemTag[] { ItemTag.Damage, ItemTag.Utility, ItemTag.BrotherBlacklist };
            ItemAPI.ApplyTagToItem("Defense", miredUrn);

            var squidPolyp = Utils.Paths.ItemDef.Squid.Load<ItemDef>();
            squidPolyp.tags = new ItemTag[] { ItemTag.Damage, ItemTag.Utility, ItemTag.AIBlacklist, ItemTag.InteractableRelated };
            ItemAPI.ApplyTagToItem("Defense", squidPolyp);

            var halcyonSeed = Utils.Paths.ItemDef.TitanGoldDuringTP.Load<ItemDef>();
            halcyonSeed.tags = new ItemTag[] { ItemTag.Damage, ItemTag.WorldUnique, ItemTag.CannotSteal, ItemTag.CannotCopy, ItemTag.HoldoutZoneRelated };
            ItemAPI.ApplyTagToItem("Defense", halcyonSeed);

            var bensRaincoat = Utils.Paths.ItemDef.ImmuneToDebuff.Load<ItemDef>();
            bensRaincoat.tags = new ItemTag[] { ItemTag.Utility };
            ItemAPI.ApplyTagToItem("Defense", bensRaincoat);

            var defenseNucleus = Utils.Paths.ItemDef.MinorConstructOnKill.Load<ItemDef>();
            defenseNucleus.tags = new ItemTag[] { ItemTag.Damage };
            ItemAPI.ApplyTagToItem("Defense", defenseNucleus);

            var plasmaShrimp = Utils.Paths.ItemDef.MissileVoid.Load<ItemDef>();
            ItemAPI.ApplyTagToItem("Defense", plasmaShrimp);

            var symbioticScorpion = Utils.Paths.ItemDef.PermanentDebuffOnHit.Load<ItemDef>();
            symbioticScorpion.tags = new ItemTag[] { ItemTag.Damage, ItemTag.AIBlacklist, ItemTag.BrotherBlacklist };
            ItemAPI.ApplyTagToItem("Defense", symbioticScorpion);

            var newlyHatchedZoea = Utils.Paths.ItemDef.VoidMegaCrabItem.Load<ItemDef>();
            newlyHatchedZoea.tags = new ItemTag[] { ItemTag.Damage };
            ItemAPI.ApplyTagToItem("Defense", newlyHatchedZoea);

            var warHorn = Utils.Paths.ItemDef.EnergizedOnEquipmentUse.Load<ItemDef>();
            ItemAPI.ApplyTagToItem("Defense", warHorn);

            // removals and defense additions

            ReplaceWithDefense("ArmorPlate"); // Repulsion Armor Plate
            ReplaceWithDefense("BarrierOnKill"); // Topaz Brooch
            ReplaceWithDefense("BarrierOnOverHeal"); // Aegis
            ReplaceWithDefense("Bear"); // Tougher Times
            ReplaceWithDefense("BeetleGland"); // Queen's Gland
            ReplaceWithDefense("CaptainDefenseMatrix"); // Defensive Microbots
            ReplaceWithDefense("ExtraLife"); // Dio's Best Friend
            ReplaceWithDefense("FlatHealth"); // Bison Steak
            ReplaceWithDefense("GhostOnKill"); // Happiest Mask
            ReplaceWithDefense("HeadHunter"); // Wake of Vultures
            ReplaceWithDefense("HealOnCrit"); // Harvester's Scythe
            ReplaceWithDefense("HealWhileSafe"); // Cautious Slug
            ReplaceWithDefense("IncreaseHealing"); // Rejuvenation Rack
            ReplaceWithDefense("Infusion");
            ReplaceWithDefense("Medkit");
            ReplaceWithDefense("Mushroom"); // Bustling Fungus
            ReplaceWithDefense("Pearl");
            ReplaceWithDefense("PersonalShield"); // Personal Shield Generator
            ReplaceWithDefense("Plant"); // Interstellar Desk Plant
            ReplaceWithDefense("RepeatHeal"); // Corpsebloom
            ReplaceWithDefense("RoboBallBuddy"); // Empathy Cores
            ReplaceWithDefense("Seed"); // Leeching Seed
            ReplaceWithDefense("ShieldOnly"); // Transcendence
            ReplaceWithDefense("SprintArmor"); // Rose Buckler
            ReplaceWithDefense("Tooth"); // Monster Tooth
            ReplaceWithDefense("TPHealingNova"); // Lepton Daisy
            ReplaceWithDefense("BearVoid"); // Safer Spaces
            ReplaceWithDefense("ExtraLifeVoid"); // Pluripotent Larva
            ReplaceWithDefense("HealingPotion"); // Power Elixir
            ReplaceWithDefense("MushroomVoid"); // Weeping Fungus
            ReplaceWithDefense("OutOfCombatArmor"); // Oddly-shaped Opal

            // category chest changes
            var smallHealingChestDropTable = Utils.Paths.BasicPickupDropTable.dtSmallChestHealing.Load<BasicPickupDropTable>();
            smallHealingChestDropTable.requiredItemTags = new ItemTag[] { ItemAPI.FindItemTagByName("Defense") };
            LanguageAPI.Add("CATEGORYCHEST_HEALING_NAME", "Chest - Defense");
            LanguageAPI.Add("CATEGORYCHEST_HEALING_CONTEXT", "Open Chest - Defense");

            var largeHealingChestDropTable = Utils.Paths.BasicPickupDropTable.dtCategoryChest2Healing.Load<BasicPickupDropTable>();
            largeHealingChestDropTable.requiredItemTags = new ItemTag[] { ItemAPI.FindItemTagByName("Defense") };
            LanguageAPI.Add("CATEGORYCHEST2_HEALING_NAME", "Large Chest - Defense");
            LanguageAPI.Add("CATEGORYCHEST2_HEALING_CONTEXT", "Open Large Chest - Defense");
        }

        public static void BetterAIBlacklist()
        {
            foreach (ItemDef itemDef in ItemCatalog.itemDefs)
            {
                if ((itemDef.tags.Contains(ItemTag.OnKillEffect) || itemDef.tags.Contains(ItemTag.InteractableRelated) || itemDef.tags.Contains(ItemTag.SprintRelated) || itemDef.tags.Contains(ItemTag.EquipmentRelated) || itemDef.tags.Contains(ItemTag.HoldoutZoneRelated) || itemDef.tags.Contains(ItemTag.OnStageBeginEffect)) && !itemDef.tags.Contains(ItemTag.AIBlacklist))
                {
                    List<ItemTag> tags = itemDef.tags.ToList();
                    tags.Add(ItemTag.AIBlacklist);
                    itemDef.tags = tags.ToArray();
                    // Main.WRBLogger.LogError("Added AI Blacklist to " + Language.GetString(itemDef.nameToken));
                }
            }
        }
    }
}