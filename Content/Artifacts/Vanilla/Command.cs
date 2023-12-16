using R2API.Utils;
using RoR2.Artifacts;

namespace WellRoundedBalance.Artifacts.Vanilla
{
    internal class Command : ArtifactEditBase<Command>
    {
        public override string Name => ":: Artifacts :: Command";

        [ConfigField("Affect Chests?", "", true)]
        public static bool affectChests;

        [ConfigField("Affect Shrines of Chance?", "", true)]
        public static bool affectShrines;

        [ConfigField("Affect Teleporter Boss Items?", "", true)]
        public static bool affectBosses;

        [ConfigField("Affect Sacrifice?", "", true)]
        public static bool affectSacrifice;

        [ConfigField("Potential Replacement Chance", "Decimal.", 1f)]
        public static float replaceChance;

        [ConfigField("White Potential Item Count", "Set to 1 to disable", 3)]
        public static int whitePotential;

        [ConfigField("Green Potential Item Count", "Set to 1 to disable", 3)]
        public static int greenPotential;

        [ConfigField("Red Potential Item Count", "Set to 1 to disable", 2)]
        public static int redPotential;

        [ConfigField("Yellow Potential Item Count", "Set to 1 to disable", 1)]
        public static int yellowPotential;

        [ConfigField("Lunar Potential Item Count", "Set to 1 to disable", 3)]
        public static int lunarPotential;

        [ConfigField("Void White Potential Item Count", "Set to 1 to disable", 3)]
        public static int voidWhitePotential;

        [ConfigField("Void Green Potential Item Count", "Set to 1 to disable", 3)]
        public static int voidGreenPotential;

        [ConfigField("Void Red Potential Item Count", "Set to 1 to disable", 2)]
        public static int voidRedPotential;

        [ConfigField("Void Yellow Potential Item Count", "Set to 1 to disable", 1)]
        public static int voidYellowPotential;

        [ConfigField("Equipment Potential Item Count", "Set to 1 to disable", 1)]
        public static int equipmentPotential;

        public static GameObject voidPotentialPrefab = null;
        public static GameObject commandCubePrefab = null;
        public static Xoroshiro128Plus rng = null;
        public static PickupDropTable dropTable = null;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            voidPotentialPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/OptionPickup/OptionPickup.prefab").WaitForCompletion();
            commandCubePrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Command/CommandCube.prefab").WaitForCompletion();

            IL.RoR2.Artifacts.CommandArtifactManager.Init += CommandArtifactManager_Init;
            RunArtifactManager.onArtifactEnabledGlobal += CommandEnabled;
            RunArtifactManager.onArtifactDisabledGlobal += CommandDisabled;
        }

        private void CommandDisabled([JetBrains.Annotations.NotNull] RunArtifactManager runArtifactManager, [JetBrains.Annotations.NotNull] ArtifactDef artifactDef)
        {
            if (artifactDef != RoR2Content.Artifacts.commandArtifactDef)
            {
                return;
            }
            On.RoR2.ChestBehavior.ItemDrop -= ChestBehavior_ItemDrop;
            On.RoR2.ShrineChanceBehavior.AddShrineStack -= ShrineChanceBehavior_AddShrineStack;
            On.RoR2.BossGroup.DropRewards -= BossGroup_DropRewards;
            On.RoR2.Artifacts.SacrificeArtifactManager.OnServerCharacterDeath -= SacrificeArtifactManager_OnServerCharacterDeath;
        }

        private static void ChestBehavior_ItemDrop(On.RoR2.ChestBehavior.orig_ItemDrop orig, ChestBehavior self)
        {
            if (Run.instance.treasureRng.RangeFloat(0f, 1f) <= replaceChance)
            {
                rng = self.GetFieldValue<Xoroshiro128Plus>("rng");
                dropTable = self.dropTable;
                On.RoR2.PickupDropletController.CreatePickupDroplet_PickupIndex_Vector3_Vector3 += CreatePickupDroplet_BasicPickupDropTable;
                orig(self);
                rng = null;
                dropTable = null;
                On.RoR2.PickupDropletController.CreatePickupDroplet_PickupIndex_Vector3_Vector3 -= CreatePickupDroplet_BasicPickupDropTable;
            }
            else
            {
                orig(self);
            }
        }

        private static void ShrineChanceBehavior_AddShrineStack(On.RoR2.ShrineChanceBehavior.orig_AddShrineStack orig, ShrineChanceBehavior self, Interactor activator)
        {
            if (Run.instance.treasureRng.RangeFloat(0f, 1f) <= replaceChance)
            {
                rng = self.GetFieldValue<Xoroshiro128Plus>("rng");
                dropTable = self.dropTable;
                On.RoR2.PickupDropletController.CreatePickupDroplet_PickupIndex_Vector3_Vector3 += CreatePickupDroplet_BasicPickupDropTable;
                orig(self, activator);
                rng = null;
                dropTable = null;
                On.RoR2.PickupDropletController.CreatePickupDroplet_PickupIndex_Vector3_Vector3 -= CreatePickupDroplet_BasicPickupDropTable;
            }
            else
            {
                orig(self, activator);
            }
        }

        private static void SacrificeArtifactManager_OnServerCharacterDeath(On.RoR2.Artifacts.SacrificeArtifactManager.orig_OnServerCharacterDeath orig, DamageReport damageReport)
        {
            if (Run.instance.treasureRng.RangeFloat(0f, 1f) <= replaceChance)
            {
                rng = typeof(SacrificeArtifactManager).GetFieldValue<Xoroshiro128Plus>("treasureRng");
                dropTable = typeof(SacrificeArtifactManager).GetFieldValue<PickupDropTable>("dropTable");
                On.RoR2.PickupDropletController.CreatePickupDroplet_PickupIndex_Vector3_Vector3 += CreatePickupDroplet_BasicPickupDropTable;
                orig(damageReport);
                rng = null;
                dropTable = null;
                On.RoR2.PickupDropletController.CreatePickupDroplet_PickupIndex_Vector3_Vector3 -= CreatePickupDroplet_BasicPickupDropTable;
            }
            else
            {
                orig(damageReport);
            }
        }

        private static void CreatePickupDroplet_BasicPickupDropTable(On.RoR2.PickupDropletController.orig_CreatePickupDroplet_PickupIndex_Vector3_Vector3 orig, PickupIndex pickupIndex, Vector3 position, Vector3 velocity)
        {
            if (dropTable == null)
            {
                rng = null;
                On.RoR2.PickupDropletController.CreatePickupDroplet_PickupIndex_Vector3_Vector3 -= CreatePickupDroplet_BasicPickupDropTable;
                orig(pickupIndex, position, velocity);
                return;
            }
            int tier = GetTier(pickupIndex);
            PickupIndex[] choices = null;
            PickupIndex[] choices2 = null;
            int num = 0;
            if (tier == 6) // lunar
            {
                choices2 = GetUniqueItemsOfSameTier(GetChoiceCountByTier(tier) - 1, pickupIndex, rng);
                num = choices2.Length;
                if (num == 0)
                {
                    orig(pickupIndex, position, velocity);
                    return;
                }
                choices = new PickupIndex[num + 1];
            }
            else // not lunar
            {
                dropTable.canDropBeReplaced = false;
                WeightedSelection<PickupIndex> selection = ((BasicPickupDropTable)dropTable).GetFieldValue<WeightedSelection<PickupIndex>>("selector");
                for (int i = 0; i < selection.Count; i++)
                {
                    if (GetTier(selection.GetChoice(i).value) != tier || selection.GetChoice(i).value == pickupIndex)
                    {
                        selection.RemoveChoice(i);
                        i--;
                    }
                }
                num = Mathf.Min(GetChoiceCountByTier(tier) - 1, selection.Count);
                if (num == 0)
                {
                    orig(pickupIndex, position, velocity);
                    return;
                }
                choices = new PickupIndex[num + 1];
                choices2 = dropTable.GenerateUniqueDrops(num, rng);
                dropTable.canDropBeReplaced = true;
                dropTable.InvokeMethod("Regenerate", Run.instance);
            }

            choices[0] = pickupIndex;
            for (int i = 0; i < num; i++)
            {
                choices[i + 1] = choices2[i];
            }
            GenericPickupController.CreatePickupInfo pickupInfo = new GenericPickupController.CreatePickupInfo
            {
                pickerOptions = PickupPickerController.GenerateOptionsFromArray(choices),
                position = position,
                rotation = Quaternion.identity,
                prefabOverride = (choices.Length > 3) ? commandCubePrefab : voidPotentialPrefab,
                pickupIndex = pickupIndex
            };
            PickupDropletController.CreatePickupDroplet(pickupInfo, position, velocity);
        }

        private static void BossGroup_DropRewards(On.RoR2.BossGroup.orig_DropRewards orig, BossGroup self)
        {
            if (Run.instance.treasureRng.RangeFloat(0f, 1f) <= replaceChance)
            {
                rng = self.GetFieldValue<Xoroshiro128Plus>("rng");
                On.RoR2.PickupDropletController.CreatePickupDroplet_PickupIndex_Vector3_Vector3 += CreatePickupDroplet_BossDropTable;
                orig(self);
                rng = null;
                On.RoR2.PickupDropletController.CreatePickupDroplet_PickupIndex_Vector3_Vector3 -= CreatePickupDroplet_BossDropTable;
                for (int i = 0; i < bossDropsByTier.Length; i++)
                {
                    bossDropsByTier[i] = null;
                }
            }
            else
            {
                orig(self);
            }
        }

        public static PickupIndex[][] bossDropsByTier = new PickupIndex[10][];

        private static void CreatePickupDroplet_BossDropTable(On.RoR2.PickupDropletController.orig_CreatePickupDroplet_PickupIndex_Vector3_Vector3 orig, PickupIndex pickupIndex, Vector3 position, Vector3 velocity)
        {
            int tier = GetTier(pickupIndex);
            PickupIndex[] choices = null;
            int num = 0;

            if (tier == 6) // lunar
            {
                PickupIndex[] bossDropsLunar = GetUniqueItemsOfSameTier(GetChoiceCountByTier(tier) - 1, pickupIndex, rng);
                if (bossDropsLunar == null || bossDropsLunar.Length == 0)
                {
                    orig(pickupIndex, position, velocity);
                    return;
                }
                num = bossDropsLunar.Length;
                choices = new PickupIndex[num + 1];
                choices[0] = pickupIndex;
                for (int i = 0; i < num; i++)
                {
                    choices[i + 1] = bossDropsLunar[i];
                }
            }
            else // not lunar
            {
                if (bossDropsByTier[tier - 1] == null)
                {
                    bossDropsByTier[tier - 1] = GetUniqueItemsOfSameTier(GetChoiceCountByTier(tier) - 1, pickupIndex, rng);
                }
                if (bossDropsByTier[tier - 1] == null || bossDropsByTier[tier - 1].Length == 0)
                {
                    orig(pickupIndex, position, velocity);
                    return;
                }
                num = bossDropsByTier[tier - 1].Length;
                choices = new PickupIndex[num + 1];
                choices[0] = pickupIndex;
                for (int i = 0; i < num; i++)
                {
                    choices[i + 1] = bossDropsByTier[tier - 1][i];
                }
            }

            GenericPickupController.CreatePickupInfo pickupInfo = new GenericPickupController.CreatePickupInfo
            {
                pickerOptions = PickupPickerController.GenerateOptionsFromArray(choices),
                position = position,
                rotation = Quaternion.identity,
                prefabOverride = (choices.Length > 3) ? commandCubePrefab : voidPotentialPrefab,
                pickupIndex = pickupIndex
            };
            PickupDropletController.CreatePickupDroplet(pickupInfo, position, velocity);
        }

        private static int GetTier(PickupIndex pickupIndex)
        {
            switch (PickupCatalog.GetPickupDef(pickupIndex).itemTier)
            {
                case ItemTier.Tier1:
                    return 1;

                case ItemTier.Tier2:
                    return 2;

                case ItemTier.Tier3:
                    return 3;

                case ItemTier.Boss:
                    return 5;

                case ItemTier.Lunar:
                    return 6;

                case ItemTier.VoidTier1:
                    return 7;

                case ItemTier.VoidTier2:
                    return 8;

                case ItemTier.VoidTier3:
                    return 9;

                case ItemTier.VoidBoss:
                    return 10;

                default:
                    if (PickupCatalog.GetPickupDef(pickupIndex).isLunar)
                    {
                        return 6;
                    }
                    return 4;
            }
        }

        private static PickupIndex[] GetUniqueItemsOfSameTier(int num, PickupIndex pickupIndex, Xoroshiro128Plus rng)
        {
            List<PickupIndex> list = null;
            switch (GetTier(pickupIndex))
            {
                case 1:
                    list = Run.instance.availableTier1DropList;
                    break;

                case 2:
                    list = Run.instance.availableTier2DropList;
                    break;

                case 3:
                    list = Run.instance.availableTier3DropList;
                    break;

                case 4:
                    list = Run.instance.availableEquipmentDropList;
                    break;

                case 5:
                    list = Run.instance.availableBossDropList;
                    break;

                case 6:
                    list = Run.instance.availableLunarCombinedDropList;
                    break;

                case 7:
                    list = Run.instance.availableVoidTier1DropList;
                    break;

                case 8:
                    list = Run.instance.availableVoidTier2DropList;
                    break;

                case 9:
                    list = Run.instance.availableVoidTier3DropList;
                    break;

                case 10:
                    list = Run.instance.availableVoidBossDropList;
                    break;
            }
            WeightedSelection<PickupIndex> selection = new(8);
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] != pickupIndex)
                    selection.AddChoice(list[i], 1f);
            }
            return typeof(PickupDropTable).InvokeMethod<PickupIndex[]>("GenerateUniqueDropsFromWeightedSelection", num, rng, selection);
        }

        private void CommandEnabled([JetBrains.Annotations.NotNull] RunArtifactManager runArtifactManager, [JetBrains.Annotations.NotNull] ArtifactDef artifactDef)
        {
            if (artifactDef != RoR2Content.Artifacts.commandArtifactDef)
            {
                return;
            }
            if (NetworkServer.active)
            {
                if (affectChests)
                    On.RoR2.ChestBehavior.ItemDrop += ChestBehavior_ItemDrop;
                if (affectShrines)
                    On.RoR2.ShrineChanceBehavior.AddShrineStack += ShrineChanceBehavior_AddShrineStack;
                if (affectBosses)
                    On.RoR2.BossGroup.DropRewards += BossGroup_DropRewards;
                if (affectSacrifice)
                    On.RoR2.Artifacts.SacrificeArtifactManager.OnServerCharacterDeath += SacrificeArtifactManager_OnServerCharacterDeath;
            }
        }

        private void CommandArtifactManager_Init(ILContext il)
        {
            ILCursor c = new(il);
            if (c.TryGotoNext(MoveType.After,
                x => x.MatchStsfld("RoR2.Artifacts.CommandArtifactManager", "commandCubePrefab")))
            {
                c.Emit(OpCodes.Ret);
            }
            else
            {
                Logger.LogError("Failed to apply Command Deletion hook");
            }
        }

        public static int GetChoiceCountByTier(int tier)
        {
            return tier switch
            {
                1 => whitePotential,
                2 => greenPotential,
                3 => redPotential,
                4 => equipmentPotential,
                5 => yellowPotential,
                6 => lunarPotential,
                7 => voidWhitePotential,
                8 => voidGreenPotential,
                9 => voidRedPotential,
                10 => voidYellowPotential,
                _ => 1,
            };
        }
    }
}