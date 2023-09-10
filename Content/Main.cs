using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using System.Reflection;
using System.Runtime.CompilerServices;
using R2API.ContentManagement;
using MonoMod.RuntimeDetour;
using HarmonyLib;
using WellRoundedBalance.Items.ConsistentCategories;

[assembly: HG.Reflection.SearchableAttribute.OptIn]

namespace WellRoundedBalance
{
    [BepInDependency("HIFU.Inferno", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Moffein.RiskyArtifacts", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Moffein.AI_Blacklist", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Wolfo.WolfoQualityOfLife", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Wolfo.LittleGameplayTweaks", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("dev.wildbook.multitudes", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.TPDespair.ZetArtifacts", BepInDependency.DependencyFlags.SoftDependency)]
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
        public const string PluginVersion = "1.3.8.5";
        public static ConfigFile WRBAchievementConfig;
        public static ConfigFile WRBAllyConfig;
        public static ConfigFile WRBArtifactAddConfig;
        public static ConfigFile WRBArtifactEditConfig;
        public static ConfigFile WRBConfig;
        public static ConfigFile WRBDifficultyConfig;
        public static ConfigFile WRBEliteConfig;
        public static ConfigFile WRBEnemyConfig;
        public static ConfigFile WRBEquipmentConfig;
        public static ConfigFile WRBGamemodeConfig;
        public static ConfigFile WRBInteractableConfig;
        public static ConfigFile WRBItemConfig;
        public static ConfigFile WRBMechanicConfig;
        public static ConfigFile WRBModuleConfig;
        public static ConfigFile WRBSurvivorConfig;
        public static ConfigFile WRBMiscConfig;
        public static ConfigFile WRBBackupConfig; // DO NOT USE THIS !! JJJJJ You like to kiss boys you're a boykisser
        public static ManualLogSource WRBLogger;

        public static ConfigEntry<bool> enableLogging { get; set; }
        public ConfigEntry<bool> enableAutoConfig { get; private set; }
        public ConfigEntry<string> latestVersion { get; private set; }

        public static ConfigEntry<bool> enableAchievements { get; set; }
        public static ConfigEntry<bool> enableAllies { get; set; }
        public static ConfigEntry<bool> enableArtifactAdds { get; set; }
        public static ConfigEntry<bool> enableArtifactEdits { get; set; }
        public static ConfigEntry<bool> enableDifficulties { get; set; }
        public static ConfigEntry<bool> enableElites { get; set; }
        public static ConfigEntry<bool> enableEnemies { get; set; }
        public static ConfigEntry<bool> enableEquipment { get; set; }
        public static ConfigEntry<bool> enableGamemodes { get; set; }
        public static ConfigEntry<bool> enableInteractables { get; set; }
        public static ConfigEntry<bool> enableItems { get; set; }
        public static ConfigEntry<bool> enableMechanics { get; set; }
        public static ConfigEntry<bool> enableSurvivors { get; set; }

        public static AssetBundle wellroundedbalance;

        public static bool InfernoLoaded = false;
        public static bool RiskyArtifactsLoaded = false;
        public static bool PieceOfShitLoaded = false;
        public static bool ZetArtifactsLoaded = false;
        public static bool WildbookMultitudesLoaded = false;
        public static DifficultyDef InfernoDef = null;
        public static Hook hook;

        private bool mp = false;
        private bool hasZanySoupd = false;

        public void Awake()
        {
            WRBLogger = Logger;
            WRBConfig = Config;

            wellroundedbalance = AssetBundle.LoadFromFile(Assembly.GetExecutingAssembly().Location.Replace("WellRoundedBalance.dll", "wellroundedbalance"));

            WRBAchievementConfig = new ConfigFile(Paths.ConfigPath + "\\BALLS.WellRoundedBalance.Achievements.cfg", true);
            WRBAllyConfig = new ConfigFile(Paths.ConfigPath + "\\BALLS.WellRoundedBalance.Allies.cfg", true);
            WRBArtifactAddConfig = new ConfigFile(Paths.ConfigPath + "\\BALLS.WellRoundedBalance.ArtifactsAdd.cfg", true);
            WRBArtifactEditConfig = new ConfigFile(Paths.ConfigPath + "\\BALLS.WellRoundedBalance.ArtifactsEdit.cfg", true);
            WRBDifficultyConfig = new ConfigFile(Paths.ConfigPath + "\\BALLS.WellRoundedBalance.Difficulties.cfg", true);
            WRBEliteConfig = new ConfigFile(Paths.ConfigPath + "\\BALLS.WellRoundedBalance.Elites.cfg", true);
            WRBEnemyConfig = new ConfigFile(Paths.ConfigPath + "\\BALLS.WellRoundedBalance.Enemies.cfg", true);
            WRBEquipmentConfig = new ConfigFile(Paths.ConfigPath + "\\BALLS.WellRoundedBalance.Equipment.cfg", true);
            WRBGamemodeConfig = new ConfigFile(Paths.ConfigPath + "\\BALLS.WellRoundedBalance.Gamemodes.cfg", true);
            WRBInteractableConfig = new ConfigFile(Paths.ConfigPath + "\\BALLS.WellRoundedBalance.Interactables.cfg", true);
            WRBItemConfig = new ConfigFile(Paths.ConfigPath + "\\BALLS.WellRoundedBalance.Items.cfg", true);
            WRBMechanicConfig = new ConfigFile(Paths.ConfigPath + "\\BALLS.WellRoundedBalance.Mechanics.cfg", true);
            WRBModuleConfig = new ConfigFile(Paths.ConfigPath + "\\BALLS.WellRoundedBalance.Modules.cfg", true);
            WRBSurvivorConfig = new ConfigFile(Paths.ConfigPath + "\\BALLS.WellRoundedBalance.Survivors.cfg", true);

            BetterItemCategories.enable = WRBItemConfig.Bind(":: Items : Changes :: Better Item Categories", "Enable item category changes?", true);

            enableAchievements = WRBModuleConfig.Bind(":: Module Toggles ::", "Enable Achievement changes?", true, "Disabling this could cause achievements to get locked again, if the unlockable method changed or got more difficult.");
            enableAllies = WRBModuleConfig.Bind(":: Module Toggles ::", "Enable Ally changes?", true);
            enableArtifactAdds = WRBModuleConfig.Bind(":: Module Toggles ::", "Enable New Artifacts?", true);
            enableArtifactEdits = WRBModuleConfig.Bind(":: Module Toggles ::", "Enable Artifact changes?", true);
            enableDifficulties = WRBModuleConfig.Bind(":: Module Toggles ::", "Enable Difficulty changes?", true);
            enableElites = WRBModuleConfig.Bind(":: Module Toggles ::", "Enable Elite changes?", true, "Disabling this will cause the Eclipse 3 modifier to not function, as it's impossible not to hardcode it, and I'm not hardcoding even more effects for vanilla elites which I dislike. The same goes for disabling individual elites - their particular \"stronger in unique ways\" modifier will not work on Eclipse 3 or above.");

            enableEnemies = WRBModuleConfig.Bind(":: Module Toggles ::", "Enable Enemy changes?", true);
            enableEquipment = WRBModuleConfig.Bind(":: Module Toggles ::", "Enable Equipment changes?", true);
            enableGamemodes = WRBModuleConfig.Bind(":: Module Toggles ::", "Enable Gamemode changes?", true);
            enableInteractables = WRBModuleConfig.Bind(":: Module Toggles ::", "Enable Interactable changes?", true);
            enableItems = WRBModuleConfig.Bind(":: Module Toggles ::", "Enable Item changes?", true);
            enableMechanics = WRBModuleConfig.Bind(":: Module Toggles ::", "Enable Mechanic changes?", true);
            enableSurvivors = WRBModuleConfig.Bind(":: Module Toggles ::", "Enable Survivor changes?", true, "These are not HIFUTweaks changes, as they all have separate configs.");

            WRBMiscConfig = new ConfigFile(Paths.ConfigPath + "\\BALLS.WellRoundedBalance.Misc.cfg", true);
            WRBBackupConfig = new ConfigFile(Paths.ConfigPath + "\\BALLS.WellRoundedBalance.Backup.cfg", true);
            WRBBackupConfig.Bind(": DO NOT MODIFY THIS FILES CONTENTS :", ": DO NOT MODIFY THIS FILES CONTENTS :", ": DO NOT MODIFY THIS FILES CONTENTS :", ": DO NOT MODIFY THIS FILES CONTENTS :");

            enableLogging = WRBMiscConfig.Bind("Logging", "Enable Initialization logging?", false, "Enabling this slows down loading times, but can help with resolving mod compatibility issues in some cases.");
            enableAutoConfig = WRBMiscConfig.Bind("Config", "Enable Auto Config Sync", true, "Disabling this would stop WRB from syncing config whenever a new version is found.");
            bool _preVersioning = !((Dictionary<ConfigDefinition, string>)AccessTools.DeclaredPropertyGetter(typeof(ConfigFile), "OrphanedEntries").Invoke(WRBMiscConfig, null)).Keys.Any(x => x.Key == "Latest Version");
            latestVersion = WRBMiscConfig.Bind("Config", "Latest Version", PluginVersion, "DO NOT CHANGE THIS");
            if (enableAutoConfig.Value && (_preVersioning || (latestVersion.Value != PluginVersion)))
            {
                latestVersion.Value = PluginVersion;
                ConfigManager.VersionChanged = true;
                WRBLogger.LogInfo("Config Autosync Enabled.");
            }

            if (mp)
            {
                On.RoR2.Networking.NetworkManagerSystemSteam.OnClientConnect += (s, u, t) => { };
                // nvm cant cause of backup config sharing violation on path
            }

            InfernoLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("HIFU.Inferno");
            RiskyArtifactsLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.RiskyArtifacts");
            PieceOfShitLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Wolfo.WolfoQualityOfLife");
            ZetArtifactsLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.TPDespair.ZetArtifacts");
            WildbookMultitudesLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("dev.wildbook.multitudes");

            Initialize.Init();

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

            On.RoR2.UI.MainMenu.BaseMainMenuScreen.OnEnter += BaseMainMenuScreen_OnEnter;

            if (PieceOfShitLoaded)
            {
                WRBLogger.LogDebug("Wolfo QoL detected");
                On.RoR2.PickupPickerController.OnDisplayBegin += PickupPickerController_OnDisplayBegin;
            }

            InfernoCompat();
        }

        private void PickupPickerController_OnDisplayBegin(On.RoR2.PickupPickerController.orig_OnDisplayBegin orig, PickupPickerController self, NetworkUIPromptController networkUIPromptController, LocalUser localUser, CameraRigController cameraRigController)
        {
            orig(self, networkUIPromptController, localUser, cameraRigController);
            return;
        }

        private void BaseMainMenuScreen_OnEnter(On.RoR2.UI.MainMenu.BaseMainMenuScreen.orig_OnEnter orig, RoR2.UI.MainMenu.BaseMainMenuScreen self, RoR2.UI.MainMenu.MainMenuController mainMenuController)
        {
            orig(self, mainMenuController);
            if (!hasZanySoupd)
            {
                WRBLogger.LogDebug("==+----------------==ZANY==----------------+==");
                for (int j = 0; j < 3; j++)
                {
                    WRBLogger.LogMessage("Thanks for playing Well Rounded Balance <3");
                }
                WRBLogger.LogDebug("==+----------------==GOOFY==----------------+==");
                hasZanySoupd = true;
            }
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
                if (DifficultyCatalog.GetDifficultyDef(Run.instance.selectedDifficulty) == InfernoDef)
                {
                    // WRBLogger.LogError("Difficulty is inferno");
                    return true;
                }
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