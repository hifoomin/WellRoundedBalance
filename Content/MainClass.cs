using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using R2API.Utils;
using System;
using System.Linq;
using System.Reflection;
using WellRoundedBalance.Items;
using WellRoundedBalance.Equipment;
using WellRoundedBalance.Global;
using WellRoundedBalance.Interactables;
using WellRoundedBalance.Mechanic;
using WellRoundedBalance.Enemies;
using WellRoundedBalance.Projectiles;
using WellRoundedBalance.Eclipse;
using System.Runtime.CompilerServices;
using WellRoundedBalance.Elites;
[assembly: HG.Reflection.SearchableAttribute.OptIn]
// used for BodyCatalog

namespace WellRoundedBalance
{
    [BepInDependency("HIFU.Inferno", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Moffein.RiskyArtifacts", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(R2API.R2API.PluginGUID)]
    /*  Swap to new R2APIs later
        Rework Defense Nucleus
        Rework Titanic Knurl
        Rework Power Elixir to old riskymod (upon taking heavy damage, quickly regen and consume this item, regenerates each stage)
        Make Lunar Pod give you the item directly >:)
        Implement Duh's void cradle idea
        Make it so you can only take a couple lunar coins per run (maybe like 6?)
        Make it so shrine of order gives you 1-3 lunar coins
        Make it so void cradles give x% curse instead of costing %max hp
    */
    [BepInDependency("com.Wolfo.WolfoQualityOfLife", BepInDependency.DependencyFlags.SoftDependency)]
    // [BepInDependency("com.xoxfaby.BetterUI", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("dev.ontrigger.itemstats", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("Withor.FixedDescriptions", BepInDependency.DependencyFlags.SoftDependency)] // may thy name shall not curse mine project
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [R2APISubmoduleDependency(nameof(LanguageAPI), nameof(RecalculateStatsAPI), nameof(LoadoutAPI), nameof(DirectorAPI), nameof(PrefabAPI), nameof(ItemAPI))]
    public class Main : BaseUnityPlugin
    {
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "BALLS";
        public const string PluginName = "WellRoundedBalance";
        public const string PluginVersion = "0.0.1";
        public static ConfigFile WRBConfig;
        public static ConfigFile WRBItemConfig;
        public static ConfigFile WRBMechanicConfig;
        public static ConfigFile WRBEquipmentConfig;
        public static ConfigFile WRBInteractableConfig;
        public static ConfigFile WRBEnemyConfig;
        public static ConfigFile WRBEliteConfig;
        public static ConfigFile WRBGamemodeConfig;
        public static ManualLogSource WRBLogger;

        public static bool InfernoLoaded = false;
        public static bool RiskyArtifactsLoaded = false;
        public static DifficultyDef InfernoDef = null;

        public void Awake()
        {
            WRBLogger = Logger;
            Main.WRBConfig = base.Config;

            WRBItemConfig = new ConfigFile(Paths.ConfigPath + "\\BALLS.WellRoundedBalance.Items.cfg", true);
            WRBMechanicConfig = new ConfigFile(Paths.ConfigPath + "\\BALLS.WellRoundedBalance.Mechanics.cfg", true);
            WRBEquipmentConfig = new ConfigFile(Paths.ConfigPath + "\\BALLS.WellRoundedBalance.Equipment.cfg", true);
            WRBInteractableConfig = new ConfigFile(Paths.ConfigPath + "\\BALLS.WellRoundedBalance.Interactables.cfg", true);
            WRBEnemyConfig = new ConfigFile(Paths.ConfigPath + "\\BALLS.WellRoundedBalance.Enemies.cfg", true);
            WRBEliteConfig = new ConfigFile(Paths.ConfigPath + "\\BALLS.WellRoundedBalance.Elites.cfg", true);
            WRBGamemodeConfig = new ConfigFile(Paths.ConfigPath + "\\BALLS.WellRoundedBalance.Gamemodes.cfg", true);

            InfernoLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("HIFU.Inferno");
            RiskyArtifactsLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.RiskyArtifacts");

            Molotov.Create();
            VoidBall.Create();

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

            foreach (Type type in enumerable2)
            {
                ItemBase based = (ItemBase)Activator.CreateInstance(type);
                if (ValidateItem(based))
                {
                    based.Init();
                    // i would really really like to sort everything alphabetically but i have no idea how, please help
                }
            }

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

            RemoveRollOfPenisAndGesture.Based();
            Mechanic.Monster.SpeedBoost.AddSpeedBoost();
            Mechanic.Bosses.BetterScaling.NerfHealthScaling();
        }

        public bool ValidateMechanic(MechanicBase gb)
        {
            if (gb.isEnabled)
            {
                bool enabledfr = WRBMechanicConfig.Bind(gb.Name, "Enable?", true, "Vanilla is false").Value;
                if (enabledfr) return true;
            }
            return false;
        }

        public bool ValidateItem(ItemBase ib)
        {
            if (ib.isEnabled)
            {
                bool enabledfr = WRBItemConfig.Bind(ib.Name, "Enable?", true, "Vanilla is false").Value;
                if (enabledfr) return true;
            }
            return false;
        }

        public bool ValidateEquipment(EquipmentBase eqb)
        {
            if (eqb.isEnabled)
            {
                bool enabledfr = WRBEquipmentConfig.Bind(eqb.Name, "Enable?", true, "Vanilla is false").Value;
                if (enabledfr) return true;
            }
            return false;
        }

        public bool ValidateInteractable(InteractableBase ib)
        {
            if (ib.isEnabled)
            {
                bool enabledfr = WRBInteractableConfig.Bind(ib.Name, "Enable?", true, "Vanilla is false").Value;
                if (enabledfr) return true;
            }
            return false;
        }

        public bool ValidateEnemy(EnemyBase enb)
        {
            if (enb.isEnabled)
            {
                bool enabledfr = WRBEnemyConfig.Bind(enb.Name, "Enable?", true, "Vanilla is false").Value;
                if (enabledfr) return true;
            }
            return false;
        }

        public bool ValidateElite(EliteBase elb)
        {
            if (elb.isEnabled)
            {
                bool enabledfr = WRBEliteConfig.Bind(elb.Name, "Enable?", true, "Vanilla is false").Value;
                if (enabledfr) return true;
            }
            return false;
        }

        public bool ValidateGamemode(GamemodeBase gmb)
        {
            if (gmb.isEnabled)
            {
                return true;
            }
            return false;
        }

        public static List<string> SortAlphabetically(List<string> input)
        {
            input.Sort();
            return input;

            // TODO - doesnt do anything currently, cant figure out a way to sort item names, etc alphabetically including the semicolons, so whites are first, then greens etc.
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