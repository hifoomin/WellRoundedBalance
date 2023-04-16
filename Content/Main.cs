using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using System;
using System.Reflection;

using WellRoundedBalance.Items;
using WellRoundedBalance.Equipment;
using WellRoundedBalance.Interactables;
using WellRoundedBalance.Enemies;
using WellRoundedBalance.Projectiles;
using System.Runtime.CompilerServices;
using WellRoundedBalance.Elites;
using R2API.ContentManagement;

using WellRoundedBalance.Misc;

using WellRoundedBalance.Mechanics;

using WellRoundedBalance.Items.NoTier;
using WellRoundedBalance.Difficulties;
using WellRoundedBalance.Artifacts.Vanilla;
using WellRoundedBalance.Artifacts.New;
using WellRoundedBalance.Gamemodes;
using WellRoundedBalance.Items.ConsistentCategories;
using MonoMod.RuntimeDetour;

using WellRoundedBalance.Mechanics.Monsters;

// using WellRoundedBalance.Enemies.FamilyEvents;

[assembly: HG.Reflection.SearchableAttribute.OptIn]

namespace WellRoundedBalance
{
    [BepInDependency("HIFU.Inferno", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Moffein.RiskyArtifacts", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Moffein.AI_Blacklist", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Wolfo.WolfoQualityOfLife", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Wolfo.LittleGameplayTweaks", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(LanguageAPI.PluginGUID)]
    [BepInDependency(RecalculateStatsAPI.PluginGUID)]
    [BepInDependency(DirectorAPI.PluginGUID)]
    [BepInDependency(PrefabAPI.PluginGUID)]
    [BepInDependency(R2APIContentManager.PluginGUID)]
    [BepInDependency(ItemAPI.PluginGUID)]
    [BepInDependency(DamageAPI.PluginGUID)]
    [BepInDependency(EliteAPI.PluginGUID)]
    [BepInDependency(DotAPI.PluginGUID)]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class Main : BaseUnityPlugin
    {
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "BALLS";
        public const string PluginName = "WellRoundedBalance";
        public const string PluginVersion = "1.2.0";
        public static ConfigFile WRBConfig;
        public static ConfigFile WRBItemConfig;
        public static ConfigFile WRBMechanicConfig;
        public static ConfigFile WRBEquipmentConfig;
        public static ConfigFile WRBInteractableConfig;
        public static ConfigFile WRBEnemyConfig;
        public static ConfigFile WRBEliteConfig;
        public static ConfigFile WRBGamemodeConfig;
        public static ConfigFile WRBArtifactConfig;
        public static ConfigFile WRBDifficultyConfig;
        public static ConfigFile WRBMiscConfig;
        public static ManualLogSource WRBLogger;

        public static ConfigEntry<bool> enableLogging { get; set; }
        public ConfigEntry<bool> enableAutoConfig { get; private set; }
        public ConfigEntry<string> latestVersion { get; private set; }

        public static AssetBundle wellroundedbalance;

        public static bool InfernoLoaded = false;
        public static bool RiskyArtifactsLoaded = false;
        public static bool PieceOfShitLoaded = false;
        public static DifficultyDef InfernoDef = null;
        public static Hook hook;

        public void Awake()
        {
            WRBLogger = Logger;
            WRBConfig = Config;

            wellroundedbalance = AssetBundle.LoadFromFile(Assembly.GetExecutingAssembly().Location.Replace("WellRoundedBalance.dll", "wellroundedbalance"));

            WRBItemConfig = new ConfigFile(Paths.ConfigPath + "\\BALLS.WellRoundedBalance.Items.cfg", true);
            WRBMechanicConfig = new ConfigFile(Paths.ConfigPath + "\\BALLS.WellRoundedBalance.Mechanics.cfg", true);
            WRBEquipmentConfig = new ConfigFile(Paths.ConfigPath + "\\BALLS.WellRoundedBalance.Equipment.cfg", true);
            WRBInteractableConfig = new ConfigFile(Paths.ConfigPath + "\\BALLS.WellRoundedBalance.Interactables.cfg", true);
            WRBEnemyConfig = new ConfigFile(Paths.ConfigPath + "\\BALLS.WellRoundedBalance.Enemies.cfg", true);
            WRBEliteConfig = new ConfigFile(Paths.ConfigPath + "\\BALLS.WellRoundedBalance.Elites.cfg", true);
            WRBGamemodeConfig = new ConfigFile(Paths.ConfigPath + "\\BALLS.WellRoundedBalance.Gamemodes.cfg", true);
            WRBArtifactConfig = new ConfigFile(Paths.ConfigPath + "\\BALLS.WellRoundedBalance.Artifacts.cfg", true);
            WRBDifficultyConfig = new ConfigFile(Paths.ConfigPath + "\\BALLS.WellRoundedBalance.Difficulties.cfg", true);
            WRBMiscConfig = new ConfigFile(Paths.ConfigPath + "\\BALLS.WellRoundedBalance.Misc.cfg", true);

            enableLogging = WRBMiscConfig.Bind("Logging", "Enable Initialization logging?", false, "Enabling this slows down loading times, but can help with resolving mod compatibility issues in some cases.");
            enableAutoConfig = WRBMiscConfig.Bind("Config", "Enable Auto Config Sync", true, "Disabling this would stop WRB from syncing config whenever a new version is found.");
            bool _preVersioning = WRBMiscConfig.Keys.Any(x => x.Key == "Latest Version");
            latestVersion = WRBMiscConfig.Bind("Config", "Latest Version", PluginVersion, "DO NOT CHANGE THIS");
            if (enableAutoConfig.Value && (_preVersioning || latestVersion.Value != PluginVersion))
            {
                latestVersion.Value = PluginVersion;
                ConfigManager.VersionChanged = true;
                WRBLogger.LogInfo("Config Autosync Enabled.");
            }

            InfernoLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("HIFU.Inferno");
            RiskyArtifactsLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.RiskyArtifacts");
            PieceOfShitLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Wolfo.WolfoQualityOfLife");

            FunnyLabel.Hooks();
            // Useless.Create();
            Buffs.Useless.Create();
            VoidBall.Create();
            BlazingProjectileVFX.Create();
            Molotov.Create();
            DucleusLaser.Create();
            TitanFist.Create();

            On.RoR2.ItemCatalog.Init += ItemCatalog_Init;

            IEnumerable<Type> enumerable = from type in Assembly.GetExecutingAssembly().GetTypes()
                                           where !type.IsAbstract && type.IsSubclassOf(typeof(MechanicBase))
                                           select type;

            WRBLogger.LogInfo("==+----------------==MECHANICS==----------------+==");

            foreach (Type type in enumerable)
            {
                MechanicBase based = (MechanicBase)Activator.CreateInstance(type);
                if (ValidateMechanic(based))
                {
                    based.Init();
                }
            }

            IEnumerable<Type> enumerable2 = from type in Assembly.GetExecutingAssembly().GetTypes()
                                            where !type.IsAbstract && type.IsSubclassOf(typeof(ItemBase))
                                            select type;

            WRBLogger.LogInfo("==+----------------==ITEMS==----------------+==");
            // List<ItemBase> baseds = new();

            foreach (Type type in enumerable2)
            {
                ItemBase based = (ItemBase)Activator.CreateInstance(type);
                if (ValidateItem(based))
                {
                    based.Init();
                }

                // done alphabetically
                // baseds.Add((ItemBase)Activator.CreateInstance(type));
            }

            // nvm this breaks the entire game what
            /*
            foreach (ItemBase itemBased in baseds.OrderBy(x => (char)x.InternalPickupToken.ToLower()[0]))
            {
                // Debug.Log(itemBased.InternalPickupToken);
                if (ValidateItem(itemBased))
                {
                    itemBased.Init();
                }
            }
            */

            IEnumerable<Type> enumerable3 = from type in Assembly.GetExecutingAssembly().GetTypes()
                                            where !type.IsAbstract && type.IsSubclassOf(typeof(EquipmentBase))
                                            select type;

            WRBLogger.LogInfo("==+----------------==EQUIPMENT==----------------+==");

            foreach (Type type in enumerable3)
            {
                EquipmentBase based = (EquipmentBase)Activator.CreateInstance(type);
                if (ValidateEquipment(based))
                {
                    based.Init();
                }
            }

            IEnumerable<Type> enumerable4 = from type in Assembly.GetExecutingAssembly().GetTypes()
                                            where !type.IsAbstract && type.IsSubclassOf(typeof(InteractableBase))
                                            select type;

            WRBLogger.LogInfo("==+----------------==INTERACTABLES==----------------+==");

            foreach (Type type in enumerable4)
            {
                InteractableBase based = (InteractableBase)Activator.CreateInstance(type);
                if (ValidateInteractable(based))
                {
                    based.Init();
                }
            }

            IEnumerable<Type> enumerable5 = from type in Assembly.GetExecutingAssembly().GetTypes()
                                            where !type.IsAbstract && type.IsSubclassOf(typeof(EnemyBase))
                                            select type;

            WRBLogger.LogInfo("==+----------------==ENEMIES==----------------+==");

            foreach (Type type in enumerable5)
            {
                EnemyBase based = (EnemyBase)Activator.CreateInstance(type);
                if (ValidateEnemy(based))
                {
                    based.Init();
                }
            }

            IEnumerable<Type> enumerable6 = from type in Assembly.GetExecutingAssembly().GetTypes()
                                            where !type.IsAbstract && type.IsSubclassOf(typeof(EliteBase))
                                            select type;

            WRBLogger.LogInfo("==+----------------==ELITES==----------------+==");

            foreach (Type type in enumerable6)
            {
                EliteBase based = (EliteBase)Activator.CreateInstance(type);
                if (ValidateElite(based))
                {
                    based.Init();
                }
            }

            IEnumerable<Type> enumerable9 = from type in Assembly.GetExecutingAssembly().GetTypes()
                                            where !type.IsAbstract && type.IsSubclassOf(typeof(DifficultyBase))
                                            select type;

            WRBLogger.LogInfo("==+----------------==DIFFICULTIES==----------------+==");

            foreach (Type type in enumerable9)
            {
                DifficultyBase based = (DifficultyBase)Activator.CreateInstance(type);
                if (ValidateDifficulty(based))
                {
                    based.Init();
                }
            }

            IEnumerable<Type> enumerable7 = from type in Assembly.GetExecutingAssembly().GetTypes()
                                            where !type.IsAbstract && type.IsSubclassOf(typeof(GamemodeBase))
                                            select type;

            WRBLogger.LogInfo("==+----------------==GAMEMODES==----------------+==");

            foreach (Type type in enumerable7)
            {
                GamemodeBase based = (GamemodeBase)Activator.CreateInstance(type);
                if (ValidateGamemode(based))
                {
                    based.Init();
                }
            }

            IEnumerable<Type> enumerable8 = from type in Assembly.GetExecutingAssembly().GetTypes()
                                            where !type.IsAbstract && type.IsSubclassOf(typeof(ArtifactEditBase))
                                            select type;

            IEnumerable<Type> enumerable10 = from type in Assembly.GetExecutingAssembly().GetTypes()
                                             where !type.IsAbstract && type.IsSubclassOf(typeof(ArtifactAddBase))
                                             select type;

            WRBLogger.LogInfo("==+----------------==ARTIFACTS==----------------+==");

            foreach (Type type in enumerable8)
            {
                ArtifactEditBase based = (ArtifactEditBase)Activator.CreateInstance(type);
                if (ValidateArtifactEdit(based))
                {
                    based.Init();
                }
            }

            foreach (Type type in enumerable10)
            {
                ArtifactAddBase based = (ArtifactAddBase)Activator.CreateInstance(type);
                if (ValidateArtifactAdd(based))
                {
                    // based.Init();
                    // disabled until icon is done
                }
            }

            // FamilyEvents.Init();
            EmptyBottle.Init();

            string balls = WRBMiscConfig.Bind("Annoying Pop Up", "Set to Fuck Off to disable", "", "Disables the mf config changed message").Value;
            bool shownConfigMessage = false;
            RoR2Application.onLoad += () => Dialogue.input = GameObject.Find("MPEventSystem Player0").GetComponent<RoR2.UI.MPInput>();
            On.RoR2.UI.MainMenu.BaseMainMenuScreen.OnEnter += (orig, self, mainMenuController) =>
            {
                orig(self, mainMenuController);
                if (!shownConfigMessage && ConfigManager.ConfigChanged && balls.ToLower() != "fuck off")
                {
                    shownConfigMessage = true;
                    Dialogue.ShowPopup("Config changed?", "Thank you for enjoying Well Rounded Balance <3! Despite the extensive configuration, we want our default experience to be as enjoyable as possible. Please let us know your balanced takes at <style=cDeath>cutt.ly/ballscord</style>! any constructive feedback is welcome <3.\n\n<style=cStack>set Misc > Annoying Pop Up to \'Fuck Off\' to disable this message.</style>");
                }
            };

            if (enableLogging.Value)
            {
                SharedBase.initList.Sort();
                for (int i = 0; i < SharedBase.initList.Count; i++)
                {
                    var index = SharedBase.initList[i];
                    WRBLogger.LogDebug("Initialized abstract " + index);
                }
            }

            WRBLogger.LogDebug("==+----------------==INFO==----------------+==");
            WRBLogger.LogDebug("Initialized " + SharedBase.initList.Count + " abstract classes");
            On.RoR2.UI.MainMenu.BaseMainMenuScreen.OnEnter += BaseMainMenuScreen_OnEnter;

            if (PieceOfShitLoaded)
            {
                WRBLogger.LogDebug("Wolfo QoL detected");
                On.RoR2.PickupPickerController.OnDisplayBegin += PickupPickerController_OnDisplayBegin;
            }
        }

        private void PickupPickerController_OnDisplayBegin(On.RoR2.PickupPickerController.orig_OnDisplayBegin orig, PickupPickerController self, NetworkUIPromptController networkUIPromptController, LocalUser localUser, CameraRigController cameraRigController)
        {
            orig(self, networkUIPromptController, localUser, cameraRigController);
            return;
        }

        private void BaseMainMenuScreen_OnEnter(On.RoR2.UI.MainMenu.BaseMainMenuScreen.orig_OnEnter orig, RoR2.UI.MainMenu.BaseMainMenuScreen self, RoR2.UI.MainMenu.MainMenuController mainMenuController)
        {
            orig(self, mainMenuController);
            WRBLogger.LogDebug("==+----------------==ZANY==----------------+==");
            for (int j = 0; j < 3; j++)
            {
                WRBLogger.LogMessage("Thanks for playing Well Rounded Balance <3");
            }
            WRBLogger.LogDebug("==+----------------==GOOFY==----------------+==");
        }

        private void ItemCatalog_Init(On.RoR2.ItemCatalog.orig_Init orig)
        {
            WRBLogger.LogDebug("ItemAPI.AddItemTag(\"Defense\") returns " + ItemAPI.AddItemTag("Defense"));
            BetterItemCategories.Init();
            orig();
            BetterItemCategories.BetterAIBlacklist();
        }

        public bool ValidateMechanic(MechanicBase gb)
        {
            if (gb.isEnabled)
            {
                bool enabledfr = WRBMechanicConfig.Bind(gb.Name, "Enable Changes?", true, "Vanilla is false").Value;
                if (enabledfr) return true;
                else ConfigManager.ConfigChanged = true;
            }
            return false;
        }

        public bool ValidateItem(ItemBase ib)
        {
            if (ib.isEnabled)
            {
                bool enabledfr = WRBItemConfig.Bind(ib.Name, "Enable Changes?", true, "Vanilla is false").Value;
                if (enabledfr) return true;
                else ConfigManager.ConfigChanged = true;
            }
            return false;
        }

        public bool ValidateEquipment(EquipmentBase eqb)
        {
            if (eqb.isEnabled)
            {
                bool enabledfr = WRBEquipmentConfig.Bind(eqb.Name, "Enable Changes?", true, "Vanilla is false").Value;
                if (enabledfr) return true;
                else ConfigManager.ConfigChanged = true;
            }
            return false;
        }

        public bool ValidateInteractable(InteractableBase ib)
        {
            if (ib.isEnabled)
            {
                bool enabledfr = WRBInteractableConfig.Bind(ib.Name, "Enable Changes?", true, "Vanilla is false").Value;
                if (enabledfr) return true;
                else ConfigManager.ConfigChanged = true;
            }
            return false;
        }

        public bool ValidateEnemy(EnemyBase enb)
        {
            if (enb.isEnabled)
            {
                bool enabledfr = WRBEnemyConfig.Bind(enb.Name, "Enable Changes?", true, "Vanilla is false").Value;
                if (enabledfr) return true;
                else ConfigManager.ConfigChanged = true;
            }
            return false;
        }

        public bool ValidateElite(EliteBase elb)
        {
            if (elb.isEnabled)
            {
                bool enabledfr = WRBEliteConfig.Bind(elb.Name, "Enable Changes?", true, "Vanilla is false").Value;
                if (enabledfr) return true;
                else ConfigManager.ConfigChanged = true;
            }
            return false;
        }

        public bool ValidateGamemode(GamemodeBase gmb)
        {
            if (gmb.isEnabled)
            {
                bool enabledfr = WRBGamemodeConfig.Bind(gmb.Name, "Enable Changes?", true, "Vanilla is false").Value;
                if (enabledfr) return true;
                else ConfigManager.ConfigChanged = true;
            }
            return false;
        }

        public bool ValidateArtifactEdit(ArtifactEditBase aeb)
        {
            if (aeb.isEnabled)
            {
                bool enabledfr = WRBArtifactConfig.Bind(aeb.Name, "Enable Changes?", true, "Vanilla is false").Value;
                if (enabledfr) return true;
                else ConfigManager.ConfigChanged = true;
            }
            return false;
        }

        public bool ValidateArtifactAdd(ArtifactAddBase aab)
        {
            if (aab.isEnabled)
            {
                bool enabledfr = WRBArtifactConfig.Bind(aab.Name, "Enable?", true, "Vanilla is false").Value;
                if (enabledfr) return true;
                else ConfigManager.ConfigChanged = true;
            }
            return false;
        }

        public bool ValidateDifficulty(DifficultyBase db)
        {
            if (db.isEnabled)
            {
                bool enabledfr = WRBDifficultyConfig.Bind(db.Name, "Enable Changes?", true, "Vanilla is false").Value;
                if (enabledfr) return true;
                else ConfigManager.ConfigChanged = true;
            }
            return false;
        }

        private void InfernoCompat()
        {
            if (InfernoLoaded)
            {
                InfernoDef = GetInfernoDef();
            }
        }

        public static float GetProjectileSimpleModifiers(float speed)
        {
            if (InfernoLoaded) speed *= GetInfernoProjectileSpeedMult();
            if (RiskyArtifactsLoaded) speed *= GetRiskyArtifactsWarfareProjectileSpeedMult();
            return speed;
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static float GetRiskyArtifactsWarfareProjectileSpeedMult()
        {
            if (RunArtifactManager.instance && RunArtifactManager.instance.IsArtifactEnabled(Risky_Artifacts.Artifacts.Warfare.artifact))
            {
                return Risky_Artifacts.Artifacts.Warfare.projSpeed;
            }
            return 1f;
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static DifficultyDef GetInfernoDef()
        {
            return Inferno.Main.InfernoDiffDef;
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static bool IsInfernoDef()
        {
            if (InfernoLoaded && Run.instance)
            {
                if (DifficultyCatalog.GetDifficultyDef(Run.instance.selectedDifficulty) == InfernoDef) return true;
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static float GetInfernoProjectileSpeedMult()
        {
            if (Run.instance && DifficultyCatalog.GetDifficultyDef(Run.instance.selectedDifficulty) == InfernoDef)
            {
                return Inferno.Main.ProjectileSpeed.Value;
            }
            return 1f;
        }
    }
}