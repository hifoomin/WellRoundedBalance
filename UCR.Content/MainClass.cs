using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using R2API;
using R2API.Utils;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using RiskOfOptions;
using RiskOfOptions.Options;
using UltimateCustomRun.Items;
using UltimateCustomRun.Equipment;
using UltimateCustomRun.BodyStatsSkills;
using UltimateCustomRun.Global;
using UltimateCustomRun.Elites;
using UltimateCustomRun.Directors;

namespace UltimateCustomRun
{
    /* TODO:
     - Genesis Loop and Old War Stealhkit health thresholds
     - Hopoo Feather counts as n of itself when jumping
     - Lepton Lily counts as n of itself when calculating pulse thresholds
     - Leeching Seed counts as n of itself when healing
     - Regenerating Scrap counts as n of itself when printing items
     - Bens Raincoat debuff prevention count
     - Bottled Chaos counts as n of itself when activating equip
     - Dio's Best Friend tougher times on death
     - Nkuhanas Opinion max pool, pool threshold and skull damage
     - Defense Nucleus stat boosts
     - Halcyon Seed stat boosts
     - Queen's Gland beetle guard count and stat boosts
     - Beads of Fealty more scavs and more lunar coins
     - Essence of Heresy debuff duration, initial hit damage, explosion damage
     - Gesture of The Drowned random equipment cooldown on every equipment activation
     - Light Flux Pauldron cooldown and attack speed
     - Strides of Heresy move speed, healing, duration
     - Transcendence increasing shield recharge timer every stack
     - Needletick % and count for elites
     - Newly Hatched Zoea cooldown, max allies
     - Goobo Jr stat boosts
     - Milky Chrysalis flight speed, glide speed, boost speed and cooldown
     - Topaz Brooch percent barrier
     - Rose Buckler updating sounds and visual for health threshold config
     - EmitDelegates for stacking stats on items that don't (e.g. atg, chronobauble)
     - Improve descriptions for Focused Convergence and Mercurial Rachis

     - Fix spikestrip adding duplicate elite key
     - Fix Sawmerang

     - Add a way to get all isc's somehow? And change their director stuff preferably per stage
     - Add a way to change monster cards stuff
     - Add a user friendly way to change stage spawn pools
    */

    [BepInDependency(R2API.R2API.PluginGUID)]
    // swap to new later
    [BepInDependency("com.rune580.riskofoptions")]
    [BepInDependency("com.Wolfo.WolfoQualityOfLife", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.xoxfaby.BetterUI", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("dev.ontrigger.itemstats", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("Withor.FixedDescriptions", BepInDependency.DependencyFlags.SoftDependency)] // may thy name shall not curse mine project
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [R2APISubmoduleDependency(nameof(LanguageAPI), nameof(RecalculateStatsAPI), nameof(LoadoutAPI), nameof(DirectorAPI), nameof(PrefabAPI), nameof(ItemAPI))]
    public class Main : BaseUnityPlugin
    {
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "HIFU";
        public const string PluginName = "UltimateCustomRun";
        public const string PluginVersion = "0.3.0";
        public static ConfigFile UCRConfig;
        public static ManualLogSource UCRLogger;
        public static ConfigFile UCREConfig;
        public static ConfigFile UCREQConfig;
        public static ConfigFile UCRIConfig;
        public static ConfigFile UCRGConfig;
        public static ConfigFile UCRDConfig;

        public static AssetBundle UCR;
        public static ConfigEntry<bool> Dummy { get; set; }
        public static ConfigEntry<bool> Dummy2 { get; set; }
        public static ConfigEntry<bool> Dummy3 { get; set; }

        public void Awake()
        {
            UCRLogger = Logger;
            Main.UCRConfig = base.Config;
            UCREConfig = new ConfigFile(Paths.ConfigPath + "\\HIFU.UltimateCustomRun.Elites.cfg", true);
            UCREQConfig = new ConfigFile(Paths.ConfigPath + "\\HIFU.UltimateCustomRun.Equipment.cfg", true);
            UCRIConfig = new ConfigFile(Paths.ConfigPath + "\\HIFU.UltimateCustomRun.Items.cfg", true);
            UCRGConfig = new ConfigFile(Paths.ConfigPath + "\\HIFU.UltimateCustomRun.Global.cfg", true);
            UCRDConfig = new ConfigFile(Paths.ConfigPath + "\\HIFU.UltimateCustomRun.Directors.cfg", true);
            UCR = AssetBundle.LoadFromFile(Assembly.GetExecutingAssembly().Location.Replace("UltimateCustomRun.dll", "ultimatecustomrun"));
            // load brundle
            ModSettingsManager.SetModIcon(UCR.LoadAsset<Sprite>("texUCRIcon.png"));
            Dummy = Config.Bind("__Important", "Multiplayer Stuff", true, "Make sure to have the same configs for multiplayer!");
            Dummy2 = Config.Bind("__Important", "Config Stuff", true, "Please note that the in-game config menu is just for quick adjustments, you can make finer and bigger adjustments in the mod's config directly.");
            Dummy3 = Config.Bind("__Important", "Runtime Stuff", true, "Please note that the vast majority of things don't update in realtime, instead you'll have to restart the game between config adjustments. This is for performance reasons.");
            // funnily enough, ROO wouldn't work without this, else the UltimateCustomRun tab would have no entries and it'd break ROO entirely
            ModSettingsManager.AddOption(new CheckBoxOption(Dummy));
            ModSettingsManager.AddOption(new CheckBoxOption(Dummy2));
            ModSettingsManager.AddOption(new CheckBoxOption(Dummy3));

            IEnumerable<Type> enumerable = from type in Assembly.GetExecutingAssembly().GetTypes()
                                           where !type.IsAbstract && type.IsSubclassOf(typeof(GlobalBase))
                                           select type;

            UCRLogger.LogInfo("==+----------------==GLOBAL==----------------+==");

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

            UCRLogger.LogInfo("==+----------------==ITEMS==----------------+==");

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

            UCRLogger.LogInfo("==+----------------==EQUIPMENT==----------------+==");

            foreach (Type type in enumerable3)
            {
                EquipmentBase based = (EquipmentBase)Activator.CreateInstance(type);
                if (ValidateEquipment(based))
                {
                    based.Init();
                }
            }

            IEnumerable<Type> enumerable4 = from type in Assembly.GetExecutingAssembly().GetTypes()
                                            where !type.IsAbstract && type.IsSubclassOf(typeof(EnemyBase))
                                            select type;

            UCRLogger.LogInfo("==+----------------==ENEMIES==----------------+==");

            foreach (Type type in enumerable4)
            {
                EnemyBase based = (EnemyBase)Activator.CreateInstance(type);
                if (ValidateEnemy(based))
                {
                    based.Init();
                }
            }

            IEnumerable<Type> enumerable5 = from type in Assembly.GetExecutingAssembly().GetTypes()
                                            where !type.IsAbstract && type.IsSubclassOf(typeof(DirectorBase))
                                            select type;

            UCRLogger.LogInfo("==+----------------==DIRECTORS==----------------+==");

            foreach (Type type in enumerable5)
            {
                DirectorBase based = (DirectorBase)Activator.CreateInstance(type);
                based.Init();
            }
            Generate.Awake();
            SendChatNotif.Send();
        }

        public bool ValidateGlobal(GlobalBase gb)
        {
            if (gb.isEnabled)
            {
                bool enabledfr = UCRGConfig.Bind(gb.Name, "Enable?", true, "Vanilla is false").Value;
                if (enabledfr)
                {
                    return true;
                }
            }
            return false;
        }

        public bool ValidateItem(ItemBase ib)
        {
            if (ib.isEnabled)
            {
                bool enabledfr = UCRIConfig.Bind(ib.Name, "Enable?", true, "Vanilla is false").Value;
                if (enabledfr)
                {
                    return true;
                }
            }
            return false;
        }

        public bool ValidateEnemy(EnemyBase eb)
        {
            if (eb.isEnabled)
            {
                bool enabledfr = UCREConfig.Bind(eb.Name, "Enable?", true, "Vanilla is false").Value;
                if (enabledfr)
                {
                    return true;
                }
            }
            return false;
        }

        public bool ValidateEquipment(EquipmentBase eqb)
        {
            if (eqb.isEnabled)
            {
                bool enabledfr = UCREQConfig.Bind(eqb.Name, "Enable?", true, "Vanilla is false").Value;
                if (enabledfr)
                {
                    return true;
                }
            }
            return false;
        }

        public static List<string> SortAlphabetically(List<string> input)
        {
            input.Sort();
            return input;

            // TODO - doesnt do anything currently, cant figure out a way to sort item names, etc alphabetically including the semicolons, so whites are first, then greens etc.
        }

        /*
        public static List<GameObject> projectilePrefabContent = new();
        public static List<Type> entityStateContent = new();
        public static List<SkillDef> skillDefContent = new();

        public static void RegisterType(Type t)
        {
            entityStateContent.Add(t);
        }
        */

        private void DoNothing()
        {
        }
    }
}