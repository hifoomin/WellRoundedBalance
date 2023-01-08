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

namespace WellRoundedBalance
{
    [BepInDependency(R2API.R2API.PluginGUID)]
    /*  Swap to new R2APIs later
        Rework Defense Nucleus
        Rework Titanic Knurl
        Fix the commented out items (mostly me being lazy to remove the configs and shit)
        Make Lunar Pod give you the item directly >:)
        Implement Duh's void cradle idea
        Make it so you can only take a couple lunar coins per run (maybe like 6?)
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
        public static ConfigFile WRBGlobalConfig;
        public static ConfigFile WRBEquipmentConfig;
        public static ConfigFile WRBInteractableConfig;
        public static ConfigFile WRBEnemyConfig;
        public static ManualLogSource WRBLogger;

        public void Awake()
        {
            WRBLogger = Logger;
            Main.WRBConfig = base.Config;

            WRBItemConfig = new ConfigFile(Paths.ConfigPath + "\\BALLS.WellRoundedBalance.Items.cfg", true);
            WRBGlobalConfig = new ConfigFile(Paths.ConfigPath + "\\BALLS.WellRoundedBalance.Global.cfg", true);
            WRBEquipmentConfig = new ConfigFile(Paths.ConfigPath + "\\BALLS.WellRoundedBalance.Equipment.cfg", true);
            WRBInteractableConfig = new ConfigFile(Paths.ConfigPath + "\\BALLS.WellRoundedBalance.Interactables.cfg", true);
            WRBEnemyConfig = new ConfigFile(Paths.ConfigPath + "\\BALLS.WellRoundedBalance.Enemies.cfg", true);

            IEnumerable<Type> enumerable = from type in Assembly.GetExecutingAssembly().GetTypes()
                                           where !type.IsAbstract && type.IsSubclassOf(typeof(GlobalBase))
                                           select type;

            WRBLogger.LogInfo("==+----------------==MECHANICS==----------------+==");

            foreach (Type type in enumerable)
            {
                GlobalBase based = (GlobalBase)Activator.CreateInstance(type);
                if (ValidateGlobal(based))
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

            RemoveRollOfPenisAndGesture.Based();
        }

        public bool ValidateGlobal(GlobalBase gb)
        {
            if (gb.isEnabled)
            {
                bool enabledfr = WRBGlobalConfig.Bind(gb.Name, "Enable?", true, "Vanilla is false").Value;
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

        public bool ValidateEnemy(EnemyBase eb)
        {
            if (eb.isEnabled)
            {
                bool enabledfr = WRBEnemyConfig.Bind(eb.Name, "Enable?", true, "Vanilla is false").Value;
                if (enabledfr) return true;
            }
            return false;
        }

        public static List<string> SortAlphabetically(List<string> input)
        {
            input.Sort();
            return input;

            // TODO - doesnt do anything currently, cant figure out a way to sort item names, etc alphabetically including the semicolons, so whites are first, then greens etc.
        }
    }
}