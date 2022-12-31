using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using R2API;
using R2API.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using WellRoundedBalance.Items;
using WellRoundedBalance.Equipment;
using WellRoundedBalance.Global;

namespace WellRoundedBalance
{
    [BepInDependency(R2API.R2API.PluginGUID)]
    /* swap to new R2APIs later
        Rework Defense Nucleus
        Rework Titanic Knurl
        Fix the commented out items (mostly me being lazy to remove the configs and shit)
        Tweak gold and time scaling (I literally just guesstimated lmao)
    */
    [BepInDependency("com.Wolfo.WolfoQualityOfLife", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.xoxfaby.BetterUI", BepInDependency.DependencyFlags.SoftDependency)]
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
        public static ManualLogSource WRBLogger;

        public void Awake()
        {
            WRBLogger = Logger;
            Main.WRBConfig = base.Config;

            IEnumerable<Type> enumerable = from type in Assembly.GetExecutingAssembly().GetTypes()
                                           where !type.IsAbstract && type.IsSubclassOf(typeof(GlobalBase))
                                           select type;

            WRBLogger.LogInfo("==+----------------==GLOBAL==----------------+==");

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
        }

        public bool ValidateGlobal(GlobalBase gb)
        {
            if (gb.isEnabled)
            {
                return true;
            }
            return false;
        }

        public bool ValidateItem(ItemBase ib)
        {
            if (ib.isEnabled)
            {
                return true;
            }
            return false;
        }

        public bool ValidateEquipment(EquipmentBase eqb)
        {
            if (eqb.isEnabled)
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
    }
}