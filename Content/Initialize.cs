using BepInEx.Configuration;
using Mono.Cecil.Cil;
using Newtonsoft.Json.Utilities;
using R2API.MiscHelpers;
using RoR2;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Jobs;
using WellRoundedBalance.Achievements;
using WellRoundedBalance.Allies;
using WellRoundedBalance.Artifacts.New;
using WellRoundedBalance.Artifacts.Vanilla;
using WellRoundedBalance.Difficulties;
using WellRoundedBalance.Elites;
using WellRoundedBalance.Enemies;
using WellRoundedBalance.Enemies.All;
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
        /*
        public static JobHandle handle;
        public static NativeArray<AchievementBase> result;

        public struct AchievementJob : IJobParallelFor
        {
            [ReadOnly]
            public NativeArray<AchievementBase> result = new(1, Allocator.TempJob);

            public void Execute()
            {
                Type type = result[0];

                AchievementBase based = (AchievementBase)Activator.CreateInstance(type);
                if (Validate(based))
                {
                    try
                    {
                        based.Init();
                    }
                    catch (Exception ex)
                    {
                        Main.WRBLogger.LogError($"Failed to initialize {type.Name}: {ex}");
                    }
                }
            }
        }
        */

        // bruh what the fuck is this shit I hate jobs I hate unity what the

        public static void Init()
        {
            var stopwatch = Stopwatch.StartNew();

            FunnyLabel.Init();
            // Useless.Create();
            Buffs.Useless.Init();
            VoidBall.Init();
            BlazingProjectileVFX.Init();
            Molotov.Init();
            MolotovBig.Init();
            DucleusLaser.Init();
            TitanFist.Init();
            EarthQuakeWave.Init();
            GupSpike.Init();

            On.RoR2.ItemCatalog.Init += ItemCatalog_Init;

            object achievementLock = new();
            object allyLock = new();
            object artifactAddLock = new();
            object artifactEditLock = new();
            object difficultyLock = new();
            object eliteLock = new();
            object enemyLock = new();
            object equipmentLock = new();
            object gamemodeLock = new();
            object interactableLock = new();
            object itemLock = new();
            object mechanicLock = new();
            object survivorLock = new();

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

                /*
                result = new NativeArray<AchievementBase>(1, Allocator.TempJob);

                AchievementJob achievementJob = new()
                {
                    result = result
                };

                handle = achievementJob.Schedule();

                // Sometime later in the frame, wait for the job to complete before accessing the results. bruh can I not do it outside a monobehaviour or something
                handle.Complete();

                result.Dispose();
                */
                // bruh
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

            if (Main.enableSurvivors.Value)
            {
                IEnumerable<Type> survivor = Assembly.GetExecutingAssembly().GetTypes()
                                                    .Where(type => !type.IsAbstract && type.IsSubclassOf(typeof(SurvivorBase)));

                Main.WRBLogger.LogInfo("==+----------------==SURVIVORS==----------------+==");

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