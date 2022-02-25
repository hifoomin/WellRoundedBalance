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
using UltimateCustomRun.Stages;
using UltimateCustomRun.Survivors;
using UnityEngine;

namespace UltimateCustomRun
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [R2APISubmoduleDependency(nameof(LanguageAPI), nameof(RecalculateStatsAPI), nameof(BuffAPI), nameof(LoadoutAPI), nameof(DirectorAPI))]
    public class Main : BaseUnityPlugin
    {
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "HIFU";
        public const string PluginName = "UltimateCustomRun";
        public const string PluginVersion = "1.0.0";

        public static ConfigFile UCRConfig;
        public static ManualLogSource UCRLogger;

        public void Awake()
        {
            UCRLogger = Logger;
            Main.UCRConfig = base.Config;

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
                }
            }

            IEnumerable<Type> enumerable3 = from type in Assembly.GetExecutingAssembly().GetTypes()
                                            where !type.IsAbstract && type.IsSubclassOf(typeof(EnemyBase))
                                            select type;

            UCRLogger.LogInfo("==+----------------==ENEMIES==----------------+==");

            foreach (Type type in enumerable3)
            {
                EnemyBase based = (EnemyBase)Activator.CreateInstance(type);
                if (ValidateEnemy(based))
                {
                    based.Init();
                }
            }

            Commencement.Changes();
            DistantRoost.AddCredits();
            SunderedGrove.AddGrovetender();
            SunderedGrove.RemoveClayDunestrider();
            TitanicPlains.AddBison();

            Loader.Changes();
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

        public static List<string> SortAlphabetically(List<string> input)
        {
            input.Sort();
            return input;

            // TODO - doesnt do anything currently, cant figure out a way to sort item names, etc alphabetically including the semicolons, so whites are first, then greens etc.
        }

        public static List<GameObject> projectilePrefabContent = new();
        public static List<Type> entityStateContent = new();
        public static List<SkillDef> skillDefContent = new();

        public static void RegisterType(Type t)
        {
            entityStateContent.Add(t);
        }

        private void UnusedOrNotWorkingOrTODO()
        {
            /*
             IL.RoR2.GlobalEventManager.OnInteractionBegin += SquidPolyp.ChangeAS;
             IL.RoR2.GlobalEventManager.OnInteractionBegin += SquidPolyp.ChangeLifetime;
             throws

             IL.RoR2.GlobalEventManager.OnCharacterDeath += CeremonialDagger.ChangeCount;

             IL.RoR2.GlobalEventManager.OnCharacterDeath += HappiestMask.ChangeChance;
             IL.RoR2.GlobalEventManager.OnCharacterDeath += HappiestMask.ChangeDuration;

             MonsterToothFlatHealing = Config.Bind<float>(":: Items : Whites :: Monster Tooth", "Flat Healing", (float)8f, "Vanilla is 8");
             MonsterToothPercentHealing = Config.Bind<float>(":: Items : Whites :: Monster Tooth", "Percent Healing", (float)0.02f, "Decimal. Per Stack. Vanilla is 0.02");

             StunGrenadeChance = Config.Bind<float>(":: Items : Whites :: Stun Grenade", "Chance", (float)5f, "Vanilla is 5");

            TopazBroochPercentBarrier = Config.Bind<float>(":: Items : Whites :: Topaz Brooch", "Percent Barrier", (float)0.1f, "Decimal. Per Stack. Vanilla is 0");
            TopazBroochPercentBarrierStack = Config.Bind<bool>(":: Items : Whites :: Topaz Brooch", "Increase Percent Barrier Gain Per Stack?", (bool)true, "Vanilla is false");

             SquidPolypDuration = Config.Bind<int>(":: Items :: Greens :: Squid Polyp", "Lifetime", (int)30, "Vanilla is 30");
             SquidPolypAS = Config.Bind<int>(":: Items :: Greens :: Squid Polyp", "Attack Speed Item Count", (int)10, "Per Stack. Vanilla is 10");

             DiosTTCount = Config.Bind<int>(":: Items ::: Reds :: Dios Best Friend", "Tougher Times Per Consumed Dios Count", (int)0, "Vanilla is 0");

             HappiestMaskDuration = Config.Bind<int>(":: Items ::: Reds :: Happiest Mask", "Ghost Lifetime", (int)30, "Per Stack. Vanilla is 30");
             HappiestMaskChance = Config.Bind<float>(":: Items ::: Reds :: Happiest Mask", "Chance", (float)0.07f, "Decimal. Vanilla is 0.07");

             LanguageAPI.Add("ITEM_TOOTH_DESC", "Killing an enemy spawns a <style=cIsHealing>Healing orb</style> that heals for <style=cIsHealing>" + MonsterToothFlatHealing.Value + "</style> plus an additional <style=cIsHealing>" + d(MonsterToothPercentHealing.Value) + " <style=cStack>(+" + d(MonsterToothPercentHealing.Value) + " per stack)</style></style> of <style=cIsHealing>maximum health</style>.");

             var wooliegaming = SquidPolypAS.Value * 10f;
             LanguageAPI.Add("ITEM_SQUIDTURRET_DESC", "Activating an interactable summons a <style=cIsDamage>Squid Turret</style> that attacks nearby enemies at <style=cIsDamage>" + wooliegaming + "% <style=cStack>(+" + wooliegaming + "% per stack)</style> attack Speed</style>. Lasts <style=cIsUtility>" + SquidPolypDuration.Value + "</style> seconds.");

             LanguageAPI.Add("ITEM_GHOSTONKILL_DESC", "Killing enemies has a <style=cIsDamage>" + d(HappiestMaskChance.Value) + "</style> Chance to <style=cIsDamage>spawn a ghost</style> of the killed enemy with <style=cIsDamage>1500%</style> Damage. Lasts <style=cIsDamage>" + HappiestMaskDuration.Value + "s</style> <style=cStack>(+" + HappiestMaskDuration.Value + "s per stack)</style>.");
            */
        }

        /*
        public static ConfigEntry<float> MonsterToothFlatHealing { get; set; }
        public static ConfigEntry<float> MonsterToothPercentHealing { get; set; }

        public static ConfigEntry<float> StunGrenadeChance { get; set; }

        public static ConfigEntry<float> TopazBroochPercentBarrier { get; set; }
        public static ConfigEntry<bool> TopazBroochPercentBarrierStack { get; set; }

        public static ConfigEntry<int> DiosTTCount { get; set; }

        public static ConfigEntry<int> HappiestMaskDuration { get; set; }
        public static ConfigEntry<float> HappiestMaskChance { get; set; }

        public static ConfigEntry<int> StridesCharges { get; set; }
        public static ConfigEntry<float> StridesCooldown { get; set; }
        public static ConfigEntry<float> StridesDuration { get; set; }
        public static ConfigEntry<float> StridesHealingPercent { get; set; }
        */
    }
}