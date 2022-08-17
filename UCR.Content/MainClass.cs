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

namespace UltimateCustomRun
{
    /* TODO:
     - Power Elixir, Delicate Watch, Genesis Loop and Old War Stealhkit health thresholds
     - Hopoo Feather counts as n of itself when jumping
     - Lepton Lily counts as n of itself when calculating pulse thresholds
     - Leeching Seed counts as n of itself when healing
     - Regenerating Scrap counts as n of itself when printing items
     - Bens Raincoat debuff prevention count
     - Bottled Chaos counts as n of itself when activating equip
     - Dio's Best Friend tougher times on death
     - Nkuhanas Opinion max pool, pool threshold and skull damage
     - Proper Pocket ICBM IL hook/rewrite
     - Defense Nucleus stat boosts
     - Halcyon Seed stat boosts
     - Mired Urn range, dps and healing
     - Queen's Gland beetle guard count
     - Beads of Fealty more scavs and more lunar coins
     - Essence of Heresy debuff duration, initial hit damage, explosion damage, cooldown
     - Focused Convergence charging speed, zone size change, charging on kill
     - Gesture of The Drowned random equipment cooldown on every equipment activation
     - Light Flux Pauldron cooldown and attack speed
     - Mercurial Rachis radius and damage
     - Shaped Glass damage, curse, linear damage
     - Stone Flux Pauldron health, move speed, mass
     - Strides of Heresy move speed, healing, duration, cooldown
     - Transcendence increasing shield recharge timer every stack
     - Needletick % and count for elites
     - Newly Hatched Zoea cooldown, max allies
     - Singularity Band damage, cooldown, range, detonation timer
     - Tentabauble debuff duration
     - Goobo Jr stat boosts
     - Milky Chrysalis flight speed, glide speed, boost speed and cooldown
    */

    [BepInDependency(R2API.R2API.PluginGUID)]
    [BepInDependency("com.rune580.riskofoptions")]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [R2APISubmoduleDependency(nameof(LanguageAPI), nameof(RecalculateStatsAPI), nameof(LoadoutAPI), nameof(DirectorAPI), nameof(PrefabAPI))]
    public class Main : BaseUnityPlugin
    {
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "HIFU";
        public const string PluginName = "UltimateCustomRun";
        public const string PluginVersion = "0.2.1";
        public static ConfigFile UCRConfig;
        public static ManualLogSource UCRLogger;

        public static AssetBundle UCR;
        public static ConfigEntry<bool> Dummy { get; set; }
        public static ConfigEntry<bool> Dummy2 { get; set; }
        public static ConfigEntry<bool> Dummy3 { get; set; }

        public void Awake()
        {
            UCRLogger = Logger;
            Main.UCRConfig = base.Config;

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

            // module/base init stuff below

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
            Generate.Awake();
        }

        public bool ValidateGlobal(GlobalBase gb)
        {
            if (gb.isEnabled)
            {
                bool enabledfr = Config.Bind(gb.Name, "Enable?", true, "Vanilla is false").Value;
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
                bool enabledfr = Config.Bind(ib.Name, "Enable?", true, "Vanilla is false").Value;
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
                bool enabledfr = Config.Bind(eb.Name, "Enable?", true, "Vanilla is false").Value;
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
                bool enabledfr = Config.Bind(eqb.Name, "Enable?", true, "Vanilla is false").Value;
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

        private void UnusedOrNotWorkingOrTODO()
        {
            /*
             MonsterToothFlatHealing = Config.Bind<float>(":: Items : Whites :: Monster Tooth", "Flat Healing", (float)8f, "Vanilla is 8");
             MonsterToothPercentHealing = Config.Bind<float>(":: Items : Whites :: Monster Tooth", "Percent Healing", (float)0.02f, "Decimal. Per Stack. Vanilla is 0.02");

             TopazBroochPercentBarrier = Config.Bind<float>(":: Items : Whites :: Topaz Brooch", "Percent Barrier", (float)0.1f, "Decimal. Per Stack. Vanilla is 0");
             TopazBroochPercentBarrierStack = Config.Bind<bool>(":: Items : Whites :: Topaz Brooch", "Increase Percent Barrier Gain Per Stack?", (bool)true, "Vanilla is false");

             DiosTTCount = Config.Bind<int>(":: Items ::: Reds :: Dios Best Friend", "Tougher Times Per Consumed Dios Count", (int)0, "Vanilla is 0");

            */
        }

        private void DoNothing()
        {
        }
    }
}