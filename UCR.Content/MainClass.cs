using BepInEx;
using R2API;
using R2API.Utils;
using RoR2;
using UnityEngine;
using BepInEx.Configuration;
using System.Collections.Generic;

namespace UltimateCustomRun
{

    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [R2APISubmoduleDependency(nameof(LanguageAPI), nameof(RecalculateStatsAPI))]
    public class Main : BaseUnityPlugin
    {
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "HIFU";
        public const string PluginName = "UltimateCustomRun";
        public const string PluginVersion = "1.0.0";

        // I REALLY need help on how to autogenerate configs AND descriptions.
        // AND I also need to add description compat for itemstatsmod............... HELP
        // PLEASE HELP
        //  _   _  _____ _     ______ 
        // | | | ||  ___| |    | ___ \
        // | |_| || |__ | |    | |_/ /
        // |  _  ||  __|| |    |  __/ 
        // | | | || |___| |____| |    
        // \_| |_/\____/\_____/\_|    

        //        _       _           _ 
        //       | |     | |         | |
        //   __ _| | ___ | |__   __ _| |
        //  / _` | |/ _ \| '_ \ / _` | |
        // | (_| | | (_) | |_) | (_| | |
        //  \__, |_|\___/|_.__/ \__,_|_|
        //   __/ |                      
        //  |___/  

        public static ConfigEntry<bool> ConfigGuide { get; set; }
        public static ConfigEntry<int> EquipmentChargeCap { get; set; }

        public static ConfigEntry<float> GlobalCritDamageMultiplier { get; set; }

        public static ConfigEntry<float> GlobalLowHealthThreshold { get; set; }

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

        public static ConfigEntry<bool> AprB { get; set; }
        public static ConfigEntry<bool> AprC { get; set; }
        public static ConfigEntry<float> AprDamage { get; set; }
        public static ConfigEntry<bool> AprE { get; set; }
        public static ConfigEntry<bool> AprF { get; set; }

        public static ConfigEntry<float> BackupMagCDR { get; set; }

        public static ConfigEntry<float> BisonSteakHealth { get; set; }
        public static ConfigEntry<bool> BisonSteakLevelHealth { get; set; }
        public static ConfigEntry<float> BisonSteakRegen { get; set; }
        public static ConfigEntry<bool> BisonSteakRegenStack { get; set; }

        public static ConfigEntry<float> BungusInterval { get; set; }
        public static ConfigEntry<float> BungusHealingPercent { get; set; }
        public static ConfigEntry<float> BungusHealingPercentStack { get; set; }
        public static ConfigEntry<float> BungusRadius { get; set; }
        public static ConfigEntry<float> BungusRadiusStack { get; set; }

        public static ConfigEntry<float> CautiousSlugHealing { get; set; }

        public static ConfigEntry<float> CrowbarDamage { get; set; }
        public static ConfigEntry<float> CrowbarThreshold { get; set; }

        public static ConfigEntry<float> EnergyDrinkSpeed { get; set; }

        public static ConfigEntry<int> FireworksCount { get; set; }
        public static ConfigEntry<int> FireworksCountStack { get; set; }
        public static ConfigEntry<float> FireworksDamage { get; set; }
        public static ConfigEntry<float> FireworksProcCo { get; set; }

        public static ConfigEntry<float> FocusCrystalDamage { get; set; }
        public static ConfigEntry<float> FocusCrystalRange { get; set; }

        public static ConfigEntry<float> GasolineBaseRadius { get; set; }
        public static ConfigEntry<float> GasolineBurnDamage { get; set; }
        public static ConfigEntry<float> GasolineExplosionDamage { get; set; }
        public static ConfigEntry<float> GasolineStackRadius{ get; set; }

        public static ConfigEntry<float> LensMakersCrit { get; set; }
        public static ConfigEntry<float> LensMakersCritDamage { get; set; }

        public static ConfigEntry<bool> MedkitBuffStack { get; set; }
        public static ConfigEntry<bool> MedkitBuffToDebuff { get; set; }
        public static ConfigEntry<float> MedkitFlatHealing { get; set; }
        public static ConfigEntry<float> MedkitPercentHealing { get; set; }

        public static ConfigEntry<float> MonsterToothFlatHealing { get; set; }
        public static ConfigEntry<float> MonsterToothPercentHealing { get; set; }

        public static ConfigEntry<float> PoofSpeed { get; set; }

        public static ConfigEntry<float> PSGPercent { get; set; }

        public static ConfigEntry<float> RapArmor { get; set; }
        public static ConfigEntry<float> RapFlatDmgDecrease { get; set; }
        public static ConfigEntry<float> RapMinimumDmgTaken { get; set; }

        public static ConfigEntry<float> SoldiersSyringeAS { get; set; }

        public static ConfigEntry<float> StickyBombChance { get; set; }
        public static ConfigEntry<float> StickyBombDamage { get; set; }
        public static ConfigEntry<float> StickyBombDelay { get; set; }
        public static ConfigEntry<bool> StickyBombFalloff { get; set; }
        public static ConfigEntry<float> StickyBombRadius { get; set; }

        public static ConfigEntry<float> StunGrenadeChance { get; set; }

        public static ConfigEntry<float> TopazBroochBarrier { get; set; }
        public static ConfigEntry<float> TopazBroochPercentBarrier { get; set; }
        public static ConfigEntry<bool> TopazBroochPercentBarrierStack { get; set; }

        public static ConfigEntry<float> TougherTimesArmor { get; set; }
        public static ConfigEntry<float> TougherTimesBlockChance { get; set; }

        public static ConfigEntry<float> TriTipChance { get; set; }
        public static ConfigEntry<float> TriTipDuration { get; set; }
        public static ConfigEntry<bool> TriTipBuffStack { get; set; }

        public static ConfigEntry<float> WarbannerDamage { get; set; }
        public static ConfigEntry<float> WarbannerRadius { get; set; }
        public static ConfigEntry<float> WarbannerRadiusStack { get; set; }

        //   __ _ _ __ ___  ___ _ __  ___ 
        //  / _` | '__/ _ \/ _ \ '_ \/ __|
        // | (_| | | |  __/  __/ | | \__ \
        //  \__, |_|  \___|\___|_| |_|___/
        //   __/ |                        
        //  |___/   

        public static ConfigEntry<float> AtGChance { get; set; }
        public static ConfigEntry<float> AtGDamage { get; set; }
        public static ConfigEntry<float> AtGProcCo { get; set; }

        public static ConfigEntry<float> BandolierBase { get; set; }
        public static ConfigEntry<float> BandolierExponent { get; set; }
        public static ConfigEntry<bool> BandolierGuide { get; set; }

        public static ConfigEntry<float> BerzerkersArmorAlways { get; set; }
        public static ConfigEntry<float> BerzerkersBuffArmor { get; set; }
        public static ConfigEntry<float> BerzerkersDurationBase { get; set; }
        public static ConfigEntry<float> BerzerkersDurationStack { get; set; }
        public static ConfigEntry<int> BerzerkersKillsReq { get; set; }

        public static ConfigEntry<float> ChronobaubleAS { get; set; }
        public static ConfigEntry<bool> ChronobaubleStacking { get; set; }

        public static ConfigEntry<bool> DeathMarkChanges { get; set; }
        public static ConfigEntry<float> DeathMarkDmgIncreasePerDebuff { get; set; }
        public static ConfigEntry<int> DeathMarkMinimumDebuffsRequired { get; set; }
        public static ConfigEntry<float> DeathMarkStackBonus { get; set; }

        public static ConfigEntry<float> FuelCellCDR { get; set; }

        public static ConfigEntry<float> GhorsTomeChance { get; set; }
        public static ConfigEntry<int> GhorsTomeReward { get; set; }

        public static ConfigEntry<float> HarvestersCrit { get; set; }
        public static ConfigEntry<bool> HarvestersCritStack { get; set; }
        public static ConfigEntry<float> HarvestersHealBase { get; set; }
        public static ConfigEntry<float> HarvestersHealStack { get; set; }

        public static ConfigEntry<float> InfusionBaseCap { get; set; }
        public static ConfigEntry<float> InfusionBaseHealth { get; set; }
        public static ConfigEntry<float> InfusionPercentHealth { get; set; }
        public static ConfigEntry<bool> InfusionScaling { get; set; }

        public static ConfigEntry<float> KjaroBaseDamage { get; set; }
        public static ConfigEntry<float> KjaroTotalDamage { get; set; }

        public static ConfigEntry<float> OldGThreshold { get; set; }

        public static ConfigEntry<float> OldWarArmor { get; set; }
        public static ConfigEntry<bool> OldWarArmorStack { get; set; }

        public static ConfigEntry<float> PredatoryAS { get; set; }
        public static ConfigEntry<float> PredatoryBaseCap { get; set; }
        public static ConfigEntry<float> PredatoryCrit { get; set; }
        public static ConfigEntry<bool> PredatoryCritStack { get; set; }
        public static ConfigEntry<float> PredatoryStackCap { get; set; }
        public static ConfigEntry<float> PredatorySpeed { get; set; }
        public static ConfigEntry<bool> PredatorySpeedStack { get; set; }

        public static ConfigEntry<float> RedWhipSpeed { get; set; }

        public static ConfigEntry<float> RunaldBaseDamage { get; set; }
        public static ConfigEntry<float> RunaldTotalDamage { get; set; }

        public static ConfigEntry<bool> ReplaceRoseBucklerSprintWithHpThreshold { get; set; }
        public static ConfigEntry<int> RoseBucklerArmor { get; set; }
        public static ConfigEntry<int> RoseBucklerArmorAlways { get; set; }
        public static ConfigEntry<float> RoseBucklerThreshold { get; set; }

        public static ConfigEntry<int> SquidPolypAS { get; set; }
        public static ConfigEntry<int> SquidPolypDuration { get; set; }

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
        public void Awake()
        {
            
            //        _       _           _ 
            //       | |     | |         | |
            //   __ _| | ___ | |__   __ _| |
            //  / _` | |/ _ \| '_ \ / _` | |
            // | (_| | | (_) | |_) | (_| | |
            //  \__, |_|\___/|_.__/ \__,_|_|
            //   __/ |                      
            //  |___/ 

            ConfigGuide = Config.Bind<bool>(":: Config : Guide ::", "", (bool)true, "Some items, for example Fireworks Count work like this:\n4 + 4 * stack which results in 8 + 4 per stack.\nThis behavior is indicated in the config with the V tag.");

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
            Guide = Config.Bind<bool>("::: Global :::: Scaling :::", "Time Scaling Guide", (bool)true, "The entire Scaling formula is as follows:\n(Player Factor Base + Player Count * Player Count Multiplier + \nDifficulty Coefficient Multiplier * Difficulty Def Scaling Value \n(1 for Drizzle, 2 for Rainstorm, 3 for Monsoon) * \nPlayer Count ^ Player Count Exponent * \nTime in Minutes) * Exponential Stage Scaling Base ^ Stages Cleared \nI highly recommend changing Gold Scaling while changing these as well");
            Guide2 = Config.Bind<bool>("::: Global :::: Scaling :::", "Gold Scaling Guide", (bool)true, "The entire Scaling formula is as follows:\n(Gold Reward ^ Gold Reward Exponent) / Gold Reward Divisor Base ^ \n(Stage Clear Count / Gold Reward Divisor Stage)");

            SharedCombatDelay = Config.Bind<float>(":: Items Shared ::", "Activation Delay in Combat", (float)5f, "This affects Red Whip and probably something else. Vanilla is 5");
            SharedDangerDelay = Config.Bind<float>(":: Items Shared ::", "Activation Delay in Danger", (float)7f, "This affects Shields and Cautious Slug. Vanilla is 7");

            //           _     _ _            
            //          | |   (_) |           
            // __      _| |__  _| |_ ___  ___ 
            // \ \ /\ / / '_ \| | __/ _ \/ __|
            //  \ V  V /| | | | | ||  __/\__ \
            //   \_/\_/ |_| |_|_|\__\___||___/
            //                                

            // AprB = Config.Bind<bool>(":: Items : Whites :: Armor-Piercing Rounds", "Affect Bosses?", (bool)true, "Vanilla is false");
            // AprC = Config.Bind<bool>(":: Items : Whites :: Armor-Piercing Rounds", "Affect Champions?", (bool)true, "Vanilla is false");
            AprDamage = Config.Bind<float>(":: Items : Whites :: Armor-Piercing Rounds", "Damage Coefficient", (float)0.2f, "Decimal. Vanilla is 0.2");
            // AprE = Config.Bind<bool>(":: Items : Whites :: Armor-Piercing Rounds", "Affect Elites?", (bool)true, "Vanilla is false");
            // AprF = Config.Bind<bool>(":: Items : Whites :: Armor-Piercing Rounds", "Affect Fliers?", (bool)true, "Vanilla is false");

            BackupMagCDR = Config.Bind<float>(":: Items : Whites :: Backup Mag", "Cooldown Reduction", (float)0f, "Decimal. Per Stack. Vanilla is 0");
            // BackupMagCharges = Config.Bind<int>(":: Items : Whites :: Backup Mag", "Charges", (float)1, "Per Stack. Vanilla is 1");

            BisonSteakHealth = Config.Bind<float>(":: Items : Whites :: Bison Steak", "Health", (float)25f, "Per Stack. Vanilla is 25");
            BisonSteakLevelHealth = Config.Bind<bool>(":: Items : Whites :: Bison Steak", "Give Health worth a single Level Up?", (bool)false, "Vanilla is false");
            BisonSteakRegen = Config.Bind<float>(":: Items : Whites :: Bison Steak", "Regen", (float)0f, "Vanilla is 0");
            BisonSteakRegenStack = Config.Bind<bool>(":: Items : Whites :: Bison Steak", "Stack Regen?", (bool)false, "Vanilla is false");

            BungusHealingPercent = Config.Bind<float>(":: Items : Whites :: Bustling Fungus", "Base Healing Percent", (float)0.045f, "Decimal. Vanilla is 0.045");
            BungusHealingPercentStack = Config.Bind<float>(":: Items : Whites :: Bustling Fungus", "Healing Percent", (float)0.0225f, "Decimal. Per Stack. Vanilla is 0.0225");
            BungusInterval = Config.Bind<float>(":: Items : Whites :: Bustling Fungus", "Interval", (float)0.25f, "Decimal. Vanilla is 0.25");
            BungusRadius = Config.Bind<float>(":: Items : Whites :: Bustling Fungus", "Base Radius", (float)1.5f, "V. Vanilla is 1.5");
            BungusRadiusStack = Config.Bind<float>(":: Items : Whites :: Bustling Fungus", "Radius", (float)1.5f, "V. Per Stack. Vanilla is 1.5");

            CautiousSlugHealing = Config.Bind<float>(":: Items : Whites :: Cautious Slug", "Regen", (float)3f, "Per Stack. Vanilla is 3");

            CrowbarDamage = Config.Bind<float>(":: Items : Whites :: Crowbar", "Damage Coefficient", (float)0.75f, "Decimal. Per Stack. Vanilla is 0.75");
            CrowbarThreshold = Config.Bind<float>(":: Items : Whites :: Crowbar", "Threshold", (float)0.9f, "Decimal. Vanilla is 0.9");

            EnergyDrinkSpeed = Config.Bind<float>(":: Items : Whites :: Energy Drink", "Speed", (float)0.25f, "Decimal. Per Stack. Vanilla is 0.25");

            FireworksCount = Config.Bind<int>(":: Items : Whites :: Fireworks", "Base Count", (int)4, "V. Vanilla is 4");
            FireworksCountStack = Config.Bind<int>(":: Items : Whites :: Fireworks", "Count", (int)4, "V. Per Stack. Vanilla is 4");
            FireworksDamage = Config.Bind<float>(":: Items : Whites :: Fireworks", "Damage Coefficient", (float)3f, "Decimal. Vanilla is 3");
            FireworksProcCo = Config.Bind<float>(":: Items : Whites :: Fireworks", "Proc Coefficient", (float)0.2f, "Vanilla is 0.2");

            FocusCrystalDamage = Config.Bind<float>(":: Items : Whites :: Focus Crystal", "Damage Coefficient", (float)0.2f, "Decimal. Per Stack. Vanilla is 0.2");
            FocusCrystalRange = Config.Bind<float>(":: Items : Whites :: Focus Crystal", "Range", (float)13f, "Vanilla is 13");

            GasolineBaseRadius = Config.Bind<float>(":: Items : Whites :: Gasoline", "Base Range", (float)8f, "V. Vanilla is 8");
            GasolineBurnDamage = Config.Bind<float>(":: Items : Whites :: Gasoline", "Burn Damage", (float)1.5f, "Vanilla is 1.5");
            GasolineExplosionDamage = Config.Bind<float>(":: Items : Whites :: Gasoline", "Explosion Damage", (float)1.5f, "Vanilla is 1.5");
            GasolineStackRadius = Config.Bind<float>(":: Items : Whites :: Gasoline", "Range", (float)4f, "V. Per Stack. Vanilla is 4");

            LensMakersCrit = Config.Bind<float>(":: Items : Whites :: Lens-Makers Glasses", "Crit Chance", (float)10f, "Per Stack. Vanilla is 10");
            // LensMakersCritDamage = Config.Bind<float>(":: Items : Whites :: Lens-Makers Glasses", "Crit Damage", (float)25f, "Per Stack. Vanilla is 0");

            MedkitBuffStack = Config.Bind<bool>(":: Items : Whites :: Medkit", "Make Medkit Buff stack?", (bool)false, "Vanilla is false");
            MedkitBuffToDebuff = Config.Bind<bool>(":: Items : Whites :: Medkit", "Make Medkit a Debuff instead?", (bool)false, "Vanilla is false");
            MedkitFlatHealing = Config.Bind<float>(":: Items : Whites :: Medkit", "Flat Healing", (float)20f, "Vanilla is 20");
            MedkitPercentHealing = Config.Bind<float>(":: Items : Whites :: Medkit", "Percent Healing", (float)0.05f, "Decimal. Per Stack. Vanilla is 0.05");

            // MonsterToothFlatHealing = Config.Bind<float>(":: Items : Whites :: Monster Tooth", "Flat Healing", (float)8f, "Vanilla is 8");
            // MonsterToothPercentHealing = Config.Bind<float>(":: Items : Whites :: Monster Tooth", "Percent Healing", (float)0.02f, "Decimal. Per Stack. Vanilla is 0.02");

            PoofSpeed = Config.Bind<float>(":: Items : Whites :: Pauls Goat Hoof", "Speed", (float)0.14f, "Decimal. Per Stack. Vanilla is 0.14");

            PSGPercent = Config.Bind<float>(":: Items : Whites :: Personal Shield Generator", "Shield Percent", (float)0.08f, "Decimal. Per Stack. Vanilla is 0.08");

            RapFlatDmgDecrease = Config.Bind<float>(":: Items : Whites :: Repulsion Armor Plate", "Flat Damage Reduction", (float)5f, "Per Stack. Vanilla is 5");
            RapMinimumDmgTaken = Config.Bind<float>(":: Items : Whites :: Repulsion Armor Plate", "Minimum Damage", (float)1f, "Vanilla is 1");
            RapArmor = Config.Bind<float>(":: Items : Whites :: Repulsion Armor Plate", "Armor", (float)0f, "Per Stack. Vanilla is 0");

            SoldiersSyringeAS = Config.Bind<float>(":: Items : Whites :: Soldiers Syringe", "Attack Speed", (float)0.15f, "Decimal. Per Stack. Vanilla is 0.15");

            StickyBombDamage = Config.Bind<float>(":: Items : Whites :: Sticky Bomb", "Damage Coefficient", (float) 1.8f, "Decimal. Vanilla is 1.8");
            StickyBombChance = Config.Bind<float>(":: Items : Whites :: Sticky Bomb", "Chance", (float)5f, "Per Stack. Vanilla is 5");
            StickyBombRadius = Config.Bind<float>(":: Items : Whites :: Sticky Bomb", "Radius", (float)10f, "Vanilla is 10");
            StickyBombFalloff = Config.Bind<bool>(":: Items : Whites :: Sticky Bomb", "Falloff Type", (true), "If set to true, use Sweetspot, if set to false, use None. Vanilla is Sweetspot");
            StickyBombDelay = Config.Bind<float>(":: Items : Whites :: Sticky Bomb", "Explosion Delay", (float)1.5f, "Decimal. Vanilla is 1.5");

            // StunGrenadeChance = Config.Bind<float>(":: Items : Whites :: Stun Grenade", "Chance", (float)5f, "Vanilla is 5");

            TopazBroochBarrier = Config.Bind<float>(":: Items : Whites :: Topaz Brooch", "Barrier", (float)15f, "Per Stack. Vanilla is 15");
            TopazBroochPercentBarrier = Config.Bind<float>(":: Items : Whites :: Topaz Brooch", "Percent Barrier", (float)0.1f, "Decimal. Per Stack. Vanilla is 0");
            TopazBroochPercentBarrierStack = Config.Bind<bool>(":: Items : Whites :: Topaz Brooch", "Stack Percent Barrier?", (bool)true, "Vanilla is false");

            TougherTimesBlockChance = Config.Bind<float>(":: Items : Whites :: Tougher Times", "Block Chance", (float)15f, "Per Stack. Vanilla is 15");
            TougherTimesArmor = Config.Bind<float>(":: Items : Whites :: Tougher Times", "Armor", (float)0f, "Per Stack. Vanilla is 0");

            TriTipChance = Config.Bind<float>(":: Items : Whites :: Tri Tip Dagger", "Chance", (float)10f, "Per Stack. Vanilla is 10");
            TriTipDuration = Config.Bind<float>(":: Items : Whites :: Tri Tip Dagger", "Duration", (float)3f, "Vanilla is 3");
            TriTipBuffStack = Config.Bind<bool>(":: Items : Whites :: Tri Tip Dagger", "Stack Tri Tip Bleed?", (bool)false, "Vanilla is true");

            WarbannerDamage = Config.Bind<float>(":: Items : Whites :: Warbanner", "Base Damage", (float)0f, "Vanilla is 0");
            WarbannerRadius = Config.Bind<float>(":: Items : Whites :: Warbanner", "Base Radius", (float)8f, "V. Vanilla is 8");
            WarbannerRadiusStack = Config.Bind<float>(":: Items : Whites :: Warbanner", "Radius", (float)8f, "V. Per Stack. Vanilla is 8");

            //   __ _ _ __ ___  ___ _ __  ___ 
            //  / _` | '__/ _ \/ _ \ '_ \/ __|
            // | (_| | | |  __/  __/ | | \__ \
            //  \__, |_|  \___|\___|_| |_|___/
            //   __/ |                        
            //  |___/   

            AtGChance = Config.Bind<float>(":: Items :: Greens :: AtG Missile Mk1", "Chance", (float)10f, "Vanilla is 10");
            AtGDamage = Config.Bind<float>(":: Items :: Greens :: AtG Missile Mk1", "Total Damage", (float)3f, "Decimal. Vanilla is 3");
            AtGProcCo = Config.Bind<float>(":: Items :: Greens :: AtG Missile Mk1", "Proc Coefficient", (float)1f, "Vanilla is 1");

            BandolierBase = Config.Bind<float>(":: Items :: Greens :: Bandolier", "Base", (float)1f, "Vanilla is 1");
            BandolierExponent = Config.Bind<float>(":: Items :: Greens :: Bandolier", "Exponent", (float)0.33f, "Vanilla is 0.33");
            BandolierGuide = Config.Bind<bool>(":: Items :: Greens :: Bandolier", "Formula", (bool)true, "Bandolier Formula:\n1 - 1 / (stack + Base)^Exponent) * 100\nIf you want to make stacking better, decrease the Base and increase the Exponent.");

            BerzerkersKillsReq = Config.Bind<int>(":: Items :: Greens :: Berzerkers Pauldron", "Kills Required", (int)4, "Vanilla is 4");
            BerzerkersDurationBase = Config.Bind<float>(":: Items :: Greens :: Berzerkers Pauldron", "Base Duration", (float)2f, "V. Vanilla is 2");
            BerzerkersDurationStack = Config.Bind<float>(":: Items :: Greens :: Berzerkers Pauldron", "Duration", (float)4f, "V. Per Stack. Vanilla is 4");
            BerzerkersBuffArmor = Config.Bind<float>(":: Items :: Greens :: Berzerkers Pauldron", "Buff Armor", (float)0f, "Per Stack. Vanilla is 0");
            BerzerkersArmorAlways = Config.Bind<float>(":: Items :: Greens :: Berzerkers Pauldron", "Armor", (float)0f, "Per Stack. Vanilla is 0");

            ChronobaubleStacking = Config.Bind<bool>(":: Items :: Greens :: Chronobauble", "Stack Attack Speed Decrease?", (bool)false, "Vanilla is false");
            ChronobaubleAS = Config.Bind<float>(":: Items :: Greens :: Chronobauble", "Chronobauble Attack Speed Decrease", (float)0.0f, "Decimal. Per Stack. Vanilla is 0");

            DeathMarkChanges = Config.Bind<bool>(":: Items :: Greens :: Death Mark", "Enable Death Mark Changes?", (bool)false, "Vanilla is false");
            DeathMarkDmgIncreasePerDebuff = Config.Bind<float>(":: Items :: Greens :: Death Mark", "Damage Increase Per Debuff", (float)0.1f, "Decimal. Per Debuff. Only works with Death Mark Changes enabled");
            DeathMarkStackBonus = Config.Bind<float>(":: Items :: Greens :: Death Mark", "Damage Increase", (float)0.05f, "Decimal. Per Stack. Per Debuff. Only works with Death Mark Changes enabled");
            DeathMarkMinimumDebuffsRequired = Config.Bind<int>(":: Items :: Greens :: Death Mark", "Minimum Debuffs", (int)2, "Only works with Death Mark Changes enabled");

            FuelCellCDR = Config.Bind<float>(":: Items :: Greens :: Fuel Cell", "Cooldown Reduction", (float)0.15f, "Decimal. Per Stack. Vanilla is 0.15");

            GhorsTomeReward = Config.Bind<int>(":: Items :: Greens :: Ghors Tome", "Reward", (int)25, "Vanilla is 25");
            GhorsTomeChance = Config.Bind<float>(":: Items :: Greens :: Ghors Tome", "Chance", (float)4f, "Vanilla is 4");

            HarvestersCritStack = Config.Bind<bool>(":: Items :: Greens :: Harvesters Scythe", "Stack Crit Chance?", (bool)false, "Vanilla is false");
            HarvestersCrit = Config.Bind<float>(":: Items :: Greens :: Harvesters Scythe", "Crit Chance", (float)5f, "Vanilla is 5");
            HarvestersHealBase = Config.Bind<float>(":: Items :: Greens :: Harvesters Scythe", "Base Healing", (float)4f, "V. Vanilla is 4");
            HarvestersHealStack = Config.Bind<float>(":: Items :: Greens :: Harvesters Scythe", "Healing", (float)4f, "V. Per Stack. Vanilla is 4");

            InfusionScaling = Config.Bind<bool>(":: Items :: Greens :: Infusion", "Use Level Scaling Cap?", (bool)false, "Infusion Formula:\nBase Cap * 1 + 0.3 * (Level - 1) * Count");
            InfusionBaseCap = Config.Bind<float>(":: Items :: Greens :: Infusion", "Base Cap", (float)30f, "Only works with Level Scaling Cap enabled");
            InfusionBaseHealth = Config.Bind<float>(":: Items :: Greens :: Infusion", "Base Health", (float)0f, "Per Stack. Vanilla is 0");
            InfusionPercentHealth = Config.Bind<float>(":: Items :: Greens :: Infusion", "Percent Health", (float)0f, "Decimal. Per Stack. Vanilla is 0");

            // KjaroTotalDamage = Config.Bind<float>(":: Items :: Greens :: Kjaros Band", "Total Damage", (float)3f, "Decimal. Per Stack. Vanilla is 3");
            // KjaroBaseDamage = Config.Bind<float>(":: Items :: Greens :: Kjaros Band", "Base Damage", (float)0f, "Decimal. Per Stack. Vanilla is 0");

            OldGThreshold = Config.Bind<float>(":: Items :: Greens :: Old Guillotine", "Threshold", (float)13f, "Vanilla is 13");

            OldWarArmor = Config.Bind<float>(":: Items :: Greens :: Old War Stealthkit", "Armor", (float)0f, "With Buff. Vanilla is 0");
            OldWarArmorStack = Config.Bind<bool>(":: Items :: Greens :: Old War Stealthkit", "Stack Armor?", (bool)false, "With Buff. Vanilla is false");

            // PredatoryAS = Config.Bind<float>(":: Items :: Greens :: Predatory Instincts", "Buff Attack Speed", (float)0.12f, "Decimal. Per Buff. Vanilla is 0.12");
            // PredatoryBaseCap = Config.Bind<float>(":: Items :: Greens :: Predatory Instincts", "Buff Base Cap", (int)1, "V. Vanilla is 1");
            // PredatoryStackCap = Config.Bind<float>(":: Items :: Greens :: Predatory Instincts", "Buff Cap Per Stack", (int)2, "V. Per Stack. Vanilla is 2");
            // PredatoryCritStack = Config.Bind<bool>(":: Items :: Greens :: Predatory Instincts", "Stack Crit Chance?", (bool)false, "Vanilla is false");
            // PredatoryCrit = Config.Bind<float>(":: Items :: Greens :: Predatory Instincts", "Crit Chance", (float)5f, "Vanilla is 5");
            // PredatorySpeed = Config.Bind<float>(":: Items :: Greens :: Predatory Instincts", "Buff Base Speed", (float)0f, "Decimal. Per Buff. Vanilla is 0");
            // PredatorySpeedStack = Config.Bind<bool>(":: Items :: Greens :: Predatory Instincts", "Stack Speed?", (bool)false, " Vanilla is false");

            RedWhipSpeed = Config.Bind<float>(":: Items :: Greens :: Red Whip", "Speed", (float)0.3f, "Decimal. Per Stack. Vanilla is 0.3");

            ReplaceRoseBucklerSprintWithHpThreshold = Config.Bind<bool>(":: Items :: Greens :: Rose Buckler", "Replace Condition to be Below X% Health instead?", (bool)false, "Vanilla is false");
            RoseBucklerThreshold = Config.Bind<float>(":: Items :: Greens :: Rose Buckler", "Health Threshold", (float)0.5f, "Decimal. Only works with Replace Condition enabled");
            RoseBucklerArmor = Config.Bind<int>(":: Items :: Greens :: Rose Buckler", "Conditional Armor", (int)30, "Per Stack. Vanilla is 30");
            RoseBucklerArmorAlways = Config.Bind<int>(":: Items :: Greens :: Rose Buckler", "Armor", (int)0, "Per Stack. Vanilla is 0");

            // RunaldTotalDamage = Config.Bind<float>(":: Items :: Greens :: Runalds Band", "Total Damage", (float)2.5f, "Per Stack. Vanilla is 2.5");
            // RunaldBaseDamage = Config.Bind<float>(":: Items :: Greens :: Runalds Band", "Base Damage", (float)0f, "Decimal. Per Stack. Vanilla is 0");

            // SquidPolypDuration = Config.Bind<int>(":: Items :: Greens :: Squid Polyp", "Lifetime", (int)30, "Vanilla is 30");
            // SquidPolypAS = Config.Bind<int>(":: Items :: Greens :: Squid Polyp", "Attack Speed Item Count", (int)10, "Per Stack. Vanilla is 10");

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

            IL.RoR2.CharacterBody.FixedUpdate += AAShared.ChangeCombatDelay;
            IL.RoR2.CharacterBody.FixedUpdate += AAShared.ChangeDangerDelay;

            //           _     _ _            
            //          | |   (_) |           
            // __      _| |__  _| |_ ___  ___ 
            // \ \ /\ / / '_ \| | __/ _ \/ __|
            //  \ V  V /| | | | | ||  __/\__ \
            //   \_/\_/ |_| |_|_|\__\___||___/
            //    


            IL.RoR2.HealthComponent.TakeDamage += ArmorPiercingRounds.ChangeDamage;
            //IL.RoR2.HealthComponent.TakeDamage += ArmorPiercingRounds.ChangeType;

            IL.RoR2.CharacterBody.RecalculateStats += BisonSteak.ChangeHealth;

            IL.RoR2.CharacterBody.MushroomItemBehavior.FixedUpdate += BustlingFungus.ChangeRadius;
            IL.RoR2.CharacterBody.MushroomItemBehavior.FixedUpdate += BustlingFungus.ChangeHealing;

            IL.RoR2.CharacterBody.RecalculateStats += CautiousSlug.ChangeHealing;

            IL.RoR2.HealthComponent.TakeDamage += Crowbar.ChangeDamage;
            IL.RoR2.HealthComponent.TakeDamage += Crowbar.ChangeThreshold;

            IL.RoR2.CharacterBody.RecalculateStats += EnergyDrink.ChangeSpeed;

            IL.RoR2.GlobalEventManager.OnInteractionBegin += Fireworks.ChangeCount;

            IL.RoR2.HealthComponent.TakeDamage += FocusCrystal.ChangeDamage;
            IL.RoR2.HealthComponent.TakeDamage += FocusCrystal.ChangeRadius;

            IL.RoR2.GlobalEventManager.ProcIgniteOnKill += Gasoline.ChangeBurnDamage;
            IL.RoR2.GlobalEventManager.ProcIgniteOnKill += Gasoline.ChangeExplosionDamage;
            IL.RoR2.GlobalEventManager.ProcIgniteOnKill += Gasoline.ChangeRadius;

            IL.RoR2.CharacterBody.RecalculateStats += LensMakersGlasses.ChangeCrit;

            IL.RoR2.CharacterBody.RemoveBuff_BuffIndex += Medkit.ChangeFlatHealing;
            IL.RoR2.CharacterBody.RemoveBuff_BuffIndex += Medkit.ChangePercentHealing;

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

            if (BisonSteakRegen.Value != 0f)
            {
                GlobalEventManager.onCharacterDeathGlobal += BisonSteak.AddBehaviorRegen;
            }

            if (TopazBroochPercentBarrier.Value != 0f)
            {
                GlobalEventManager.onCharacterDeathGlobal += TopazBrooch.AddBehavior;
            }

            RecalculateStatsAPI.GetStatCoefficients += BackupMag.AddBehavior;

            if (BisonSteakLevelHealth.Value)
            {
                RecalculateStatsAPI.GetStatCoefficients += BisonSteak.AddBehaviorLevelHealth;
            }
            if (BisonSteakRegenStack.Value)
            {
                RecalculateStatsAPI.GetStatCoefficients += BisonSteak.ChangeBuffBehavior;
            }

            //RecalculateStatsAPI.GetStatCoefficients += LensMakersGlasses.AddBehavior;
            // Waiting :plead

            RecalculateStatsAPI.GetStatCoefficients += RepulsionArmorPlate.AddBehavior;

            RecalculateStatsAPI.GetStatCoefficients += TougherTimes.AddBehavior;

            RecalculateStatsAPI.GetStatCoefficients += Warbanner.AddBehavior;

            Fireworks.Changes();

            FocusCrystal.ChangeVisual();

            GhorsTome.ChangeReward();

            Medkit.ChangeBuffBehavior();

            StickyBomb.Changes();

            TriTipDagger.ChangeBehavior();

            //   __ _ _ __ ___  ___ _ __  ___ 
            //  / _` | '__/ _ \/ _ \ '_ \/ __|
            // | (_| | | |  __/  __/ | | \__ \
            //  \__, |_|  \___|\___|_| |_|___/
            //   __/ |                        
            //  |___/ 

            IL.RoR2.GlobalEventManager.ProcMissile += AtGMissileMk1.ChangeChance;
            IL.RoR2.GlobalEventManager.ProcMissile += AtGMissileMk1.ChangeDamage;

            IL.RoR2.GlobalEventManager.OnCharacterDeath += Bandolier.ChangeBase;
            IL.RoR2.GlobalEventManager.OnCharacterDeath += Bandolier.ChangeExponent;

            IL.RoR2.CharacterBody.AddMultiKill += BerzerkersPauldron.ChangeKillCount;
            IL.RoR2.CharacterBody.AddMultiKill += BerzerkersPauldron.ChangeBuffDuration;

            IL.RoR2.GlobalEventManager.OnHitEnemy += DeathMark.ChangeDebuffsReq;
            IL.RoR2.HealthComponent.TakeDamage += DeathMark.Changes;

            IL.RoR2.Inventory.CalculateEquipmentCooldownScale += FuelCell.ChangeCDR;

            IL.RoR2.GlobalEventManager.OnCharacterDeath += GhorsTome.ChangeChance;

            IL.RoR2.GlobalEventManager.OnCrit += HarvestersScythe.ChangeHealing;
            IL.RoR2.CharacterBody.RecalculateStats += HarvestersScythe.ChangeCrit;

            if (InfusionScaling.Value == true)
            {
                IL.RoR2.GlobalEventManager.OnCharacterDeath += Infusion.ChangeBehavior;
            }

            //IL.RoR2.GlobalEventManager.OnHitEnemy += KjaroChange;
            // throws OR breaks all my other IL

            IL.RoR2.CharacterBody.OnInventoryChanged += OldGuillotine.ChangeThreshold;

            /*
            IL.RoR2.CharacterBody.AddTimedBuff_BuffDef_float += PredatoryInstincts.ChangeCap;
            IL.RoR2.CharacterBody.RecalculateStats += PredatoryInstincts.ChangeAS;
            */

            IL.RoR2.CharacterBody.RecalculateStats += RedWhip.ChangeSpeed;

            if (ReplaceRoseBucklerSprintWithHpThreshold.Value == true)
            {
                IL.RoR2.CharacterBody.RecalculateStats += RoseBuckler.ChangeBehavior;
            }
            IL.RoR2.CharacterBody.RecalculateStats += RoseBuckler.ChangeArmor;

            //IL.RoR2.GlobalEventManager.OnHitEnemy += RunaldChange;
            // throws OR breaks all my other IL

            // IL.RoR2.GlobalEventManager.OnInteractionBegin += SquidPolyp.ChangeAS;
            // IL.RoR2.GlobalEventManager.OnInteractionBegin += SquidPolyp.ChangeLifetime;
            // throws

            AtGMissileMk1.ChangeProc();

            RecalculateStatsAPI.GetStatCoefficients += BerzerkersPauldron.AddBehavior;
            // throws (harmless)


            RecalculateStatsAPI.GetStatCoefficients += Chronobauble.AddBehavior;
            // throws (harmless)


            if (HarvestersCritStack.Value)
            {
                RecalculateStatsAPI.GetStatCoefficients += HarvestersScythe.AddBehavior;
            }

            RecalculateStatsAPI.GetStatCoefficients += Infusion.BehaviorAddFlatHealth;
            RecalculateStatsAPI.GetStatCoefficients += Infusion.BehaviorAddPercentHealth;

            RecalculateStatsAPI.GetStatCoefficients += OldWarStealthkit.AddBehavior;

            RecalculateStatsAPI.GetStatCoefficients += PredatoryInstincts.AddBehavior;
           
            RecalculateStatsAPI.GetStatCoefficients += RoseBuckler.AddBehavior;
            if (ReplaceRoseBucklerSprintWithHpThreshold.Value == true)
            {
                RoseBuckler.Insanity();
            }

            //  _                            
            // | |                           
            // | |_   _ _ __   __ _ _ __ ___ 
            // | | | | | '_ \ / _` | '__/ __|
            // | | |_| | | | | (_| | |  \__ \
            // |_|\__,_|_| |_|\__,_|_|  |___/

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
            //  

            /*
            List<string> aprStrings = new List<string>();
            if (AprB.Value) { aprStrings.Add("bosses"); }
            if (AprC.Value) { aprStrings.Add("champions"); }
            if (AprE.Value) { aprStrings.Add("elites"); }
            if (AprF.Value) { aprStrings.Add("fliers"); }

            string allEnemiesAffected = "";
            for (int i = 0; i < aprStrings.Count; i++)
            {
                if (i != aprStrings.Count - 1)
                {
                    allEnemiesAffected += $"{aprStrings[i]}, ";
                }
                else
                {
                    allEnemiesAffected += $"and {aprStrings[i]}";
                }
            }
            */
            // LanguageAPI.Add("ITEM_BOSSDAMAGEBONUS_DESC", "Deal an additional <style=cIsDamage>" + d(AprDamage.Value) + "</style> damage <style=cStack>(+" + d(AprDamage.Value) + " per stack)</style> to " + allEnemiesAffected + ".");
            LanguageAPI.Add("ITEM_BOSSDAMAGEBONUS_DESC", "Deal an additional <style=cIsDamage>" + d(AprDamage.Value) + "</style> damage <style=cStack>(+" + d(AprDamage.Value) + " per stack)</style> to bosses.");

            // THANK YOU BORBO HOLY
            // play their game
            // https://plumicorn.itch.io/superbug

            bool bCDR = BackupMagCDR.Value != 0f;
            LanguageAPI.Add("ITEM_SECONDARYSKILLMAGAZINE_PICKUP", "Add an extra charge of your Secondary skill" +
            (bCDR ? " and reduce its Cooldown." : ""));
            LanguageAPI.Add("ITEM_SECONDARYSKILLMAGAZINE_DESC", "Add <style=cIsUtility>+1</style> <style=cStack>(+1 per stack)</style> charge of your <style=cIsUtility>Secondary skill</style>." +
            (bCDR ? " Reduce your Secondary skill Cooldown by <style=cIsUtility>" + d(BackupMagCDR.Value) + "</style> <style=cStack>(+" + d(BackupMagCDR.Value) + " per stack)</style>." : ""));

            bool useFlatHealthSteak = BisonSteakHealth.Value != 0f;
            bool useLevelHealthSteak = BisonSteakLevelHealth.Value;
            bool useBothHealthSteak = useFlatHealthSteak && useLevelHealthSteak;
            bool useRegen = BisonSteakRegen.Value != 0f;
            bool useRegenStack = BisonSteakRegenStack.Value;
            LanguageAPI.Add("ITEM_FLATHEALTH_PICKUP",
            (useRegen ? "Regenerate on kill." : "") +
            $"Gain" +
            (useLevelHealthSteak ? $" 30% base health" : "") +
            (useBothHealthSteak ? $" +" : "") +
            (useFlatHealthSteak ? $" {BisonSteakHealth.Value} max health" : "") +
            $".");

            LanguageAPI.Add("ITEM_FLATHEALTH_DESC",
            (useRegen ? "Increases <style=cIsHealing>base health regeneration</style> by <style=cIsHealing>" + BisonSteakRegen.Value + "</style>. " : "") +
            (useRegenStack ? "<style=cStack>(+" + BisonSteakRegen.Value + " Per Stack)</style>. " : "") +
            $"Increases <style=cIsHealing>base health</style> by" +
            (useLevelHealthSteak ? $" <style=cIsHealing>30%</style> <style=cStack>(+30% per stack)</style>" : "") +
            (useBothHealthSteak ? $" +" : "") +
            (useFlatHealthSteak ? $" <style=cIsHealing>{BisonSteakHealth.Value}</style> <style=cStack>(+{BisonSteakHealth.Value} per stack)</style>" : "") +
            $".");

            var endme = BungusHealingPercent.Value + BungusHealingPercentStack.Value;
            var pleasekillmenow = BungusRadius.Value + BungusRadiusStack.Value;
            LanguageAPI.Add("ITEM_MUSHROOM_DESC", "After standing still for <style=cIsHealing>1</style> second, create a zone that <style=cIsHealing>heals</style> for <style=cIsHealing>" + d(endme) + "</style> <style=cStack>(+" + d(BungusHealingPercentStack.Value) + " per stack)</style> of your <style=cIsHealing>health</style> every second to all allies within <style=cIsHealing>" + pleasekillmenow + "m</style> <style=cStack>(+" + BungusRadiusStack.Value + "m per stack)</style>.");

            LanguageAPI.Add("ITEM_HEALWHILESAFE_DESC", "Increases <style=cIsHealing>base health regeneration</style> by <style=cIsHealing>+" + CautiousSlugHealing.Value + " hp/s</style> <style=cStack>(+" + CautiousSlugHealing.Value + " hp/s per stack)</style> while outside of combat.");

            LanguageAPI.Add("ITEM_CROWBAR_PICKUP", "Deal bonus damage to enemies above " + d(CrowbarThreshold.Value) + " health.");
            LanguageAPI.Add("ITEM_CROWBAR_DESC", "Deal <style=cIsDamage>+" + d(CrowbarDamage.Value) + "</style> <style=cStack>(+" + d(CrowbarDamage.Value) + " per stack)</style> damage to enemies above <style=cIsDamage>" + d(CrowbarThreshold.Value) + " health</style>.");

            LanguageAPI.Add("ITEM_SPRINTBONUS_DESC", "<style=cIsUtility>Sprint speed</style> is improved by <style=cIsUtility>" + d(EnergyDrinkSpeed.Value) + "</style> <style=cStack>(+" + d(EnergyDrinkSpeed.Value) + " per stack)</style>.");

            var imsuicidal = FireworksCount.Value + FireworksCountStack.Value;
            LanguageAPI.Add("ITEM_FIREWORK_DESC", "Activating an interactable <style=cIsDamage>launches " + imsuicidal + " <style=cStack>(+" + FireworksCountStack.Value + " per stack)</style> fireworks</style> that deal <style=cIsDamage>" + d(FireworksDamage.Value) + "</style> base damage.");

            LanguageAPI.Add("ITEM_NEARBYDAMAGEBONUS_DESC", "Increase damage to enemies within <style=cIsDamage>" + FocusCrystalRange.Value + "m</style> by <style=cIsDamage>" + d(FocusCrystalDamage.Value) + "</style> <style=cStack>(+" + d(FocusCrystalDamage.Value) + " per stack)</style>.");

            var erised = GasolineBaseRadius.Value + GasolineStackRadius.Value;
            var fourlights = GasolineBurnDamage.Value / 2;
            LanguageAPI.Add("ITEM_IGNITEONKILL_DESC", "Killing an enemy <style=cIsDamage>ignites</style> all enemies within <style=cIsDamage>" + erised + "m</style> <style=cStack>(+" + GasolineStackRadius.Value + "m per stack)</style> for <style=cIsDamage>" + d(GasolineExplosionDamage.Value) + "</style> base damage. Additionally, enemies <style=cIsDamage>burn</style> for <style=cIsDamage>" + d(GasolineBurnDamage.Value) + "</style> <style=cStack>(+" + d(fourlights) + " per stack)</style> base damage.");

            LanguageAPI.Add("ITEM_CRITGLASSES_PICKUP", "Chance to 'Critically Strike', dealing " + GlobalCritDamageMultiplier.Value + "x damage.");
            LanguageAPI.Add("ITEM_CRITGLASSES_DESC", "Your attacks have a <style=cIsDamage>" + LensMakersCrit.Value + "%</style> <style=cStack>(+" + LensMakersCrit.Value + "% per stack)</style> chance to '<style=cIsDamage>Critically Strike</style>', dealing <style=cIsDamage>" + GlobalCritDamageMultiplier.Value + "x damage</style>.");

            LanguageAPI.Add("ITEM_MEDKIT_DESC", "2 seconds after getting hurt, <style=cIsHealing>heal</style> for <style=cIsHealing>" + MedkitFlatHealing.Value + "</style> plus an additional <style=cIsHealing>" + d(MedkitPercentHealing.Value) + " <style=cStack>(+" + d(MedkitPercentHealing.Value) + " per stack)</style></style> of <style=cIsHealing>maximum health</style>.");

            // LanguageAPI.Add("ITEM_TOOTH_DESC", "Killing an enemy spawns a <style=cIsHealing>healing orb</style> that heals for <style=cIsHealing>" + MonsterToothFlatHealing.Value + "</style> plus an additional <style=cIsHealing>" + d(MonsterToothPercentHealing.Value) + " <style=cStack>(+" + d(MonsterToothPercentHealing.Value) + " per stack)</style></style> of <style=cIsHealing>maximum health</style>.");

            LanguageAPI.Add("ITEM_HOOF_DESC", "Increases <style=cIsUtility>movement speed</style> by <style=cIsUtility>" + d(PoofSpeed.Value) + "</style> <style=cStack>(+" + d(PoofSpeed.Value) + " per stack)</style>.");
            
            LanguageAPI.Add("ITEM_PERSONALSHIELD_DESC", "Gain a <style=cIsHealing>shield</style> equal to <style=cIsHealing>" + d(PSGPercent.Value) + "</style> <style=cStack>(+" + d(PSGPercent.Value) + " per stack)</style> of your maximum health. Recharges outside of danger.");

            bool rArmor = RapArmor.Value != 0f;
            LanguageAPI.Add("ITEM_REPULSIONARMORPLATE_DESC",
            (rArmor ? "<style=cIsHealing>Increase armor</style> by <style=cIsHealing>" + RapArmor.Value + "</style> <style=cStack>(+" + RapArmor.Value + " per stack)</style>. " : "") +
            "Reduce all <style=cIsDamage>incoming damage</style> by <style=cIsDamage>" + RapFlatDmgDecrease.Value + "<style=cStack> (+" + RapFlatDmgDecrease.Value + " per stack)</style></style>. Cannot be reduced below <style=cIsDamage>" + RapMinimumDmgTaken.Value + "</style>.");

            LanguageAPI.Add("ITEM_SYRINGE_DESC", "Increases <style=cIsDamage>attack speed</style> by <style=cIsDamage>" + d(SoldiersSyringeAS.Value) + " <style=cStack>(+" + d(SoldiersSyringeAS.Value) + " per stack)</style></style>.");

            LanguageAPI.Add("ITEM_STICKYBOMB_DESC", "<style=cIsDamage>" + StickyBombChance.Value + "%</style> <style=cStack>(+" + StickyBombChance.Value + "% per stack)</style> chance on hit to attach a <style=cIsDamage>bomb</style> to an enemy, detonating for <style=cIsDamage>" + d(StickyBombDamage.Value) + "</style> TOTAL damage.");

            LanguageAPI.Add("ITEM_BARRIERONKILL_DESC", "Gain a <style=cIsHealing>temporary barrier</style> on kill for <style=cIsHealing>" + TopazBroochBarrier.Value + " health <style=cStack>(+" + TopazBroochBarrier.Value + " per stack)</style></style>.");

            bool tArmor = TougherTimesArmor.Value != 0f;
            // actually idk how to make like a master string that takes in both the armor existing and block chance existing and conjoins them based on those values
            LanguageAPI.Add("ITEM_BEAR_PICKUP",
            "Chance to block incoming damage." +
            (tArmor ? " Reduce incoming damage." : ""));
            LanguageAPI.Add("ITEM_BEAR_DESC",
            "<style=cIsHealing>" + TougherTimesBlockChance.Value + "%</style> <style=cStack>(+" + TougherTimesBlockChance.Value + "% per stack)</style> chance to <style=cIsHealing>block</style> incoming damage. " +
            (tArmor ? "<style=cIsHealing>Increase armor</style> by <style=cIsHealing>" + TougherTimesArmor.Value + "</style> <style=cStack>(+" + TougherTimesArmor.Value + " per stack)</style>. " : "") +
            "<style=cIsUtility>Unaffected by luck</style>.");

            LanguageAPI.Add("ITEM_BLEEDONHIT_DESC", "<style=cIsDamage>" + TriTipChance.Value + "%</style> <style=cStack>(+" + TriTipChance.Value + "% per stack)</style> chance to <style=cIsDamage>bleed</style> an enemy for <style=cIsDamage>240%</style> base damage.");

            bool wBaseDamage = WarbannerDamage.Value != 0f;
            var j = WarbannerRadius.Value + WarbannerRadiusStack.Value;
            LanguageAPI.Add("ITEM_WARDONLEVEL_PICKUP",
            "Drop a Warbanner on level up or starting the Teleporter event. Grants allies " +
            (wBaseDamage ? "base damage, " : "") +
            "movement speed and attack speed.");
            LanguageAPI.Add("ITEM_WARDONLEVEL_DESC",
            "On <style=cIsUtility>level up</style> or starting the <style=cIsUtility>Teleporter event</style>, drop a banner that strengthens all allies within <style=cIsUtility>" + j + "m</style> <style=cStack>(+" + WarbannerRadiusStack.Value + "m per stack)</style>. Raise " +
            (wBaseDamage ? "<style=cIsDamage>base damage</style> by <style=cIsDamage>" + WarbannerDamage.Value + "</style>, " : "") +
            "<style=cIsUtility>movement speed</style> and <style=cIsDamage>attack speed</style> by <style=cIsDamage>30%</style>.");

            //   __ _ _ __ ___  ___ _ __  ___ 
            //  / _` | '__/ _ \/ _ \ '_ \/ __|
            // | (_| | | |  __/  __/ | | \__ \
            //  \__, |_|  \___|\___|_| |_|___/
            //   __/ |                        
            //  |___/ 

            LanguageAPI.Add("ITEM_MISSILE_DESC", "<style=cIsDamage>" + AtGChance.Value + "%</style> chance to fire a missile that deals <style=cIsDamage>" + d(AtGDamage.Value) + "</style> <style=cStack>(+" + d(AtGDamage.Value) + " per stack)</style> TOTAL damage.");

            var imsosickandtiredofthishit = Mathf.Round(1f - 1f / Mathf.Pow(1f + (BandolierBase.Value * 100f), BandolierExponent.Value));
            var notmoddingbutpeopleingeneral = Mathf.Round(1f - 1f / Mathf.Pow(2f + (BandolierBase.Value * 100f), BandolierExponent.Value)) - Mathf.Round(1f - 1f / Mathf.Pow(1f + (BandolierBase.Value * 100f), BandolierExponent.Value));
            LanguageAPI.Add("ITEM_BANDOLIER_DESC", "<style=cIsUtility>" + d(imsosickandtiredofthishit) + "</style> <style=cStack>(+" + d(notmoddingbutpeopleingeneral) + " on stack)</style> chance on kill to drop an ammo pack that <style=cIsUtility>resets all skill cooldowns</style>.");

            bool bArmorBuff = BerzerkersBuffArmor.Value != 0f;
            bool bArmor = BerzerkersArmorAlways.Value != 0f;
            var imtrash = BerzerkersDurationBase.Value + BerzerkersDurationStack.Value;
            //LanguageAPI.Add("ITEM_WARCRYONMULTIKILL_PICKUP", "Enter a frenzy after killing " + BerzerkersKillsReq.Value + " enemies in quick succession.");
            LanguageAPI.Add("ITEM_WARCRYONMULTIKILL_DESC",
            (bArmor ? "<style=cIsHealing>Increase armor</style> by <style=cIsHealing>" + BerzerkersArmorAlways.Value + "</style> <style=cStack>(+" + BerzerkersArmorAlways.Value + " per stack)</style>. " : "") +
            // "<style=cIsDamage>Killing " + BerzerkersKillsReq.Value + " enemies</style> within <style=cIsDamage>1</style> second sends you into a <style=cIsDamage>frenzy</style> for <style=cIsDamage>" + imtrash + "s</style> <style=cStack>(+" + BerzerkersDurationStack.Value + "s per stack)</style>. Increases <style=cIsUtility>movement speed</style> by <style=cIsUtility>50%</style>, <style=cIsDamage>attack speed</style> by <style=cIsDamage>100%</style>" +
            "<style=cIsDamage>Killing 4 enemies</style> within <style=cIsDamage>1</style> second sends you into a <style=cIsDamage>frenzy</style> for <style=cIsDamage>" + imtrash + "s</style> <style=cStack>(+" + BerzerkersDurationStack.Value + "s per stack)</style>. Increases <style=cIsUtility>movement speed</style> by <style=cIsUtility>50%</style>, <style=cIsDamage>attack speed</style> by <style=cIsDamage>100%</style>" +
            (bArmorBuff ? " and <style=cIsHealing>armor</style> by <style=cIsHealing>" + BerzerkersBuffArmor.Value + "</style>." : ""));

            bool cStack = ChronobaubleStacking.Value;
            bool cAS = ChronobaubleAS.Value != 0f;
            LanguageAPI.Add("ITEM_SLOWONHIT_DESC", "<style=cIsUtility>Slow</style> enemies on hit for <style=cIsUtility>-60% movement speed</style>" +
            (cAS ? " and <style=cIsDamage>-" + d(ChronobaubleAS.Value) + " attack speed</style>" +
            (cStack ? " <style=cStack>(-" + d(ChronobaubleAS.Value) + " per stack)</style>" : "") : "") +
            " for <style=cIsUtility>2s</style> <style=cStack>(+2s per stack)</style>.");

            bool pleaseendthis = DeathMarkChanges.Value;
            if (pleaseendthis)
            {
                var whitney = DeathMarkDmgIncreasePerDebuff.Value * DeathMarkStackBonus.Value;
                LanguageAPI.Add("ITEM_DEATHMARK_PICKUP", "Enemies with " + DeathMarkMinimumDebuffsRequired.Value + " or more debuffs are marked for death, taking bonus damage.");
                LanguageAPI.Add("ITEM_DEATHMARK_DESC", "Enemies with <style=cIsDamage>" + DeathMarkMinimumDebuffsRequired.Value + "</style> or more debuffs are <style=cIsDamage>marked for death</style>, increasing damage taken by <style=cIsDamage>" + d(DeathMarkDmgIncreasePerDebuff.Value) + "</style> <style=cStack>(+" + d(whitney) + " per stack)</style> per debuff from all sources for <style=cIsUtility>7</style> <style=cStack>(+7 per stack)</style> seconds.");
            }

            LanguageAPI.Add("ITEM_EQUIPMENTMAGAZINE_DESC", "Hold an <style=cIsUtility>additional equipment charge</style> <style=cStack>(+1 per stack)</style>. <style=cIsUtility>Reduce equipment cooldown</style> by <style=cIsUtility>" + d(FuelCellCDR.Value) + "</style> <style=cStack>(+" + d(FuelCellCDR.Value) + " per stack)</style>.");

            LanguageAPI.Add("ITEM_BONUSGOLDPACKONKILL_DESC", "<style=cIsUtility>" + GhorsTomeChance.Value + "%</style> <style=cStack>(+" + GhorsTomeChance.Value + "% on stack)</style> chance on kill to drop a treasure worth <style=cIsUtility>$" + GhorsTomeReward.Value + "</style>. <style=cIsUtility>Scales over time.</style>");

            bool hStack = HarvestersCritStack.Value;
            var pain = HarvestersHealBase.Value + HarvestersHealStack.Value;
            LanguageAPI.Add("ITEM_HEALONCRIT_DESC", "Gain <style=cIsDamage>" + HarvestersCrit.Value + "% critical chance</style>" +
            (hStack ? " <style=cStack>(+" + HarvestersCrit.Value + "% per stack)</style>. " : "") +
            "<style=cIsDamage>Critical strikes</style> <style=cIsHealing>heal</style> for <style=cIsHealing>" + pain + "</style> <style=cStack>(+" + HarvestersHealStack.Value + " per stack)</style> <style=cIsHealing>health</style>.");

            bool useBaseHealthInfusion = InfusionBaseHealth.Value != 0f;
            bool usePercentHealthInfusion = InfusionPercentHealth.Value != 0f;
            bool useBothHealthInfusion = useBaseHealthInfusion && usePercentHealthInfusion;
            bool useScalingInfusion = InfusionScaling.Value == true;
            LanguageAPI.Add("ITEM_INFUSION_PICKUP",
            $"Gain" +
            (usePercentHealthInfusion ? $" {d(InfusionPercentHealth.Value)}" : "") +
            (useBothHealthInfusion ? $" +" : "") +
            (useBaseHealthInfusion ? $" {InfusionBaseHealth.Value}" : "") +
            $" max health. Killing an enemy permanently increases your maximum health, up to 100" +
            (useScalingInfusion ? $", scales with level" : "") +
            $".");
            LanguageAPI.Add("ITEM_INFUSION_DESC",
            $"Increases <style=cIsHealing>maximum health</style> by" +
            (usePercentHealthInfusion ? $" <style=cIsHealing>{d(InfusionPercentHealth.Value)}</style> <style=cStack>(+{d(InfusionPercentHealth.Value)} per stack)</style>" : "") +
            (useBothHealthInfusion ? $" +" : "") +
            (useBaseHealthInfusion ? $" <style=cIsHealing>{InfusionBaseHealth.Value}</style> <style=cStack>(+{InfusionBaseHealth.Value} per stack)</style>" : "") +
            $". Killing an enemy increases your <style=cIsHealing>health permanently</style>" +
            $" by <style=cIsHealing>1</style> <style=cStack>(+1 per stack)</style>," +
            $" up to a <style=cIsHealing>maximum</style> of" +
            $" <style=cIsHealing>100 <style=cStack>(+100 per stack)</style> health</style>." +
            (useScalingInfusion ? $" Scales with level." : ""));
            /*
            bool kBaseDamage = KjaroBaseDamage.Value != 0f;
            LanguageAPI.Add("ITEM_FIRERING_DESC", "Hits that deal <style=cIsDamage>more than 400% damage</style> also blasts enemies with a <style=cIsDamage>runic flame tornado</style>, dealing <style=cIsDamage>" + d(KjaroTotalDamage.Value) + "</style> <style=cStack>(+" + d(KjaroTotalDamage.Value) + " per stack)</style> TOTAL damage over time" +
            (kBaseDamage ? " and <style=cIsDamage>" + d(KjaroBaseDamage.Value) + "</style> <style=cStack>(+" + d(KjaroBaseDamage.Value) + " per stack)</style> base damage." : "") +
            " Recharges every <style=cIsUtility>10</style> seconds.");
            */

            var whatthefuckdoievennametheseanymore = Mathf.Round(1f - 100f / (100f + OldGThreshold.Value * 100f));
            var aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa = Mathf.Round(1f - 100f / (100f + OldGThreshold.Value * 200f));
            LanguageAPI.Add("ITEM_EXECUTELOWHEALTHELITE_DESC", "Instantly kill Elite monsters below <style=cIsHealth>" + d(whatthefuckdoievennametheseanymore) + " <style=cStack>(+" + d(aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa) + " per stack)</style> health</style>.");

            bool oArmor = OldWarArmor.Value != 0f;
            bool oArmorStack = OldWarArmorStack.Value == true;
            LanguageAPI.Add("ITEM_PHASING_DESC", "Falling below <style=cIsHealth>" + d(GlobalLowHealthThreshold.Value) + " health</style> causes you to gain <style=cIsUtility>40% movement speed</style>" +
            (oArmor ? ", <style=cIsHealing>" + OldWarArmor.Value + " armor</style>" +
            (oArmorStack ? " <style=cStack>(+" + OldWarArmor.Value + " per stack)</style>" : "") : "") +
            " and <style=cIsUtility>invisibility</style> for <style=cIsUtility>5s</style>. Recharges every <style=cIsUtility>30 seconds</style> <style=cStack>(-50% per stack)</style>.");

            // TODO: Predatory

            LanguageAPI.Add("ITEM_SPRINTOUTOFCOMBAT_DESC", "Leaving combat boosts your <style=cIsUtility>movement speed</style> by <style=cIsUtility>" + d(RedWhipSpeed.Value) + "</style> <style=cStack>(+" + d(RedWhipSpeed.Value) + " per stack)</style>.");

            bool rbArmor = RoseBucklerArmorAlways.Value != 0;
            bool rbBehavior = ReplaceRoseBucklerSprintWithHpThreshold.Value == true;
            LanguageAPI.Add("ITEM_SPRINTARMOR_DESC",
            (rbArmor ? "<style=cIsHealing>Increase armor</style> by <style=cIsHealing>" + RoseBucklerArmorAlways.Value + "</style> <style=cStack>(+" + RoseBucklerArmorAlways.Value + " per stack)</style> and <style=cIsHealing>" + RoseBucklerArmor.Value + "</style> <style=cStack>(+" + RoseBucklerArmor.Value + " per stack)</style> " : "<style=cIsHealing>Increase armor</style> by <style=cIsHealing>" + RoseBucklerArmor.Value + "</style> <style=cStack>(+" + RoseBucklerArmor.Value + " per stack)</style>") +
            " while" +
            (rbBehavior ? " <style=cIsHealth>under " + d(RoseBucklerThreshold.Value) + " health</style>." : " <style=cIsUtility>sprinting</style>."));

            /* bool rBaseDamage = RunaldBaseDamage.Value != 0f;
             LanguageAPI.Add("ITEM_ICERING_DESC", "Hits that deal <style=cIsDamage>more than 400% damage</style> also blasts enemies with a <style=cIsDamage>runic ice blast</style>, <style=cIsUtility>slowing</style> them by <style=cIsUtility>80%</style> for <style=cIsUtility>3s</style> <style=cStack>(+3s per stack)</style> and <style=cIsDamage>" + d(RunaldTotalDamage.Value) + "</style> <style=cStack>(+" + d(RunaldTotalDamage.Value) + " per stack)</style> TOTAL damage" +
             (rBaseDamage ? " and <style=cIsDamage>" + d(RunaldBaseDamage.Value) + "</style> <style=cStack>(+" + d(RunaldBaseDamage.Value) + " per stack)</style> base damage." : "") +
             " Recharges every <style=cIsUtility>10</style> seconds.");

             var wooliegaming = SquidPolypAS.Value * 10f;
             LanguageAPI.Add("ITEM_SQUIDTURRET_DESC", "Activating an interactable summons a <style=cIsDamage>Squid Turret</style> that attacks nearby enemies at <style=cIsDamage>" + wooliegaming + "% <style=cStack>(+" + wooliegaming + "% per stack)</style> attack speed</style>. Lasts <style=cIsUtility>" + SquidPolypDuration.Value + "</style> seconds.");
            */

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
