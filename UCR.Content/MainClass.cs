using BepInEx;
using R2API;
using R2API.Utils;
using BepInEx.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BepInEx.Logging;
using System;
using UltimateCustomRun.Survivors;
using UltimateCustomRun.Stages;
using UltimateCustomRun.Global;

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


        public static ConfigEntry<float> OspTime { get; set;}
        public static ConfigEntry<float> OspThreshold { get; set; }
        public static ConfigEntry<int> EquipmentChargeCap { get; set; }

        public static ConfigEntry<float> GlobalCritDamageMultiplier { get; set; }

        public static ConfigEntry<float> GlobalLowHealthThreshold { get; set; }

        public static ConfigEntry<float> LuckBase { get; set; }

        public static ConfigEntry<float> PlayerFactorBase { get; set; }
        public static ConfigEntry<float> PlayerCountMultiplier { get; set; }
        public static ConfigEntry<float> PlayerCountExponent { get; set; }
        public static ConfigEntry<bool> AdditiveStageScaling { get; set; }
        public static ConfigEntry<float> AdditiveStageScalingBase { get; set; }
        public static ConfigEntry<bool> ExponentialStageScaling { get; set; }
        public static ConfigEntry<float> ExponentialStageScalingCount { get; set; }
        public static ConfigEntry<float> ExponentialStageScalingBase { get; set; }
        public static ConfigEntry<float> TimeFactorMultiplier { get; set; }
        public static ConfigEntry<int> AmbientLevelCap { get; set; }
        public static ConfigEntry<float> GoldRewardExponent { get; set; }
        public static ConfigEntry<float> GoldRewardDivisorBase { get; set; }
        public static ConfigEntry<float> GoldRewardStageClearCountDivisor { get; set; }
        public static ConfigEntry<bool> Guide { get; set; }
        public static ConfigEntry<bool> Guide2 { get; set; }

        public static ConfigEntry<float> SharedDangerDelay { get; set; }
        public static ConfigEntry<float> SharedCombatDelay { get; set; }

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

        public static ConfigFile UCRConfig;
        public static ManualLogSource UCRLogger;

        public void Awake()
        {
            UCRLogger = Logger;
            Main.UCRConfig = base.Config;
            IEnumerable<Type> enumerable = from type in Assembly.GetExecutingAssembly().GetTypes()
                                           where !type.IsAbstract && type.IsSubclassOf(typeof(ItemBase))
                                           select type;

            UCRLogger.LogInfo("==+---------==ITEMS==----------------+==");

            foreach (Type type in enumerable)
            {
                ItemBase qased = (ItemBase)Activator.CreateInstance(type);
                qased.Init();
            }

            IEnumerable<Type> enumerable2 = from type in Assembly.GetExecutingAssembly().GetTypes()
                                           where !type.IsAbstract && type.IsSubclassOf(typeof(EnemyBase))
                                           select type;

            UCRLogger.LogInfo("==+---------==ENEMIES==----------------+==");

            foreach (Type type2 in enumerable2)
            {
                EnemyBase wased = (EnemyBase)Activator.CreateInstance(type2);
                wased.Init();
            }

            EquipmentChargeCap = Config.Bind<int>("::: Global :: Equipment :::", "Max Equipment Charges", (int)255, "Vanilla is 255");

            GlobalCritDamageMultiplier = Config.Bind<float>("::: Global : Damage :::", "Crit Damage Multiplier", (float)2f, "Vanilla is 2");

            GlobalLowHealthThreshold = Config.Bind<float>("::: Global ::: Health :::", "Low Health Threshold", (float)0.25f, "This affects Old War Stealthkit, Genesis Loop and the low health vignette. Vanilla is 0.25");
            OspTime = Config.Bind<float>("::: Global ::: Health :::", "One Shot Protection Time", 0.1f, "Vanilla is 0.1");
            OspThreshold = Config.Bind<float>("::: Global ::: Health :::", "One Shot Protection Threshold", 0.1f, "Decimal. Vanilla is 0.1");

            PlayerFactorBase = Config.Bind<float>("::: Global :::: Scaling :::", "Player Factor Base", (float)0.7f, "Vanilla is 0.7");
            PlayerCountMultiplier = Config.Bind<float>("::: Global :::: Scaling :::", "Player Count Multiplier", (float)0.3f, "Vanilla is 0.3");
            PlayerCountExponent = Config.Bind<float>("::: Global :::: Scaling :::", "Player Count Exponent", (float)0.2f, "Vanilla is 0.2");
            AdditiveStageScaling = Config.Bind<bool>("::: Global :::: Scaling :::", "Additive Stage Scaling?", (bool)false, "Vanilla is false");
            AdditiveStageScalingBase = Config.Bind<float>("::: Global :::: Scaling :::", "Additive Stage Scaling Adder", (float)0f, "Only works with Additive Stage Scaling enabled. Vanilla is 0");
            ExponentialStageScaling = Config.Bind<bool>("::: Global :::: Scaling :::", "Exponential Stage Scaling?", (bool)true, "Vanilla is true");
            ExponentialStageScalingCount = Config.Bind<float>("::: Global :::: Scaling :::", "Exponential Stage Scaling Count", (float)1f, "Every Nth Stage Scales the Difficulty Exponentially. Only works with Exponential Stage Scaling enabled. Vanilla is 1");
            ExponentialStageScalingBase = Config.Bind<float>("::: Global :::: Scaling :::", "Exponential Stage Scaling Base", (float)1.15f, "Only works with Exponential Stage Scaling enabled. Vanilla is 1.15");
            TimeFactorMultiplier = Config.Bind<float>("::: Global :::: Scaling :::", "Time Factor Multiplier", (float)0.0506f, "Vanilla is 0.0506");
            AmbientLevelCap = Config.Bind<int>("::: Global :::: Scaling :::", "Ambient Level Cap", (int)99, "Vanilla is 99");
            GoldRewardExponent = Config.Bind<float>("::: Global :::: Scaling :::", "Gold Reward Exponent", (float)1f, "Vanilla is 1");
            GoldRewardDivisorBase = Config.Bind<float>("::: Global :::: Scaling :::", "Gold Reward Divisor Base", (float)1f, "Vanilla is 1");
            GoldRewardStageClearCountDivisor = Config.Bind<float>("::: Global :::: Scaling :::", "Gold Reward Divisor Stage", (float)1f, "Vanilla is 1");
            Guide = Config.Bind<bool>("::: Global :::: Scaling :::", "Time Scaling Guide", (bool)true, "Time Scaling Formula:\n(Player Factor Base + Player Count * Player Count Multiplier + \nDifficulty Coefficient Multiplier * Difficulty Def Scaling Value \n(1 for Drizzle, 2 for Rainstorm, 3 for Monsoon) * \nPlayer Count ^ Player Count Exponent * \nTime in Minutes) * Exponential Stage Scaling Base ^ Stages Cleared \nI highly recommend changing Gold Scaling while changing these as well");
            Guide2 = Config.Bind<bool>("::: Global :::: Scaling :::", "Gold Scaling Guide", (bool)true, "Gold Scaling Formula:\n(Gold Reward ^ Gold Reward Exponent) / Gold Reward Divisor Base ^ \n(Stage Clear Count / Gold Reward Divisor Stage)");

            SharedCombatDelay = Config.Bind<float>(":: Items Shared ::", "Activation Delay in Combat", (float)5f, "This affects Red Whip and probably something else. Vanilla is 5");
            SharedDangerDelay = Config.Bind<float>(":: Items Shared ::", "Activation Delay in Danger", (float)7f, "This affects Shields and Cautious Slug. Vanilla is 7");

            // MonsterToothFlatHealing = Config.Bind<float>(":: Items : Whites :: Monster Tooth", "Flat Healing", (float)8f, "Vanilla is 8");
            // MonsterToothPercentHealing = Config.Bind<float>(":: Items : Whites :: Monster Tooth", "Percent Healing", (float)0.02f, "Decimal. Per Stack. Vanilla is 0.02");

            // StunGrenadeChance = Config.Bind<float>(":: Items : Whites :: Stun Grenade", "Chance", (float)5f, "Vanilla is 5");

            //TopazBroochPercentBarrier = Config.Bind<float>(":: Items : Whites :: Topaz Brooch", "Percent Barrier", (float)0.1f, "Decimal. Per Stack. Vanilla is 0");
            //TopazBroochPercentBarrierStack = Config.Bind<bool>(":: Items : Whites :: Topaz Brooch", "Increase Percent Barrier Gain Per Stack?", (bool)true, "Vanilla is false");

            // SquidPolypDuration = Config.Bind<int>(":: Items :: Greens :: Squid Polyp", "Lifetime", (int)30, "Vanilla is 30");
            // SquidPolypAS = Config.Bind<int>(":: Items :: Greens :: Squid Polyp", "Attack Speed Item Count", (int)10, "Per Stack. Vanilla is 10");

            // DiosTTCount = Config.Bind<int>(":: Items ::: Reds :: Dios Best Friend", "Tougher Times Per Consumed Dios Count", (int)0, "Vanilla is 0");

            // HappiestMaskDuration = Config.Bind<int>(":: Items ::: Reds :: Happiest Mask", "Ghost Lifetime", (int)30, "Per Stack. Vanilla is 30");
            // HappiestMaskChance = Config.Bind<float>(":: Items ::: Reds :: Happiest Mask", "Chance", (float)0.07f, "Decimal. Vanilla is 0.07");

            IL.RoR2.HealthComponent.TriggerOneShotProtection += OneShotProtection.ChangeTime;
            IL.RoR2.CharacterBody.RecalculateStats += OneShotProtection.ChangeThreshold;

            IL.RoR2.HealthComponent.TakeDamage += CritMultiplier.ChangeDamage;

            Scaling.ChangeBehavior();

            // IL.RoR2.HealthComponent.something += IsHealthLow.ChangeThreshold;
            // apparently i have to generate a hook via a reflection because mmhook doesnt have a hook on this

            IL.RoR2.CharacterBody.FixedUpdate += Shared.ChangeCombatDelay;
            IL.RoR2.CharacterBody.FixedUpdate += Shared.ChangeDangerDelay;


            // IL.RoR2.GlobalEventManager.OnInteractionBegin += SquidPolyp.ChangeAS;
            // IL.RoR2.GlobalEventManager.OnInteractionBegin += SquidPolyp.ChangeLifetime;
            // throws

            // IL.RoR2.GlobalEventManager.OnCharacterDeath += CeremonialDagger.ChangeCount;

            // IL.RoR2.GlobalEventManager.OnCharacterDeath += HappiestMask.ChangeChance;
            // IL.RoR2.GlobalEventManager.OnCharacterDeath += HappiestMask.ChangeDuration;
            // fuck

            CeremonialDagger.ChangeProc();

            DefensiveMicrobots.Changes();

            FrostRelic.Changes();

            Headstompers.Changes();

            Commencement.Changes();
            DistantRoost.AddCredits();
            SunderedGrove.AddGrovetender();
            SunderedGrove.RemoveClayDunestrider();
            TitanicPlains.AddBison();
        }

        public static List<string> SortAlphabetically(List<string> input)
        {
            input.Sort();
            return input;
        }

        private void ChangeDescriptions()
        {

            // LanguageAPI.Add("ITEM_TOOTH_DESC", "Killing an enemy spawns a <style=cIsHealing>healing orb</style> that heals for <style=cIsHealing>" + MonsterToothFlatHealing.Value + "</style> plus an additional <style=cIsHealing>" + d(MonsterToothPercentHealing.Value) + " <style=cStack>(+" + d(MonsterToothPercentHealing.Value) + " per stack)</style></style> of <style=cIsHealing>maximum health</style>.");

            // var wooliegaming = SquidPolypAS.Value * 10f;
            // LanguageAPI.Add("ITEM_SQUIDTURRET_DESC", "Activating an interactable summons a <style=cIsDamage>Squid Turret</style> that attacks nearby enemies at <style=cIsDamage>" + wooliegaming + "% <style=cStack>(+" + wooliegaming + "% per stack)</style> attack speed</style>. Lasts <style=cIsUtility>" + SquidPolypDuration.Value + "</style> seconds.");

            // LanguageAPI.Add("ITEM_GHOSTONKILL_DESC", "Killing enemies has a <style=cIsDamage>" + d(HappiestMaskChance.Value) + "</style> chance to <style=cIsDamage>spawn a ghost</style> of the killed enemy with <style=cIsDamage>1500%</style> damage. Lasts <style=cIsDamage>" + HappiestMaskDuration.Value + "s</style> <style=cStack>(+" + HappiestMaskDuration.Value + "s per stack)</style>.");
        }
    }
}
