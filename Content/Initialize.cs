using BepInEx.Configuration;
using System;
using System.Diagnostics;
using System.Reflection;
using WellRoundedBalance.Achievements;
using WellRoundedBalance.Allies;
using WellRoundedBalance.Artifacts.New;
using WellRoundedBalance.Artifacts.Vanilla;
using WellRoundedBalance.Difficulties;
using WellRoundedBalance.Elites;
using WellRoundedBalance.Enemies;
using WellRoundedBalance.Equipment;
using WellRoundedBalance.Gamemodes;
using WellRoundedBalance.Interactables;
using WellRoundedBalance.Items;
using WellRoundedBalance.Items.ConsistentCategories;
using WellRoundedBalance.Items.NoTier;
using WellRoundedBalance.Mechanics;
using WellRoundedBalance.Misc;
using WellRoundedBalance.Projectiles;
using WellRoundedBalance.Survivors;

namespace WellRoundedBalance
{
    public static class Initialize
    {
        public static void Init()
        {
            var stopwatch = Stopwatch.StartNew();

            FunnyLabel.Hooks();
            // Useless.Create();
            Buffs.Useless.Create();
            VoidBall.Create();
            BlazingProjectileVFX.Create();
            Molotov.Create();
            MolotovBig.Create();
            DucleusLaser.Create();
            TitanFist.Create();
            EarthQuakeWave.Create();

            On.RoR2.ItemCatalog.Init += ItemCatalog_Init;

            if (Main.enableAchievements.Value)
            {
                IEnumerable<Type> achievement = Assembly.GetExecutingAssembly().GetTypes()
                                                .Where(type => !type.IsAbstract && type.IsSubclassOf(typeof(AchievementBase)));

                Main.WRBLogger.LogInfo("==+----------------==ACHIEVEMENTS==----------------+==");

                foreach (Type type in achievement)
                {
                    AchievementBase based = (AchievementBase)Activator.CreateInstance(type);
                    if (Validate(based))
                    {
                        try { based.Init(); } catch (Exception ex) { Main.WRBLogger.LogError($"Failed to initialize {type.Name}: {ex}"); }
                    }
                }
            }

            if (Main.enableAllies.Value)
            {
                IEnumerable<Type> ally = Assembly.GetExecutingAssembly().GetTypes()
                                                .Where(type => !type.IsAbstract && type.IsSubclassOf(typeof(AllyBase)));

                Main.WRBLogger.LogInfo("==+----------------==ALLIES==----------------+==");

                foreach (Type type in ally)
                {
                    AllyBase based = (AllyBase)Activator.CreateInstance(type);
                    if (Validate(based))
                    {
                        try { based.Init(); } catch (Exception ex) { Main.WRBLogger.LogError($"Failed to initialize {type.Name}: {ex}"); }
                    }
                }
            }

            if (Main.enableArtifactAdds.Value)
            {
                /*
                IEnumerable<Type> artifactAdd = Assembly.GetExecutingAssembly().GetTypes()
                                                .Where(type => !type.IsAbstract && type.IsSubclassOf(typeof(ArtifactAddBase)));

                Main.WRBLogger.LogInfo("==+----------------==ARTIFACT ADDS==----------------+==");

                foreach (Type type in artifactAdd)
                {
                    ArtifactAddBase based = (ArtifactAddBase)Activator.CreateInstance(type);
                    if (ValidateArtifactAdd(based))
                    {
                        based.Init();
                        // disabled until icon is done
                    }
                }
                */
            }

            if (Main.enableArtifactEdits.Value)
            {
                IEnumerable<Type> artifactEdit = Assembly.GetExecutingAssembly().GetTypes()
                                                .Where(type => !type.IsAbstract && type.IsSubclassOf(typeof(ArtifactEditBase)));

                Main.WRBLogger.LogInfo("==+----------------==ARTIFACT EDITS==----------------+==");

                foreach (Type type in artifactEdit)
                {
                    ArtifactEditBase based = (ArtifactEditBase)Activator.CreateInstance(type);
                    if (Validate(based))
                    {
                        try { based.Init(); } catch (Exception ex) { Main.WRBLogger.LogError($"Failed to initialize {type.Name}: {ex}"); }
                    }
                }
            }

            if (Main.enableDifficulties.Value)
            {
                IEnumerable<Type> difficulty = Assembly.GetExecutingAssembly().GetTypes()
                                                .Where(type => !type.IsAbstract && type.IsSubclassOf(typeof(DifficultyBase)));

                Main.WRBLogger.LogInfo("==+----------------==DIFFICULTIES==----------------+==");

                foreach (Type type in difficulty)
                {
                    DifficultyBase based = (DifficultyBase)Activator.CreateInstance(type);
                    if (Validate(based))
                    {
                        try { based.Init(); } catch (Exception ex) { Main.WRBLogger.LogError($"Failed to initialize {type.Name}: {ex}"); }
                    }
                }
            }

            if (Main.enableElites.Value)
            {
                IEnumerable<Type> elite = Assembly.GetExecutingAssembly().GetTypes()
                                                .Where(type => !type.IsAbstract && type.IsSubclassOf(typeof(EliteBase)));

                Main.WRBLogger.LogInfo("==+----------------==ELITES==----------------+==");

                foreach (Type type in elite)
                {
                    EliteBase based = (EliteBase)Activator.CreateInstance(type);
                    if (Validate(based))
                    {
                        try { based.Init(); } catch (Exception ex) { Main.WRBLogger.LogError($"Failed to initialize {type.Name}: {ex}"); }
                    }
                }
            }

            if (Main.enableEnemies.Value)
            {
                IEnumerable<Type> enemy = Assembly.GetExecutingAssembly().GetTypes()
                                                .Where(type => !type.IsAbstract && type.IsSubclassOf(typeof(EnemyBase)));

                Main.WRBLogger.LogInfo("==+----------------==ENEMIES==----------------+==");

                foreach (Type type in enemy)
                {
                    EnemyBase based = (EnemyBase)Activator.CreateInstance(type);
                    if (Validate(based))
                    {
                        try { based.Init(); } catch (Exception ex) { Main.WRBLogger.LogError($"Failed to initialize {type.Name}: {ex}"); }
                    }
                }
            }

            if (Main.enableEquipment.Value)
            {
                IEnumerable<Type> equipment = Assembly.GetExecutingAssembly().GetTypes()
                                                .Where(type => !type.IsAbstract && type.IsSubclassOf(typeof(EquipmentBase)));

                Main.WRBLogger.LogInfo("==+----------------==EQUIPMENT==----------------+==");

                foreach (Type type in equipment)
                {
                    EquipmentBase based = (EquipmentBase)Activator.CreateInstance(type);
                    if (Validate(based))
                    {
                        try { based.Init(); } catch (Exception ex) { Main.WRBLogger.LogError($"Failed to initialize {type.Name}: {ex}"); }
                    }
                }
            }

            if (Main.enableGamemodes.Value)
            {
                IEnumerable<Type> gamemode = Assembly.GetExecutingAssembly().GetTypes()
                                                .Where(type => !type.IsAbstract && type.IsSubclassOf(typeof(GamemodeBase)));

                Main.WRBLogger.LogInfo("==+----------------==GAMEMODES==----------------+==");

                foreach (Type type in gamemode)
                {
                    GamemodeBase based = (GamemodeBase)Activator.CreateInstance(type);
                    if (Validate(based))
                    {
                        try { based.Init(); } catch (Exception ex) { Main.WRBLogger.LogError($"Failed to initialize {type.Name}: {ex}"); }
                    }
                }
            }

            if (Main.enableInteractables.Value)
            {
                IEnumerable<Type> interactable = Assembly.GetExecutingAssembly().GetTypes()
                                                .Where(type => !type.IsAbstract && type.IsSubclassOf(typeof(InteractableBase)));

                Main.WRBLogger.LogInfo("==+----------------==INTERACTABLES==----------------+==");

                foreach (Type type in interactable)
                {
                    InteractableBase based = (InteractableBase)Activator.CreateInstance(type);
                    if (Validate(based))
                    {
                        try { based.Init(); } catch (Exception ex) { Main.WRBLogger.LogError($"Failed to initialize {type.Name}: {ex}"); }
                    }
                }
            }

            if (Main.enableItems.Value)
            {
                IEnumerable<Type> item = Assembly.GetExecutingAssembly().GetTypes()
                                                .Where(type => !type.IsAbstract && type.IsSubclassOf(typeof(ItemBase)));

                Main.WRBLogger.LogInfo("==+----------------==ITEMS==----------------+==");

                foreach (Type type in item)
                {
                    ItemBase based = (ItemBase)Activator.CreateInstance(type);
                    if (Validate(based))
                    {
                        try { based.Init(); } catch (Exception ex) { Main.WRBLogger.LogError($"Failed to initialize {type.Name}: {ex}"); }
                    }
                }
                if (Items.Whites.PowerElixir.instance != null)
                    EmptyBottle.Init();
            }

            if (Main.enableMechanics.Value)
            {
                IEnumerable<Type> mechanic = Assembly.GetExecutingAssembly().GetTypes()
                                                .Where(type => !type.IsAbstract && type.IsSubclassOf(typeof(MechanicBase)));

                Main.WRBLogger.LogInfo("==+----------------==MECHANICS==----------------+==");

                foreach (Type type in mechanic)
                {
                    MechanicBase based = (MechanicBase)Activator.CreateInstance(type);
                    if (Validate(based))
                    {
                        try { based.Init(); } catch (Exception ex) { Main.WRBLogger.LogError($"Failed to initialize {type.Name}: {ex}"); }
                    }
                }
            }

            IEnumerable<Type> survivor = Assembly.GetExecutingAssembly().GetTypes()
                                                .Where(type => !type.IsAbstract && type.IsSubclassOf(typeof(SurvivorBase)));

            if (Main.enableSurvivors.Value)
            {
                Main.WRBLogger.LogInfo("==+----------------==SURVIVORS==----------------+==");
                /*
                Parallel.ForEach(survivor, type =>
                {
                    SurvivorBase based = (SurvivorBase)Activator.CreateInstance(type);
                    if (Validate(based))
                    {
                        try { based.Init(); } catch (Exception ex) { Main.WRBLogger.LogError($"Failed to initialize {type.Name}: {ex}"); }
                    }
                });
                */
                // not thread safe

                foreach (Type type in survivor)
                {
                    SurvivorBase based = (SurvivorBase)Activator.CreateInstance(type);
                    if (Validate(based))
                    {
                        try { based.Init(); } catch (Exception ex) { Main.WRBLogger.LogError($"Failed to initialize {type.Name}: {ex}"); }
                    }
                }
            }

            // FamilyEvents.Init();

            if (Main.enableLogging.Value)
            {
                AchievementBase.achievementList.Sort();
                AllyBase.allyList.Sort();
                ArtifactAddBase.artifactAddList.Sort();
                ArtifactEditBase.artifactEditList.Sort();
                DifficultyBase.difficultyList.Sort();
                EliteBase.eliteList.Sort();
                EnemyBase.enemyList.Sort();
                EquipmentBase.equipmentList.Sort();
                InteractableBase.interactableList.Sort();
                ItemBase.itemList.Sort();
                MechanicBase.mechanicList.Sort();
                SurvivorBase.survivorList.Sort();
                SharedBase.initList.Sort();
                for (int i = 0; i < AchievementBase.achievementList.Count; i++)
                {
                    var index = AchievementBase.achievementList[i];
                    Main.WRBLogger.LogDebug("Initialized " + index);
                }
                for (int i = 0; i < AllyBase.allyList.Count; i++)
                {
                    var index = AllyBase.allyList[i];
                    Main.WRBLogger.LogDebug("Initialized " + index);
                }
                for (int i = 0; i < ArtifactAddBase.artifactAddList.Count; i++)
                {
                    var index = ArtifactAddBase.artifactAddList[i];
                    Main.WRBLogger.LogDebug("Initialized " + index);
                }
                for (int i = 0; i < ArtifactEditBase.artifactEditList.Count; i++)
                {
                    var index = ArtifactEditBase.artifactEditList[i];
                    Main.WRBLogger.LogDebug("Initialized " + index);
                }
                for (int i = 0; i < DifficultyBase.difficultyList.Count; i++)
                {
                    var index = DifficultyBase.difficultyList[i];
                    Main.WRBLogger.LogDebug("Initialized " + index);
                }
                for (int i = 0; i < EliteBase.eliteList.Count; i++)
                {
                    var index = EliteBase.eliteList[i];
                    Main.WRBLogger.LogDebug("Initialized " + index);
                }
                for (int i = 0; i < EnemyBase.enemyList.Count; i++)
                {
                    var index = EnemyBase.enemyList[i];
                    Main.WRBLogger.LogDebug("Initialized " + index);
                }
                for (int i = 0; i < EquipmentBase.equipmentList.Count; i++)
                {
                    var index = EquipmentBase.equipmentList[i];
                    Main.WRBLogger.LogDebug("Initialized " + index);
                }
                for (int i = 0; i < InteractableBase.interactableList.Count; i++)
                {
                    var index = InteractableBase.interactableList[i];
                    Main.WRBLogger.LogDebug("Initialized " + index);
                }
                for (int i = 0; i < ItemBase.itemList.Count; i++)
                {
                    var index = ItemBase.itemList[i];
                    Main.WRBLogger.LogDebug("Initialized " + index);
                }
                for (int i = 0; i < MechanicBase.mechanicList.Count; i++)
                {
                    var index = MechanicBase.mechanicList[i];
                    Main.WRBLogger.LogDebug("Initialized " + index);
                }
                for (int i = 0; i < SurvivorBase.survivorList.Count; i++)
                {
                    var index = SurvivorBase.survivorList[i];
                    Main.WRBLogger.LogDebug("Initialized " + index);
                }
            }

            Main.WRBLogger.LogDebug("==+----------------==INFO==----------------+==");
            Main.WRBLogger.LogDebug("Initialized " + SharedBase.initList.Count + " abstract classes");
            Main.WRBLogger.LogDebug("Initialized mod in " + stopwatch.ElapsedMilliseconds + "ms");
            Main.WRBLogger.LogDebug("Lotussy");
        }

        private static void ItemCatalog_Init(On.RoR2.ItemCatalog.orig_Init orig)
        {
            Main.WRBLogger.LogDebug("ItemAPI.AddItemTag(\"Defense\") returns " + ItemAPI.AddItemTag("Defense"));
            BetterItemCategories.Init();
            orig();
            BetterItemCategories.BetterAIBlacklist();
        }

        public static bool Validate<T>(T obj) where T : SharedBase
        {
            if (obj.isEnabled)
            {
                bool enabledfr = GetConfigForType<T>().Bind(obj.Name, "Enable Changes?", true, "Vanilla is false").Value;
                if (enabledfr) return true;
                else ConfigManager.ConfigChanged = true;
            }
            return false;
        }

        private static ConfigFile GetConfigForType<T>() where T : SharedBase
        {
            return typeof(T).Name switch
            {
                nameof(AchievementBase) => Main.WRBAchievementConfig,
                nameof(AllyBase) => Main.WRBAllyConfig,
                nameof(ArtifactAddBase) => Main.WRBArtifactAddConfig,
                nameof(ArtifactEditBase) => Main.WRBArtifactEditConfig,
                nameof(DifficultyBase) => Main.WRBDifficultyConfig,
                nameof(EliteBase) => Main.WRBEliteConfig,
                nameof(EnemyBase) => Main.WRBEnemyConfig,
                nameof(EquipmentBase) => Main.WRBEquipmentConfig,
                nameof(GamemodeBase) => Main.WRBGamemodeConfig,
                nameof(InteractableBase) => Main.WRBInteractableConfig,
                nameof(ItemBase) => Main.WRBItemConfig,
                nameof(MechanicBase) => Main.WRBMechanicConfig,
                nameof(SurvivorBase) => Main.WRBSurvivorConfig,
                _ => throw new NotSupportedException($"Config not supported for type {typeof(T).Name}"),
            };
        }
    }
}