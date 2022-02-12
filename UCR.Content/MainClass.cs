using BepInEx;
using R2API;
using R2API.Utils;
using RoR2;
using UnityEngine;
using BepInEx.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BepInEx.Logging;
using System;
using UltimateCustomRun.Enemies;
using UltimateCustomRun.Enemies.Bosses;
using UltimateCustomRun.Survivors;
using UltimateCustomRun.Stages;

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

        //           _     _ _            
        //          | |   (_) |           
        // __      _| |__  _| |_ ___  ___ 
        // \ \ /\ / / '_ \| | __/ _ \/ __|
        //  \ V  V /| | | | | ||  __/\__ \
        //   \_/\_/ |_| |_|_|\__\___||___/
        //

        public static ConfigEntry<float> MonsterToothFlatHealing { get; set; }
        public static ConfigEntry<float> MonsterToothPercentHealing { get; set; }

        public static ConfigEntry<float> StunGrenadeChance { get; set; }

        public static ConfigEntry<float> TopazBroochPercentBarrier { get; set; }
        public static ConfigEntry<bool> TopazBroochPercentBarrierStack { get; set; }

        //   __ _ _ __ ___  ___ _ __  ___ 
        //  / _` | '__/ _ \/ _ \ '_ \/ __|
        // | (_| | | |  __/  __/ | | \__ \
        //  \__, |_|  \___|\___|_| |_|___/
        //   __/ |                        
        //  |___/   

        //               _     
        //              | |    
        //  _ __ ___  __| |___ 
        // | '__/ _ \/ _` / __|
        // | | |  __/ (_| \__ \
        // |_|  \___|\__,_|___/

        public static ConfigEntry<float> BehemothDamage { get; set; }
        public static ConfigEntry<float> BehemothAoe { get; set; }
        public static ConfigEntry<float> BehemothAoeStack { get; set; }

        public static ConfigEntry<float> BrainstalksDuration { get; set; }

        public static ConfigEntry<float> CeremonialDamage { get; set; }
        public static ConfigEntry<int> CeremonialCount { get; set; }
        public static ConfigEntry<float> CeremonialProcCo { get; set; }

        public static ConfigEntry<float> DefeMicroMinFireFreq { get; set; }
        public static ConfigEntry<float> DefeMicroRechargeFreq { get; set; }
        public static ConfigEntry<float> DefeMicroRange { get; set; }
        public static ConfigEntry<bool> DefeMicroGuide { get; set; }

        public static ConfigEntry<int> DiosTTCount { get; set; }

        public static ConfigEntry<float> FrostRelicAS { get; set; }
        public static ConfigEntry<float> FrostRelicBaseRadius { get; set; }
        public static ConfigEntry<float> FrostRelicRadiusPerKill { get; set; }
        public static ConfigEntry<float> FrostRelicDamage { get; set; }
        public static ConfigEntry<float> FrostRelicDuration { get; set; }
        public static ConfigEntry<float> FrostRelicProcCo { get; set; }
        public static ConfigEntry<int> FrostRelicMax { get; set; }
        public static ConfigEntry<int> FrostRelicMaxStack { get; set; }
        public static ConfigEntry<bool> FrostRelicGuide { get; set; }

        public static ConfigEntry<float> HeadstompersCooldown { get; set; }
        public static ConfigEntry<float> HeadstompersJumpHeight { get; set; }
        public static ConfigEntry<float> HeadstompersMinRange { get; set; }
        public static ConfigEntry<float> HeadstompersMaxRange { get; set; }
        public static ConfigEntry<float> HeadstompersMinDamage { get; set; }
        public static ConfigEntry<float> HeadstompersMaxDamage { get; set; }

        public static ConfigEntry<int> HappiestMaskDuration { get; set; }
        public static ConfigEntry<float> HappiestMaskChance { get; set; }

        public static ConfigEntry<int> HardlightCharges { get; set; }
        public static ConfigEntry<float> HardlightCDR { get; set; }

        //  _                            
        // | |                           
        // | |_   _ _ __   __ _ _ __ ___ 
        // | | | | | '_ \ / _` | '__/ __|
        // | | |_| | | | | (_| | |  \__ \
        // |_|\__,_|_| |_|\__,_|_|  |___/

        public static ConfigEntry<int> VisionsCharges { get; set; }
        public static ConfigEntry<float> VisionsCooldown { get; set; }
        public static ConfigEntry<float> VisionsInitialHitDamage { get; set; }
        public static ConfigEntry<float> VisionsInitialProcCoefficient { get; set; }

        public static ConfigEntry<int> HooksCharges { get; set; }
        public static ConfigEntry<float> HooksCooldown { get; set; }
        public static ConfigEntry<float> HooksExplosionDamage { get; set; }
        public static ConfigEntry<float> HooksExplosionProcCoefficient { get; set; }
        public static ConfigEntry<float> HooksExplosionRadius { get; set; }
        public static ConfigEntry<float> HooksProcCoefficient { get; set; }
        
        public static ConfigEntry<int> StridesCharges { get; set; }
        public static ConfigEntry<float> StridesCooldown { get; set; }
        public static ConfigEntry<float> StridesDuration { get; set; }
        public static ConfigEntry<float> StridesHealingPercent { get; set; }

        public static ConfigEntry<bool> BeetleQueenChanges { get; set; }
        public static ConfigEntry<bool> ClayDunestriderChanges { get; set; }
        public static ConfigEntry<bool> GrandparentChanges { get; set; }
        public static ConfigEntry<bool> GrovetenderChanges { get; set; }
        public static ConfigEntry<bool> ImpOverlordChanges { get; set; }
        public static ConfigEntry<bool> MithrixPhase1And3Changes { get; set; }
        public static ConfigEntry<bool> MithrixPhase2Changes { get; set; }
        public static ConfigEntry<bool> MithrixPhase4Changes { get; set; }
        public static ConfigEntry<bool> MithrixPhase5Changes { get; set; }
        public static ConfigEntry<bool> StoneTitanChanges { get; set; }
        public static ConfigEntry<bool> BeetleChanges { get; set; }
        public static ConfigEntry<bool> BighornBisonChanges { get; set; }
        public static ConfigEntry<bool> GolemChanges { get; set; }
        public static ConfigEntry<bool> GreaterWispChanges { get; set; }
        public static ConfigEntry<bool> ImpChanges { get; set; }
        public static ConfigEntry<bool> LemurianChanges { get; set; }
        public static ConfigEntry<bool> LesserWispChanges { get; set; }
        public static ConfigEntry<bool> LunarExploderChanges { get; set; }

        public static ConfigEntry<bool> LunarWispChanges { get; set; }
        public static ConfigEntry<bool> VoidReaverChanges { get; set; }

        public static ConfigFile UCRConfig;
        public static ManualLogSource UCRLogger;

        public void Awake()
        {
            UCRLogger = Logger;
            Main.UCRConfig = base.Config;
            IEnumerable<Type> enumerable = from type in Assembly.GetExecutingAssembly().GetTypes()
                                           where !type.IsAbstract && type.IsSubclassOf(typeof(Based))
                                           select type;
            foreach (Type type2 in enumerable)
            {
                Based based = (Based)Activator.CreateInstance(type2);
                based.Init();
            }

            BeetleQueenChanges = Config.Bind<bool>(":::: Enemies ::: Beetle Queen ::::", "Enable changes?", true, "This makes her less sluggish. Vanilla is false");
            ClayDunestriderChanges = Config.Bind<bool>(":::: Enemies ::: Clay Dunestrider ::::", "Enable changes?", true, "Does nothing currently. Vanilla is false");
            GrandparentChanges = Config.Bind<bool>(":::: Enemies ::: Grandparent ::::", "Enable changes?", true, "This makes him less sluggish. Vanilla is false");
            GrovetenderChanges = Config.Bind<bool>(":::: Enemies ::: Grovetender ::::", "Enable changes?", true, "This makes him less sluggish and enables an unused skill. Vanilla is false");
            ImpOverlordChanges = Config.Bind<bool>(":::: Enemies ::: Imp Overlord ::::", "Enable changes?", true, "This makes him less sluggish and a bit smarter. Vanilla is false");
            MithrixPhase1And3Changes = Config.Bind<bool>(":::: Enemies ::: Mithrix Phase 1 and 3 ::::", "Enable changes?", true, "This makes him faster and more threatening. Especially Phase 3. Vanilla is false");
            MithrixPhase2Changes = Config.Bind<bool>(":::: Enemies ::: Mithrix Phase 2 ::::", "Enable changes?", true, "Does nothing currently. Vanilla is false");
            MithrixPhase4Changes = Config.Bind<bool>(":::: Enemies ::: Mithrix Phase 4 ::::", "Enable changes?", true, "This makes him less sluggish and much more threatening. Might be a little buggy... Vanilla is false");
            MithrixPhase5Changes = Config.Bind<bool>(":::: Enemies ::: Mithrix Phase 5 ::::", "Enable changes?", true, "This is the escape sequence lines. Enables unused rotation, makes them move and they are generally spammier. Vanilla is false");
            StoneTitanChanges = Config.Bind<bool>(":::: Enemies ::: Stone Titan ::::", "Enable changes?", true, "This makes him less sluggish. Vanilla is false");
            BeetleChanges = Config.Bind<bool>(":::: Enemies :: Beetle ::::", "Enable changes?", true, "This makes them less sluggish, and enables a dash. Vanilla is false");
            BighornBisonChanges = Config.Bind<bool>(":::: Enemies :: Bighorn Bison ::::", "Enable changes?", true, "This makes them less sluggish and turn much faster. Vanilla is false");
            GolemChanges = Config.Bind<bool>(":::: Enemies :: Stone Golem ::::", "Enable changes?", true, "This makes them less sluggish and slightly nerfs their damage. Vanilla is false");
            GreaterWispChanges = Config.Bind<bool>(":::: Enemies :: Greater Wisp ::::", "Enable changes?", true, "This makes them less sluggish. Vanilla is false");
            ImpChanges = Config.Bind<bool>(":::: Enemies :: Imp ::::", "Enable changes?", true, "This makes them less sluggish and a bit smarter. Vanilla is false");
            LemurianChanges = Config.Bind<bool>(":::: Enemies :: Lemurian ::::", "Enable changes?", true, "This makes them less sluggish and a bit smarter. Vanilla is false");
            LesserWispChanges = Config.Bind<bool>(":::: Enemies :: Lesser Wisp ::::", "Enable changes?", true, "This makes them less sluggish. Vanilla is false");
            LunarExploderChanges = Config.Bind<bool>(":::: Enemies :: Lunar Exploder ::::", "Enable changes?", true, "This makes them less sluggish. Vanilla is false");
            LunarWispChanges = Config.Bind<bool>(":::: Enemies :: Lunar Wisp ::::", "Enable changes?", true, "This makes them more sluggish. Vanilla is false");
            VoidReaverChanges = Config.Bind<bool>(":::: Enemies :: Void Reaver ::::", "Enable changes?", true, "This makes them more aggressive. Vanilla is false");


            //        _       _           _ 
            //       | |     | |         | |
            //   __ _| | ___ | |__   __ _| |
            //  / _` | |/ _ \| '_ \ / _` | |
            // | (_| | | (_) | |_) | (_| | |
            //  \__, |_|\___/|_.__/ \__,_|_|
            //   __/ |                      
            //  |___/ 

            EquipmentChargeCap = Config.Bind<int>("::: Global :: Equipment :::", "Max Equipment Charges", (int)255, "Vanilla is 255");

            GlobalCritDamageMultiplier = Config.Bind<float>("::: Global : Damage :::", "Crit Damage Multiplier", (float)2f, "Vanilla is 2");

            GlobalLowHealthThreshold = Config.Bind<float>("::: Global ::: Health :::", "Low Health Threshold", (float)0.25f, "This affects Old War Stealthkit, Genesis Loop and the low health vignette. Vanilla is 0.25");

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

            //           _     _ _            
            //          | |   (_) |           
            // __      _| |__  _| |_ ___  ___ 
            // \ \ /\ / / '_ \| | __/ _ \/ __|
            //  \ V  V /| | | | | ||  __/\__ \
            //   \_/\_/ |_| |_|_|\__\___||___/

            // MonsterToothFlatHealing = Config.Bind<float>(":: Items : Whites :: Monster Tooth", "Flat Healing", (float)8f, "Vanilla is 8");
            // MonsterToothPercentHealing = Config.Bind<float>(":: Items : Whites :: Monster Tooth", "Percent Healing", (float)0.02f, "Decimal. Per Stack. Vanilla is 0.02");

            // StunGrenadeChance = Config.Bind<float>(":: Items : Whites :: Stun Grenade", "Chance", (float)5f, "Vanilla is 5");

            //TopazBroochPercentBarrier = Config.Bind<float>(":: Items : Whites :: Topaz Brooch", "Percent Barrier", (float)0.1f, "Decimal. Per Stack. Vanilla is 0");
            //TopazBroochPercentBarrierStack = Config.Bind<bool>(":: Items : Whites :: Topaz Brooch", "Increase Percent Barrier Gain Per Stack?", (bool)true, "Vanilla is false");

            //   __ _ _ __ ___  ___ _ __  ___ 
            //  / _` | '__/ _ \/ _ \ '_ \/ __|
            // | (_| | | |  __/  __/ | | \__ \
            //  \__, |_|  \___|\___|_| |_|___/
            //   __/ |                        
            //  |___/   

            // SquidPolypDuration = Config.Bind<int>(":: Items :: Greens :: Squid Polyp", "Lifetime", (int)30, "Vanilla is 30");
            // SquidPolypAS = Config.Bind<int>(":: Items :: Greens :: Squid Polyp", "Attack Speed Item Count", (int)10, "Per Stack. Vanilla is 10");


            //               _     
            //              | |    
            //  _ __ ___  __| |___ 
            // | '__/ _ \/ _` / __|
            // | | |  __/ (_| \__ \
            // |_|  \___|\__,_|___/

            BehemothDamage = Config.Bind<float>(":: Items ::: Reds :: Brilliant Behemoth", "Damage", (float)0.6f, "Vanilla is 0.6");
            BehemothAoe = Config.Bind<float>(":: Items ::: Reds :: Brilliant Behemoth", "Base AoE", (float)1.5f, "V. Vanilla is 1.5");
            BehemothAoeStack = Config.Bind<float>(":: Items ::: Reds :: Brilliant Behemoth", "AoE", (float)2.5f, "V. Per Stack. Vanilla is 2.5");

            BrainstalksDuration = Config.Bind<float>(":: Items ::: Reds :: Brainstalks", "Duration", (float)4f, "Per Stack. Vanilla is 4");

            // CeremonialCount = Config.Bind<int>(":: Items ::: Reds :: Ceremonial Dagger", "Count", (int)3, "Vanilla is 3");
            CeremonialDamage = Config.Bind<float>(":: Items ::: Reds :: Ceremonial Dagger", "Damage", (float)1.5f, "Per Stack. Vanilla is 1.5");
            CeremonialProcCo = Config.Bind<float>(":: Items ::: Reds :: Ceremonial Dagger", "Proc Coefficient", (float)1f, "Vanilla is 1");

            DefeMicroMinFireFreq = Config.Bind<float>(":: Items ::: Reds :: Defensive Microbots", "Minimum Fire Frequency", (float)10f, "Vanilla is 10");
            DefeMicroRechargeFreq = Config.Bind<float>(":: Items ::: Reds :: Defensive Microbots", "Recharge Frequency", (float)2f, "Vanilla is 2");
            DefeMicroGuide = Config.Bind<bool>(":: Items ::: Reds :: Defensive Microbots", "Recharge Guide", (bool)true, "Recharge Frequency Formula:\n1 / (The lower value between Minimum Fire Frequency and Recharge Frequency)");
            DefeMicroRange = Config.Bind<float>(":: Items ::: Reds :: Defensive Microbots", "Range", (float)20f, "Vanilla is 20");

            DiosTTCount = Config.Bind<int>(":: Items ::: Reds :: Dios Best Friend", "Tougher Times Per Consumed Dios Count", (int)0, "Vanilla is 0");

            FrostRelicAS = Config.Bind<float>(":: Items ::: Reds :: Frost Relic", "Attack Interval", (float)0.25f, "Vanilla is 0.25");
            FrostRelicBaseRadius = Config.Bind<float>(":: Items ::: Reds :: Frost Relic", "Base Radius", (float)6f, "Vanilla is 6");
            FrostRelicRadiusPerKill = Config.Bind<float>(":: Items ::: Reds :: Frost Relic", "Radius", (float)2f, "Per Icicle. Vanilla is 2");
            FrostRelicDamage = Config.Bind<float>(":: Items ::: Reds :: Frost Relic", "Damage", (float)3f, "Decimal. Per Tick. Vanilla is 3");
            FrostRelicDuration = Config.Bind<float>(":: Items ::: Reds :: Frost Relic", "Duration", (float)5f, "Vanilla is 5");
            FrostRelicProcCo = Config.Bind<float>(":: Items ::: Reds :: Frost Relic", "Proc Coefficient", (float)0.2f, "Per Tick. Vanilla is 0.2");
            FrostRelicMax = Config.Bind<int>(":: Items ::: Reds :: Frost Relic", "Base Icicle Cap", (int)6, "V. Vanilla is 6");
            FrostRelicMaxStack = Config.Bind<int>(":: Items ::: Reds :: Frost Relic", "Icicle Cap", (int)6, "V. Per Stack. Vanilla is 6");
            FrostRelicGuide = Config.Bind<bool>(":: Items ::: Reds :: Frost Relic", "Frost Relic Guide", (bool)true, "Frost Relic Formulas:\n1 / Attack Interval = Attacks per Second\nBase Radius * (Base Icicle Cap + Icicle Cap) = Max Radius\nDamage * (1 / Attack Interval) = DPS\nProc Coefficient * (1 / Attack Interval) = PPS");

            HeadstompersCooldown = Config.Bind<float>(":: Items ::: Reds :: H3AD-5T v2", "Cooldown", (float)1f, "Vanilla is 10");
            HeadstompersJumpHeight = Config.Bind<float>(":: Items ::: Reds :: H3AD-5T v2", "Jump Height Multiplier", (float)5f, "Vanilla is 2");
            HeadstompersMinRange = Config.Bind<float>(":: Items ::: Reds :: H3AD-5T v2", "Minimum Range", (float)5f, "Vanilla is 5");
            HeadstompersMaxRange = Config.Bind<float>(":: Items ::: Reds :: H3AD-5T v2", "Maximum Range", (float)100f, "Vanilla is 100");
            HeadstompersMinDamage = Config.Bind<float>(":: Items ::: Reds :: H3AD-5T v2", "Minimum Damage", (float)10f, "Decimal. Vanilla is 10");
            HeadstompersMaxDamage = Config.Bind<float>(":: Items ::: Reds :: H3AD-5T v2", "Maximum Damage", (float)100f, "Decimal. Vanilla is 100");

            HappiestMaskDuration = Config.Bind<int>(":: Items ::: Reds :: Happiest Mask", "Ghost Lifetime", (int)30, "Per Stack. Vanilla is 30");
            HappiestMaskChance = Config.Bind<float>(":: Items ::: Reds :: Happiest Mask", "Chance", (float)0.07f, "Decimal. Vanilla is 0.07");

            HardlightCharges = Config.Bind<int>(":: Items ::: Reds :: Hardlight Afterburner", "Charges", (int)2, "Per Stack. Vanilla is 2");
            HardlightCDR = Config.Bind<float>(":: Items ::: Reds :: Hardlight Afterburner", "Cooldown Reduction", (float)0.3333334f, "Decimal. Vanilla is 0.3333334");

            //  _                            
            // | |                           
            // | |_   _ _ __   __ _ _ __ ___ 
            // | | | | | '_ \ / _` | '__/ __|
            // | | |_| | | | | (_| | |  \__ \
            // |_|\__,_|_| |_|\__,_|_|  |___/

            //VisionsCharges = Config.Bind<int>(":: Items ::::: Lunars ::", "Visions of Heresy Charges", (int)24, "The amount of Charges of Visions of Heresy, Vanilla is 12");
            //VisionsCooldown = Config.Bind<float>(":: Items ::::: Lunars ::", "Visions of Heresy Cooldown", (float)1f, "The Cooldown of Visions of Heresy, Vanilla is 2");
            //VisionsInitialHitDamage = Config.Bind<float>(":: Items ::::: Lunars ::", "Visions of Heresy Intial Damage", (float)0.05f, "Decimal. The Initial Damage per hit of Visions of Heresy, Vanilla is 0.05");
            //VisionsInitialProcCoefficient = Config.Bind<float>(":: Items ::::: Lunars ::", "Visions of Heresy Intial Proc Coefficient", (float)0.1f, "Decimal. The Initial Proc Coefficient per hit of Visions of Heresy, Vanilla is 0.1");

            //HooksCharges = Config.Bind<int>(":: Items ::::: Lunars ::", "Hooks of Heresy Charges", (int)1, "The amount of Charges of Hooks of Heresy, Vanilla is 1");
            //HooksCooldown = Config.Bind<float>(":: Items ::::: Lunars ::", "Hooks of Heresy Cooldown", (float)5f, "The Cooldown of Hooks of Heresy, Vanilla is 5");
            //HooksProcCoefficient = Config.Bind<float>(":: Items ::::: Lunars ::", "Hooks of Heresy Proc Coefficient", (float)0.2f, "Decimal. The Proc Coefficient of Hooks of Heresy, Vanilla is 0.2");
            //HooksExplosionProcCoefficient = Config.Bind<float>(":: Items ::::: Lunars ::", "Hooks of Heresy Explosion Proc Coefficient", (float)1f, "Decimal. The Proc Coefficient of Hooks of Heresy's Explosion, Vanilla is 1");
            //HooksExplosionRadius = Config.Bind<float>(":: Items ::::: Lunars ::", "Hooks of Heresy Explosion Radius", (float)14f, "The Radius of Hooks of Heresy's Explosion, Vanilla is 14");
            //HooksExplosionDamage = Config.Bind<float>(":: Items ::::: Lunars ::", "Hooks of Heresy Explosion Damage", (float)7f, "Decimal. The Damage of Hooks of Heresy's Explosion, Vanilla is 7.");

            //StridesCharges = Config.Bind<int>(":: Items :::: Lunars ::", "Strides of Heresy Charges", (int)1, "The amount of Charges of Strides of Heresy, Vanilla is 1");
            //StridesCooldown = Config.Bind<float>(":: Items :::: Lunars ::", "Strides of Heresy Cooldown", (float)6f, "The Cooldown of Strides of Heresy, Vanilla is 6");
            //StridesDuration = Config.Bind<float>(":: Items :::: Lunars ::", "Strides of Heresy Duration", (float)3f, "The Duration of Strides of Heresy, Vanilla is 3");
            //StridesHealingPercent = Config.Bind<float>(":: Items :::: Lunars ::", "Strides of Heresy Healing Per Tick", (float)0.013, "Decimal. The Healing Per Tick of Charges of Strides of Heresy, Vanilla is 0.013");

            ChangeDescriptions();

            //        _       _           _ 
            //       | |     | |         | |
            //   __ _| | ___ | |__   __ _| |
            //  / _` | |/ _ \| '_ \ / _` | |
            // | (_| | | (_) | |_) | (_| | |
            //  \__, |_|\___/|_.__/ \__,_|_|
            //   __/ |                      
            //  |___/   

            IL.RoR2.HealthComponent.TakeDamage += CritMultiplier.ChangeDamage;

            Scaling.ChangeBehavior();

            // IL.RoR2.HealthComponent.something += IsHealthLow.ChangeThreshold;
            // apparently i have to generate a hook via a reflection because mmhook doesnt have a hook on this

            IL.RoR2.CharacterBody.FixedUpdate += Shared.ChangeCombatDelay;
            IL.RoR2.CharacterBody.FixedUpdate += Shared.ChangeDangerDelay;

            //           _     _ _            
            //          | |   (_) |           
            // __      _| |__  _| |_ ___  ___ 
            // \ \ /\ / / '_ \| | __/ _ \/ __|
            //  \ V  V /| | | | | ||  __/\__ \
            //   \_/\_/ |_| |_|_|\__\___||___/

            // IL.RoR2.GlobalEventManager.OnCharacterDeath += MonsterTooth.ChangeHealing;
            // throws

            IL.RoR2.CharacterBody.RecalculateStats += PaulsGoatHoof.ChangeSpeed;

            IL.RoR2.CharacterBody.RecalculateStats += PersonalShieldGenerator.ChangeShieldPercent;

            IL.RoR2.HealthComponent.TakeDamage += RepulsionArmorPlate.ChangeReduction;
            IL.RoR2.HealthComponent.TakeDamage += RepulsionArmorPlate.ChangeMinimum;

            IL.RoR2.CharacterBody.RecalculateStats += SoldiersSyringe.ChangeAS;

            IL.RoR2.GlobalEventManager.OnHitEnemy += StickyBomb.ChangeChance;

            // IL.RoR2.SetStateOnHurt.OnTakeDamageServer += StunGrenade.ChangeBehavior;
            // IL.RoR2.SetStateOnHurt += StunGrenade.ChangeChance;
            // seems like another reflection needed? help idk how

            IL.RoR2.GlobalEventManager.OnCharacterDeath += TopazBrooch.ChangeBarrier;

            IL.RoR2.HealthComponent.TakeDamage += TougherTimes.ChangeBlock;

            IL.RoR2.GlobalEventManager.OnHitEnemy += TriTipDagger.ChangeChance;
            IL.RoR2.GlobalEventManager.OnHitEnemy += TriTipDagger.ChangeDuration;

            IL.RoR2.TeleporterInteraction.ChargingState.OnEnter += Warbanner.ChangeRadiusTP;
            IL.RoR2.Items.WardOnLevelManager.OnCharacterLevelUp += Warbanner.ChangeRadius;

            //if (TopazBroochPercentBarrier.Value != 0f)
            //{
            //    GlobalEventManager.onCharacterDeathGlobal += TopazBrooch.AddBehavior;
            //}

            RecalculateStatsAPI.GetStatCoefficients += RepulsionArmorPlate.AddBehavior;

            RecalculateStatsAPI.GetStatCoefficients += TougherTimes.AddBehavior;

            RecalculateStatsAPI.GetStatCoefficients += Warbanner.AddBehavior;

            StickyBomb.Changes();

            TriTipDagger.ChangeBehavior();

            //   __ _ _ __ ___  ___ _ __  ___ 
            //  / _` | '__/ _ \/ _ \ '_ \/ __|
            // | (_| | | |  __/  __/ | | \__ \
            //  \__, |_|  \___|\___|_| |_|___/
            //   __/ |                        
            //  |___/ 

            // IL.RoR2.GlobalEventManager.OnInteractionBegin += SquidPolyp.ChangeAS;
            // IL.RoR2.GlobalEventManager.OnInteractionBegin += SquidPolyp.ChangeLifetime;
            // throws

            //               _     
            //              | |    
            //  _ __ ___  __| |___ 
            // | '__/ _ \/ _` / __|
            // | | |  __/ (_| \__ \
            // |_|  \___|\__,_|___/

            IL.RoR2.HealthComponent.Heal += Aegis.ChangeOverheal;

            IL.RoR2.CharacterBody.RecalculateStats += AlienHead.ChangeCDR;

            IL.RoR2.GlobalEventManager.OnHitAll += BrilliantBehemoth.ChangeDamage;
            IL.RoR2.GlobalEventManager.OnHitAll += BrilliantBehemoth.ChangeRadius;

            IL.RoR2.GlobalEventManager.OnCharacterDeath += Brainstalks.ChangeDuration;

            // IL.RoR2.GlobalEventManager.OnCharacterDeath += CeremonialDagger.ChangeCount;
            IL.RoR2.GlobalEventManager.OnCharacterDeath += CeremonialDagger.ChangeDamage;

            IL.EntityStates.Headstompers.HeadstompersIdle.FixedUpdateAuthority += Headstompers.ChangeJumpHeight;

            IL.RoR2.GlobalEventManager.OnCharacterDeath += HappiestMask.ChangeChance;
            // IL.RoR2.GlobalEventManager.OnCharacterDeath += HappiestMask.ChangeDuration;
            // fuck

            RecalculateStatsAPI.GetStatCoefficients += AlienHead.AddBehavior;

            CeremonialDagger.ChangeProc();

            DefensiveMicrobots.Changes();

            FrostRelic.Changes();

            Headstompers.Changes();


            if (BeetleQueenChanges.Value)
            {
                BeetleQueen.Buff();
            }
            if (ClayDunestriderChanges.Value)
            {
                ClayDunestrider.Berf();
            }
            if (GrandparentChanges.Value)
            {
                Grandparent.Buff();
            }
            if (GrovetenderChanges.Value)
            {
                Grovetender.Buff();
            }
            if (ImpOverlordChanges.Value)
            {
                ImpOverlord.Buff();
            }
            if (MithrixPhase1And3Changes.Value)
            {
                MithrixPhase1And3.Buff();
            }
            if (MithrixPhase2Changes.Value)
            {
                MithrixPhase2.Buff();
            }
            if (MithrixPhase4Changes.Value)
            {
                MithrixPhase4.Buff();
            }
            if (StoneTitanChanges.Value)
            {
                StoneTitan.Buff();
            }
            if (BeetleChanges.Value)
            {
                Beetle.Buff();
            }
            if (BighornBisonChanges.Value)
            {
                BighornBison.Buff();
            }
            if (GolemChanges.Value)
            {
                Golem.Berf();
            }
            if (GreaterWispChanges.Value)
            {
                GreaterWisp.Buff();
            }
            if (ImpChanges.Value)
            {
                Imp.Buff();
            }
            if (LemurianChanges.Value)
            {
                Lemurian.Buff();
            }
            if (LesserWispChanges.Value)
            {
                LesserWisp.Buff();
            }
            if (LunarExploderChanges.Value)
            {
                LunarExploder.Buff();
            }
            if (LunarWispChanges.Value)
            {
                LunarWisp.Nerf();
            }
            if (VoidReaverChanges.Value)
            {
                VoidReaver.Buff();
            }
            Commencement.Changes();
            DistantRoost.AddCredits();
            SunderedGrove.AddGrovetender();
            SunderedGrove.RemoveClayDunestrider();
            TitanicPlains.AddBison();
        }

        public static string d(float f)
        {
            return (f * 100f).ToString() + "%";
        }

        private void ChangeDescriptions()
        {

            // TODO: Separate all tokens into their respective classes and automate token loading... somehow...

            //           _     _ _            
            //          | |   (_) |           
            // __      _| |__  _| |_ ___  ___ 
            // \ \ /\ / / '_ \| | __/ _ \/ __|
            //  \ V  V /| | | | | ||  __/\__ \
            //   \_/\_/ |_| |_|_|\__\___||___/

            // LanguageAPI.Add("ITEM_TOOTH_DESC", "Killing an enemy spawns a <style=cIsHealing>healing orb</style> that heals for <style=cIsHealing>" + MonsterToothFlatHealing.Value + "</style> plus an additional <style=cIsHealing>" + d(MonsterToothPercentHealing.Value) + " <style=cStack>(+" + d(MonsterToothPercentHealing.Value) + " per stack)</style></style> of <style=cIsHealing>maximum health</style>.");

            //   __ _ _ __ ___  ___ _ __  ___ 
            //  / _` | '__/ _ \/ _ \ '_ \/ __|
            // | (_| | | |  __/  __/ | | \__ \
            //  \__, |_|  \___|\___|_| |_|___/
            //   __/ |                        
            //  |___/ 

            // var wooliegaming = SquidPolypAS.Value * 10f;
            // LanguageAPI.Add("ITEM_SQUIDTURRET_DESC", "Activating an interactable summons a <style=cIsDamage>Squid Turret</style> that attacks nearby enemies at <style=cIsDamage>" + wooliegaming + "% <style=cStack>(+" + wooliegaming + "% per stack)</style> attack speed</style>. Lasts <style=cIsUtility>" + SquidPolypDuration.Value + "</style> seconds.");

            //               _     
            //              | |    
            //  _ __ ___  __| |___ 
            // | '__/ _ \/ _` / __|
            // | | |  __/ (_| \__ \
            // |_|  \___|\__,_|___/

            var breh = BehemothAoe.Value + BehemothAoeStack.Value;
            LanguageAPI.Add("ITEM_BEHEMOTH_DESC", "All your <style=cIsDamage>attacks explode</style> in a <style=cIsDamage>" + breh + "m</style> <style=cStack>(+" + BehemothAoeStack.Value + "m per stack)</style> radius for a bonus <style=cIsDamage>" + d(BehemothDamage.Value) + "</style> TOTAL damage to nearby enemies.");

            LanguageAPI.Add("ITEM_KILLELITEFRENZY_DESC", "Upon killing an elite monster, <style=cIsDamage>enter a frenzy</style> for <style=cIsDamage>" + BrainstalksDuration.Value + "s</style> <style=cStack>(+" + BrainstalksDuration.Value + "s per stack)</style> where <style=cIsUtility>skills have no cooldowns</style>.");

            LanguageAPI.Add("ITEM_DAGGER_DESC", "Killing an enemy fires out <style=cIsDamage>3</style> <style=cIsDamage>homing daggers</style> that deal <style=cIsDamage>" + d(CeremonialDamage.Value) + "</style> <style=cStack>(+" + d(CeremonialDamage.Value) + " per stack)</style> base damage.");

            var bruj = 1 / DefeMicroRechargeFreq.Value;
            LanguageAPI.Add("ITEM_CAPTAINDEFENSEMATRIX_DESC", "Shoot down <style=cIsDamage>1</style> <style=cStack>(+1 per stack)</style> projectiles within <style=cIsDamage>" + DefeMicroRange.Value + "m</style> every <style=cIsDamage>" + bruj + " seconds</style>. <style=cIsUtility>Recharge rate scales with attack speed</style>.");

            var dpsss = FrostRelicDamage.Value / FrostRelicAS.Value;
            var radiusususus = FrostRelicBaseRadius.Value + FrostRelicRadiusPerKill.Value * FrostRelicMax.Value;
            var radiuseeeee = FrostRelicRadiusPerKill.Value * FrostRelicMax.Value;
            LanguageAPI.Add("ITEM_ICICLE_DESC", "Killing an enemy surrounds you with an <style=cIsDamage>ice storm</style> that deals <style=cIsDamage>" + d(dpsss) + " damage per second</style> and <style=cIsUtility>slows</style> enemies by <style=cIsUtility>80%</style> for <style=cIsUtility>1.5s</style>. The storm <style=cIsDamage>grows with every kill</style>, increasing its radius by <style=cIsDamage>" + FrostRelicRadiusPerKill.Value + "m</style>. Stacks up to <style=cIsDamage>" + radiusususus + "m</style> <style=cStack>(+" + radiuseeeee + "m per stack)</style>.");

            LanguageAPI.Add("ITEM_FALLBOOTS_DESC", "Increase <style=cIsUtility>jump height</style>. Creates a <style=cIsDamage>" + HeadstompersMinRange.Value + "m-" + HeadstompersMaxRange.Value + "m</style> radius <style=cIsDamage>kinetic explosion</style> on hitting the ground, dealing <style=cIsDamage>" + d(HeadstompersMinDamage.Value) + "-" + d(HeadstompersMaxDamage.Value) + "</style> base damage that scales up with <style=cIsDamage>fall distance</style>. Recharges in <style=cIsDamage>" + HeadstompersCooldown.Value + "</style> <style=cStack>(-50% per stack)</style> seconds.");

            LanguageAPI.Add("ITEM_GHOSTONKILL_DESC", "Killing enemies has a <style=cIsDamage>" + d(HappiestMaskChance.Value) + "</style> chance to <style=cIsDamage>spawn a ghost</style> of the killed enemy with <style=cIsDamage>1500%</style> damage. Lasts <style=cIsDamage>" + HappiestMaskDuration.Value + "s</style> <style=cStack>(+" + HappiestMaskDuration.Value + "s per stack)</style>.");

            LanguageAPI.Add("ITEM_UTILITYSKILLMAGAZINE_PICKUP", "Add " + HardlightCharges.Value + " extra charges of your Utility skill. Reduce Utility skill cooldown.");
            LanguageAPI.Add("ITEM_UTILITYSKILLMAGAZINE_DESC", "Add <style=cIsUtility>+" + HardlightCharges.Value + "</style> <style=cStack>(+" + HardlightCharges.Value + " per stack)</style> charges of your <style=cIsUtility>Utility skill</style>. <style=cIsUtility>Reduces Utility skill cooldown</style> by <style=cIsUtility>" + d(HardlightCDR.Value) +"</style>.");

            //  _                            
            // | |                           
            // | |_   _ _ __   __ _ _ __ ___ 
            // | | | | | '_ \ / _` | '__/ __|
            // | | |_| | | | | (_| | |  \__ \
            // |_|\__,_|_| |_|\__,_|_|  |___/

            // TODO

            // LanguageAPI.Add("ITEM_LUNARPRIMARYREPLACEMENT_DESC", "<style=cIsUtility>Replace your Primary Skill</style> with <style=cIsUtility>Hungering Gaze</style>. \n\nFire a flurry of <style=cIsUtility>tracking shards</style> that detonate after a delay, dealing <style=cIsDamage>120%</style> base damage. Hold up to " + VisionsCharges.Value + " charges <style=cStack>(+12 per stack)</style> that reload after " + VisionsCooldown.Value + " seconds <style=cStack>(+2 per stack)</style>.");
        }
    }
}
