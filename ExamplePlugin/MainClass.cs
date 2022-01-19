using BepInEx;
using R2API;
using R2API.Utils;
using RoR2;
using RoR2.Skills;
using RoR2.Orbs;
using RoR2.Projectile;
using UnityEngine;
using BepInEx.Configuration;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using UnityEngine.Networking;

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


        // Global
        public static ConfigEntry<float> GlobalCritDamageMultiplier { get; set; }

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

        // Whites 

        public static ConfigEntry<float> AprDamage { get; set; }

        public static ConfigEntry<float> BackupMagCDR { get; set; }

        public static ConfigEntry<float> BisonSteakHealth { get; set; }
        public static ConfigEntry<bool> BisonSteakLevelHealth { get; set; }

        public static ConfigEntry<float> BungusHealingPercent { get; set; }
        public static ConfigEntry<float> BungusHealingPercentStack { get; set; }
        public static ConfigEntry<float> BungusInterval { get; set; }
        public static ConfigEntry<float> BungusRadius { get; set; }
        public static ConfigEntry<float> BungusRadiusStack { get; set; }

        public static ConfigEntry<float> CrowbarDamage { get; set; }
        public static ConfigEntry<float> CrowbarThreshold { get; set; }

        public static ConfigEntry<int> FireworksCount { get; set; }
        public static ConfigEntry<int> FireworksCountStack { get; set; }
        public static ConfigEntry<float> FireworksDamage { get; set; }
        public static ConfigEntry<float> FireworksProcCo { get; set; }

        public static ConfigEntry<float> FocusCrystalDamage { get; set; }
        public static ConfigEntry<float> FocusCrystalRange { get; set; }

        public static ConfigEntry<float> LensMakersCrit { get; set; }

        //public static ConfigEntry<float> MonsterToothFlatHealing { get; set; }
        //public static ConfigEntry<float> MonsterToothPercentHealing { get; set; }

        public static ConfigEntry<float> RapArmor { get; set; }
        public static ConfigEntry<float> RapFlatDmgDecrease { get; set; }
        public static ConfigEntry<float> RapMinimumDmgTaken { get; set; }

        public static ConfigEntry<float> StickyBombChance { get; set; }
        public static ConfigEntry<float> StickyBombDamage { get; set; }
        public static ConfigEntry<float> StickyBombDelay { get; set; }
        public static ConfigEntry<bool> StickyBombFalloff { get; set; }
        public static ConfigEntry<float> StickyBombRadius { get; set; }

        public static ConfigEntry<float> TougherTimesArmor { get; set; }
        public static ConfigEntry<float> TougherTimesBlockChance { get; set; }

        public static ConfigEntry<float> WarbannerDamage { get; set; }
        public static ConfigEntry<float> WarbannerRadius { get; set; }
        public static ConfigEntry<float> WarbannerRadiusStack { get; set; }

        // Greens

        public static ConfigEntry<float> InfusionBaseCap { get; set; }
        public static ConfigEntry<float> InfusionBaseHealth { get; set; }
        public static ConfigEntry<float> InfusionPercentHealth { get; set; }
        public static ConfigEntry<bool> InfusionScaling { get; set; }

        public static ConfigEntry<float> KjaroBaseDamage { get; set; }
        public static ConfigEntry<float> KjaroTotalDamage { get; set; }
        public static ConfigEntry<float> RunaldBaseDamage { get; set; }
        public static ConfigEntry<float> RunaldTotalDamage { get; set; }

        public static ConfigEntry<bool> ReplaceRoseBucklerSprintWithHpThreshold { get; set; }
        public static ConfigEntry<int> RoseBucklerArmor { get; set; }
        public static ConfigEntry<int> RoseBucklerArmorAlways { get; set; }
        public static ConfigEntry<float> RoseBucklerThreshold { get; set; }

        // Lunars

        public static ConfigEntry<int> VisionsCharges { get; set; }
        public static ConfigEntry<float> VisionsCooldown { get; set; }
        public static ConfigEntry<float> VisionsInitialHitDamage { get; set; }
        public static ConfigEntry<float> VisionsInitialProcCoefficient { get; set; }

        public static ConfigEntry<int> HooksCharges { get; set; }
        public static ConfigEntry<float> HooksCooldown { get; set; }
        public static ConfigEntry<float> HooksProcCoefficient { get; set; }
        public static ConfigEntry<float> HooksExplosionDamage { get; set; }
        public static ConfigEntry<float> HooksExplosionProcCoefficient { get; set; }
        public static ConfigEntry<float> HooksExplosionRadius { get; set; }

        public void Awake()
        {
            // Global
            GlobalCritDamageMultiplier = Config.Bind<float>(":: Global : Damage ::", "Crit Damage Multiplier", (float)2f, "The Crit Damage Multiplier, Vanilla is 2");

            PlayerFactorBase = Config.Bind<float>(":: Global :: Scaling ::", "Player Factor Base", (float)0.7f, "The Base Value of the Player Factor, Vanilla is 0.7");
            PlayerCountMultiplier = Config.Bind<float>(":: Global :: Scaling ::", "Player Count Multiplier", (float)0.3f, "The Multiplier of the Player Count, Vanilla is 0.3");
            PlayerCountExponent = Config.Bind<float>(":: Global :: Scaling ::", "Player Count Exponent", (float)0.2f, "The Exponent of the Player Count, Vanilla is 0.2");
            AdditiveStageScaling = Config.Bind<bool>(":: Global :: Scaling ::", "Additive Scaling on Stage Entry", (bool)false, "Should there be Additive Scaling on Stage Entry?, Vanilla is false");
            AdditiveStageScalingBase = Config.Bind<float>(":: Global :: Scaling ::", "Additive Stage Scaling Add", (float)0f, "How many Seconds to add on Stage Entry?, Vanilla is 0");
            ExponentialStageScaling = Config.Bind<bool>(":: Global :: Scaling ::", "Exponential Scaling on Stage Entry", (bool)true, "Should there be Exponential Scaling on Stage Entry?, Vanilla is true");
            ExponentialStageScalingCount = Config.Bind<float>(":: Global :: Scaling ::", "Exponential Stage Scaling Count", (float)1f, "Every Nth Stage Scales the Difficulty Exponentially, Vanilla is 1");
            ExponentialStageScalingBase = Config.Bind<float>(":: Global :: Scaling ::", "Exponential Stage Scaling Exponent", (float)1.15f, "The Exponent of Exponential Scaling, Vanilla is 1.15");
            TimeFactorMultiplier = Config.Bind<float>(":: Global :: Scaling ::", "Time Factor Multiplier", (float)0.0506f, "The Time Factor Multiplier of Scaling, Vanilla is 0.0506");
            AmbientLevelCap = Config.Bind<int>(":: Global :: Scaling ::", "Ambient Level Cap", (int)99, "The Ambient Level Cap of Monsters, Vanilla is 99");
            GoldRewardExponent = Config.Bind<float>(":: Global :: Scaling ::", "Gold Reward Exponent", (float)1f, "The Exponent of Gold Awarded on kill, Vanilla is 1");
            GoldRewardDivisorBase = Config.Bind<float>(":: Global :: Scaling ::", "Gold Reward Divisor Base", (float)1f, "The Base Divisor of Gold Awarded on kill, Vanilla is 1");
            GoldRewardStageClearCountDivisor = Config.Bind<float>(":: Global :: Scaling ::", "Gold Reward Stage Clear Count Divisor", (float)1f, "The Stage Clear Count Divisor of Gold Awarded on kill, Vanilla is 1");
            Guide = Config.Bind<bool>(":: Global :: Scaling ::", "Time Scaling Guide", (bool)true, "The entire Scaling formula is as follows:\n(Player Factor Base + Player Count * Player Count Multiplier + \nDifficulty Coefficient Multiplier * Difficulty Def Scaling Value \n(1 for Drizzle, 2 for Rainstorm, 3 for Monsoon) * \nPlayer Count ^ Player Count Exponent * \nTime in Minutes) * Exponential Scaling Stage Base ^ Stages Cleared \nI highly recommend changing Gold Scaling while changing these as well");
            Guide2 = Config.Bind<bool>(":: Global :: Scaling ::", "Gold Scaling Guide", (bool)true, "The entire Scaling formula is as follows:\n(Gold Reward ^ Gold Reward Exponent) / Gold Reward Divisor Base ^ \n(Stage Clear Count / Gold Reward Stage Clear Count Divisor)");
            // Whites
            AprDamage = Config.Bind<float>(":: Items : Whites ::", "Armor-Piercing Rounds Damage Coefficient", (float)0.2f, "Decimal. The Damage Coefficient of Armor-Piercing Rounds, Vanilla is 0.2");

            BackupMagCDR = Config.Bind<float>(":: Items : Whites ::", "Backup Mag Cooldown Reduction", (float)0.05f, "Decimal. The Cooldown Reduction Multiplier of Backup Mag, per stack too, Vanilla is 0");

            BisonSteakHealth = Config.Bind<float>(":: Items : Whites ::", "Bison Steak Health", (float)25f, "The Health Increase of Bison Steak, per stack too, Vanilla is 25");
            BisonSteakLevelHealth = Config.Bind<bool>(":: Items : Whites ::", "Bison Steak Health From Level", (bool)false, "Make Bison Steak give Health from Level too?");

            BungusHealingPercent = Config.Bind<float>(":: Items : Whites ::", "Bustling Fungus Healing Percent", (float)0.045f, "Decimal. The Healing Percent of Bungus, Vanilla is 0.045");
            BungusHealingPercentStack = Config.Bind<float>(":: Items : Whites ::", "Bustling Fungus Healing Percent Per Stack", (float)0.0225f, "Decimal. The Healing Percent of Bungus, per stack, Vanilla is 0.0225");
            BungusInterval = Config.Bind<float>(":: Items : Whites ::", "Bustling Fungus Interval", (float)0.25f, "Decimal. The Interval at which Bustling Fungus heals, Vanilla is 0.25");
            BungusRadius = Config.Bind<float>(":: Items : Whites ::", "Bustling Fungus Radius", (float)1.5f, "The Radius of Bustling Fungus, Vanilla is 1.5");
            BungusRadiusStack = Config.Bind<float>(":: Items : Whites ::", "Bustling Fungus Radius Per Stack", (float)1.5f, "The Radius of Bustling Fungus, per stack, Vanilla is 1.5");

            CrowbarThreshold = Config.Bind<float>(":: Items : Whites ::", "Crowbar Threshold", (float)0.9f, "Decimal. The Threshold of Crowbar to activate, Vanilla is 0.9");
            CrowbarDamage = Config.Bind<float>(":: Items : Whites ::", "Crowbar Damage Coefficient", (float)0.75f, "Decimal. The Damage increase of Crowbar, Vanilla is 0.75");

            FireworksCount = Config.Bind<int>(":: Items : Whites ::", "Fireworks Count", (int)6, "The Amount of Fireworks fired, Vanilla is 4");
            FireworksCountStack = Config.Bind<int>(":: Items : Whites ::", "Fireworks Count Per Stack", (int)6, "The Amount of Fireworks fired, per stack, Vanilla is 4");
            FireworksDamage = Config.Bind<float>(":: Items : Whites ::", "Fireworks Damage Coefficient", (float)3f, "Decimal. The Damage Coefficient of each Firework, Vanilla is 3");
            FireworksProcCo = Config.Bind<float>(":: Items : Whites ::", "Fireworks Proc Coefficient", (float)0.2f, "The Proc Coefficient of each Firework, Vanilla is 0.2");

            FocusCrystalDamage = Config.Bind<float>(":: Items : Whites ::", "Focus Crystal Damage Coefficient", (float)0.2f, "Decimal. The Damage increase of Focus Crystal, per stack too, Vanilla is 0.2");
            FocusCrystalRange = Config.Bind<float>(":: Items : Whites ::", "Focus Crystal Range", (float)13f, "The Range of Focus Crystal, Vanilla is 13");


            LensMakersCrit = Config.Bind<float>(":: Items : Whites ::", "Lens-makers Glasses Crit", (float)10f, "The Crit Chance of Lens-makers Glasses, per stack too, Vanilla is 10");
            //MonsterToothFlatHealing = Config.Bind<float>(":: Items : Whites ::", "Monster Tooth Flat Heal", (float)8f, "The Flat Healing of Monster Tooth, only first stack");
            //MonsterToothPercentHealing = Config.Bind<float>(":: Items : Whites ::", "Monster Tooth Percent Heal", (float)0.04f, "The Percent Healing of Monster Tooth, only first stack");

            RapFlatDmgDecrease = Config.Bind<float>(":: Items : Whites ::", "Repulsion Armor Plate Flat Damage Reduction", (float)5f, "The Flat Damage Reduction of Repulsion Armor Plate, per stack too, Vanilla is 5");
            RapMinimumDmgTaken = Config.Bind<float>(":: Items : Whites ::", "Repulsion Armor Plate Minimum Damage", (float)1f, "The Minimum Damage Taken with Repulsion Armor Plate, Vanilla is 1");
            RapArmor = Config.Bind<float>(":: Items : Whites ::", "Repulsion Armor Plate Armor", (float)0f, "The Armor increase of Repulsion Armor Plate, per stack too, Vanilla is 0");

            StickyBombDamage = Config.Bind<float>(":: Items : Whites ::", "Sticky Bomb Damage Coefficient", (float) 1.8f, "Decimal. The Total Damage of Sticky Bomb, Vanilla is 1.8");
            StickyBombChance = Config.Bind<float>(":: Items : Whites ::", "Sticky Bomb Chance", (float)5f, "The Proc Chance of Sticky Bomb, per stack too, Vanilla is 5");
            StickyBombRadius = Config.Bind<float>(":: Items : Whites ::", "Sticky Bomb Radius", (float)10f, "The Radius of Sticky Bomb, Vanilla is 10");
            StickyBombFalloff = Config.Bind<bool>(":: Items : Whites ::", "Sticky Bomb Falloff Type", (true), "The Falloff Type of Sticky Bomb, if set to true, use Sweetspot, if set to false, use None, Vanilla is Sweetspot");
            StickyBombDelay = Config.Bind<float>(":: Items : Whites ::", "Sticky Bomb Explosion Delay", (float)1.5f, "Decimal. The Explosion Delay of Sticky Bomb, Vanilla is 1.5");

            TougherTimesBlockChance = Config.Bind<float>(":: Items : Whites ::", "Tougher Times Block Chance", (float)15f, "The Block Chance of Tougher Times, per stack too, Vanilla is 15");
            TougherTimesArmor = Config.Bind<float>(":: Items : Whites ::", "Tougher Times Armor", (float)0f, "The Armor increase of Tougher Times, per stack too. Vanilla is 0");

            WarbannerDamage = Config.Bind<float>(":: Items : Whites ::", "Warbanner Base Damage", (float)16, "The Base Damage increase of Warbanner in its radius, per stack too, Vanilla is 0");
            WarbannerRadius = Config.Bind<float>(":: Items : Whites ::", "Warbanner Base Radius", (float)16, "The Base Radius of Warbanner in its radius, Vanilla is 8");
            WarbannerRadiusStack = Config.Bind<float>(":: Items : Whites ::", "Warbanner Radius Per Stack", (float)16, "The Radius of Warbanner in its radius, per stack, Vanilla is 8");

            // Greens

            InfusionScaling = Config.Bind<bool>(":: Items :: Greens ::", "Infusion Level Scaling Cap", (bool)false, "Use Infusion's Level Scaling Cap? The formula is: \nInfusion Base Cap * 1 + 0.3 * (Level - 1) * Infusion Count");
            InfusionBaseCap = Config.Bind<float>(":: Items :: Greens ::", "Infusion Base Cap", (float)30f, "The Base Cap of Infusion, used for the Config Option above");
            InfusionBaseHealth = Config.Bind<float>(":: Items :: Greens ::", "Infusion Base Health", (float)25f, "The Max Health Increase of Infusion, per stack too, Vanilla is 0");
            InfusionPercentHealth = Config.Bind<float>(":: Items :: Greens ::", "Infusion Percent Health", (float)0.25f, "Decimal. The Percentage Max Health Increase of Infusion, per stack too, Vanilla is 0");

            KjaroTotalDamage = Config.Bind<float>(":: Items :: Greens ::", "Kjaros Band Total Damage", (float)3f, "Decimal. The Total Damage Increase of Kjaro's Band, per stack too, Vanilla is 3");
            KjaroBaseDamage = Config.Bind<float>(":: Items :: Greens ::", "Kjaros Band Base Damage", (float)0f, "Decimal. The Base Damage Increase of Kjaro's Band, per stack too, Vanilla is 0");
            RunaldTotalDamage = Config.Bind<float>(":: Items :: Greens ::", "Runalds Band Total Damage", (float)2.5f, "Decimal. The Total Damage Increase of Runald's Band, per stack too, Vanilla is 2.5");
            RunaldBaseDamage = Config.Bind<float>(":: Items :: Greens ::", "Runalds Band Base Damage", (float)0f, "Decimal. The Base Damage Increase of Runald's Band, per stack too, Vanilla is 0");

            ReplaceRoseBucklerSprintWithHpThreshold = Config.Bind<bool>(":: Items :: Greens ::", "Rose Buckler Disable Sprinting, Enable Health Threshold", (bool)false, "Replace Rose Buckler's Condition to be 'Below X% Health' instead?");
            RoseBucklerThreshold = Config.Bind<float>(":: Items :: Greens ::", "Rose Buckler Health Threshold", (float)0.5f, "Decimal. The Health Threshold of Rose Buckler to be active");
            RoseBucklerArmor = Config.Bind<int>(":: Items :: Greens ::", "Rose Buckler Armor", (int)30, "The Armor Increase of Rose Buckler while sprinting, per stack too, Vanilla is 30");
            RoseBucklerArmorAlways = Config.Bind<int>(":: Items :: Greens ::", "Rose Buckler Unconditional Armor", (int)0, "The Armor Increase of Rose Buckler unconditionally, per stack too, Vanilla is 0");

            // Lunars
            VisionsCharges = Config.Bind<int>(":: Items :::: Lunars ::", "Visions of Heresy Charges", (int)24, "The amount of Charges of Visions of Heresy, Vanilla is 12");
            VisionsCooldown = Config.Bind<float>(":: Items :::: Lunars ::", "Visions of Heresy Cooldown", (float)1f, "The Cooldown of Visions of Heresy, Vanilla is 2");
            VisionsInitialHitDamage = Config.Bind<float>(":: Items :::: Lunars ::", "Visions of Heresy Intial Damage", (float)0.05f, "Decimal. The Initial Damage per hit of Visions of Heresy, Vanilla is 0.05");
            VisionsInitialProcCoefficient = Config.Bind<float>(":: Items :::: Lunars ::", "Visions of Heresy Intial Proc Coefficient", (float)0.1f, "Decimal. The Initial Proc Coefficient per hit of Visions of Heresy, Vanilla is 0.1");

            HooksCharges = Config.Bind<int>(":: Items :::: Lunars ::", "Hooks of Heresy Charges", (int)1, "The amount of Charges of Hooks of Heresy, Vanilla is 1");
            HooksCooldown = Config.Bind<float>(":: Items :::: Lunars ::", "Hooks of Heresy Cooldown", (float)5f, "The Cooldown of Hooks of Heresy, Vanilla is 5");
            HooksProcCoefficient = Config.Bind<float>(":: Items :::: Lunars ::", "Hooks of Heresy Proc Coefficient", (float)0.2f, "Decimal. The Proc Coefficient of Hooks of Heresy, Vanilla is 0.2");
            HooksExplosionProcCoefficient = Config.Bind<float>(":: Items :::: Lunars ::", "Hooks of Heresy Explosion Proc Coefficient", (float)1f, "Decimal. The Proc Coefficient of Hooks of Heresy's Explosion, Vanilla is 1");
            HooksExplosionRadius = Config.Bind<float>(":: Items :::: Lunars ::", "Hooks of Heresy Explosion Radius", (float)14f, "The Radius of Hooks of Heresy's Explosion, Vanilla is 14");
            HooksExplosionDamage = Config.Bind<float>(":: Items :::: Lunars ::", "Hooks of Heresy Explosion Damage", (float)7f, "Decimal. The Damage of Hooks of Heresy's Explosion, Vanilla is 7.");
            ChangeDescriptions();

            // Global

            IL.RoR2.HealthComponent.TakeDamage += GlobalCritMultChange;

            ChangeScaling();

            // Whites

            IL.RoR2.HealthComponent.TakeDamage += AprDamageChange;

            RecalculateStatsAPI.GetStatCoefficients += BackupMagCDRAdd;

            IL.RoR2.CharacterBody.RecalculateStats += BisonSteakChangeHealth;
            if (BisonSteakLevelHealth.Value == true)
            {
                RecalculateStatsAPI.GetStatCoefficients += BisonSteakLevelHealthAdd;
            }

            IL.RoR2.CharacterBody.MushroomItemBehavior.FixedUpdate += BungusIntervalAndHealingPercentChange;
            IL.RoR2.CharacterBody.MushroomItemBehavior.FixedUpdate += BungusRadiusChange;

            IL.RoR2.HealthComponent.TakeDamage += CrowbarDamageChange;
            IL.RoR2.HealthComponent.TakeDamage += CrowbarThresholdChange;

            IL.RoR2.GlobalEventManager.OnInteractionBegin += FireworksStackChange;
            FireworksChanges();

            IL.RoR2.HealthComponent.TakeDamage += FocusCrystalDamageChange;
            IL.RoR2.HealthComponent.TakeDamage += FocusCrystalRangeChange;
            FocusCrystalRangeVisualChange();

            IL.RoR2.CharacterBody.RecalculateStats += LensMakersGlassesChangeCrit;

            //IncreaseMonsterToothHealing();

            RecalculateStatsAPI.GetStatCoefficients += RapArmorAdd;
            IL.RoR2.HealthComponent.TakeDamage += RapFlatDmgDecreaseChange;
            IL.RoR2.HealthComponent.TakeDamage += RapFlatDmgMinimumChange;

            StickyBombChanges();
            IL.RoR2.GlobalEventManager.OnHitEnemy += StickyBombChanceChange;
            // IL.RoR2.SetStateOnHurt.OnTakeDamageServer += StunGrenadeHyperbolicToLinear;

            RecalculateStatsAPI.GetStatCoefficients += TougherTimesArmorAdd;
            IL.RoR2.HealthComponent.TakeDamage += TougherTimesBlockChanceChange;

            RecalculateStatsAPI.GetStatCoefficients += WarbannerDamageAdd;
            IL.RoR2.TeleporterInteraction.ChargingState.OnEnter += WarbannerRadiusChangeTP;
            IL.RoR2.Items.WardOnLevelManager.OnCharacterLevelUp += WarbannerRadiusChange;

            // Greens

            RecalculateStatsAPI.GetStatCoefficients += InfusionBaseHealthAdd;
            if (InfusionScaling.Value == true)
            {
                IL.RoR2.GlobalEventManager.OnCharacterDeath += InfusionChange;
            }
            RecalculateStatsAPI.GetStatCoefficients += InfusionPercentHealthAdd;

            //IL.RoR2.GlobalEventManager.OnHitEnemy += KjaroChange;
            //IL.RoR2.GlobalEventManager.OnHitEnemy += RunaldChange;

            if (ReplaceRoseBucklerSprintWithHpThreshold.Value == true)
            {
                IL.RoR2.CharacterBody.RecalculateStats += RoseBucklerConditionChange;
                RoseBucklerHpThresholdCheckYesThisIsHorribleAndInsaneCryAboutIt();
            }
            IL.RoR2.CharacterBody.RecalculateStats += RoseBucklerArmorChange;
            RecalculateStatsAPI.GetStatCoefficients += RoseBucklerUnconditionalArmorAdd;

            // Lunars

            VisionsChanges();
        }

        public static string d(float f)
        {
            return (f * 100).ToString() + "%";
        }

        // Global

        private void GlobalCritMultChange(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.GotoNext(MoveType.Before,
                x => x.MatchLdfld<DamageInfo>("crit"),
                x => x.MatchBrfalse(out _),
                x => x.MatchLdloc(out _),
                x => x.MatchLdcR4(2f)
            );
            c.Index += 3;
            c.Next.Operand = GlobalCritDamageMultiplier.Value;
        }

        private void ChangeScaling()
        {
            On.RoR2.Run.RecalculateDifficultyCoefficentInternal += (orig, self) =>
            {
                int playerCount = self.participatingPlayerCount;
                float time = self.GetRunStopwatch() * 0.016666668f; // stupid vanilla workaround

                DifficultyDef difficultyDef = DifficultyCatalog.GetDifficultyDef(self.selectedDifficulty);
                float playerFactor = PlayerFactorBase.Value + playerCount * PlayerCountMultiplier.Value;
                float timeFactor = time * TimeFactorMultiplier.Value * difficultyDef.scalingValue;
                float playerScalar = (float)Math.Pow(playerCount, PlayerCountExponent.Value);
                float stageFactor = 1f;
                if (ExponentialStageScaling.Value && AdditiveStageScaling.Value == false)
                {
                    stageFactor = Mathf.Pow(ExponentialStageScalingBase.Value, self.stageClearCount / ExponentialStageScalingCount.Value);
                }
                else if (AdditiveStageScaling.Value && ExponentialStageScaling.Value == false)
                {
                    stageFactor = 1f;
                    time += AdditiveStageScalingBase.Value;
                }
                else if (ExponentialStageScaling.Value && AdditiveStageScaling.Value)
                {
                    stageFactor = Mathf.Pow(ExponentialStageScalingBase.Value, self.stageClearCount / ExponentialStageScalingCount.Value);
                    time += AdditiveStageScalingBase.Value;
                }
                else
                {
                    stageFactor = 1f;
                }
                float finalDifficulty = (playerFactor + timeFactor * playerScalar) * stageFactor;
                self.compensatedDifficultyCoefficient = finalDifficulty;
                self.difficultyCoefficient = finalDifficulty;
                self.ambientLevel = Mathf.Min(3f * (finalDifficulty - playerFactor) + 1f, Run.ambientLevelCap);

                //Vanilla code
                Run.ambientLevelCap = AmbientLevelCap.Value;
                int ambientLevelFloor = self.ambientLevelFloor;
                self.ambientLevelFloor = Mathf.FloorToInt(self.ambientLevel);
                if (ambientLevelFloor != self.ambientLevelFloor && ambientLevelFloor != 0 && self.ambientLevelFloor > ambientLevelFloor)
                {
                    self.OnAmbientLevelUp();
                }

            };
            On.RoR2.DeathRewards.OnKilledServer += (orig, self, damageReport) =>
            {
                self.goldReward = (uint)Mathf.CeilToInt(Mathf.Pow(self.goldReward, GoldRewardExponent.Value) / Mathf.Pow(GoldRewardDivisorBase.Value, Run.instance.stageClearCount / GoldRewardStageClearCountDivisor.Value));
                orig(self, damageReport);
            };
        }

        // Whites

        private void AprDamageChange(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(1),
                x => x.MatchLdcR4(0.2f)
            );
            c.Index += 1;
            c.Next.Operand = AprDamage.Value;
        }
        private void BackupMagCDRAdd(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.inventory)
            {
                var stack = sender.inventory.GetItemCount(RoR2Content.Items.SecondarySkillMagazine);
                if (stack > 0)
                {
                    args.secondaryCooldownMultAdd -= BackupMagCDR.Value * stack;
                }
            }
        }

        private void BisonSteakChangeHealth(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.GotoNext(MoveType.Before,
                x => x.MatchConvR4(),
                x => x.MatchLdcR4(25f)
            );
            c.Index += 1;
            c.Next.Operand = BisonSteakHealth.Value;
        }

        private void BisonSteakLevelHealthAdd(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.inventory)
            {
                var stack = sender.inventory.GetItemCount(RoR2Content.Items.FlatHealth);
                if (stack > 0)
                {
                    args.baseHealthAdd += sender.levelMaxHealth * stack;
                }
            }
        }

        private void BungusRadiusChange(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.GotoNext(MoveType.Before,
                x => x.MatchCallOrCallvirt<CharacterBody>("get_radius"),
                x => x.MatchLdcR4(1.5f),
                x => x.MatchAdd(),
                x => x.MatchLdcR4(1.5f)
            );
            c.Index += 1;
            c.Next.Operand = BungusRadius.Value;
            c.Next.Operand = BungusRadiusStack.Value;
        }

        private void BungusIntervalAndHealingPercentChange(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(0.25f),
                x => x.MatchStfld<HealingWard>("interval"),
                x => x.MatchLdloc(out _),
                x => x.MatchLdcR4(0.045f),
                x => x.MatchLdcR4(0.0225f)
            );
            c.Next.Operand = BungusInterval.Value;
            c.Index += 3;
            c.Next.Operand = BungusHealingPercent.Value;
            c.Index += 1;
            c.Next.Operand = BungusHealingPercentStack.Value;
        }

        private void CrowbarDamageChange(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(1f),
                x => x.MatchLdcR4(0.75f)
            );
            c.Index += 1;
            c.Next.Operand = CrowbarDamage.Value;
        }
        private void CrowbarThresholdChange(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.GotoNext(MoveType.Before,
                x => x.MatchLdarg(0),
                x => x.MatchCallOrCallvirt<HealthComponent>("get_fullCombinedHealth"),
                x => x.MatchLdcR4(0.9f)
            );
            c.Index += 2;
            c.Next.Operand = CrowbarThreshold.Value;
        }

        private void FireworksStackChange(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            //c.GotoNext(MoveType.Before,
            //x => x.MatchLdcI4(4),
            //x => x.MatchLdloc(out _),
            //x => x.MatchLdcI4(4)
            c.GotoNext(x => x.MatchStfld<RoR2.FireworkLauncher>("remaining")

            );
            //c.Next.Operand = FireworksCount.Value;
            //c.Index += 2;
            //c.Next.Operand = FireworksCountStack.Value;
            c.EmitDelegate<Func<int, int>>((val) =>
            {
                return FireworksCount.Value + ((val - 4) / 4) * FireworksCountStack.Value;
            });

        }

        private void FireworksChanges()
        {
            var furrywork = Resources.Load<GameObject>("prefabs/projectiles/FireworkProjectile");
            var croppa = furrywork.GetComponent<ProjectileImpactExplosion>();
            var croppa2 = furrywork.GetComponent<ProjectileController>();
            croppa.blastDamageCoefficient = FireworksDamage.Value / 3f;
            croppa2.procCoefficient = FireworksProcCo.Value;
        }
        private void FocusCrystalDamageChange(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.GotoNext(MoveType.Before,
                x => x.MatchStfld<DamageInfo>("damageColorIndex"),
                x => x.MatchLdloc(out _),
                x => x.MatchLdcR4(1f),
                x => x.MatchLdloc(out _),
                x => x.MatchConvR4(),
                x => x.MatchLdcR4(0.2f)
            );
            c.Index += 5;
            c.Next.Operand = FocusCrystalDamage.Value;
        }
        private void FocusCrystalRangeChange(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.GotoNext(MoveType.Before,
                x => x.MatchCallOrCallvirt<UnityEngine.Vector3>("get_sqrMagnitude"),
                x => x.MatchLdcR4(169f)
            );
            c.Index += 1;
            c.Next.Operand = FocusCrystalRange.Value * FocusCrystalRange.Value;
        }
        private void FocusCrystalRangeVisualChange()
        {
            var focus = Resources.Load<GameObject>("Prefabs/NetworkedObjects/NearbyDamageBonusIndicator");
            float actualRange = FocusCrystalRange.Value / 13;
            focus.transform.localScale = new Vector3(actualRange, actualRange, actualRange);
        }
        private void LensMakersGlassesChangeCrit(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.GotoNext(MoveType.Before,
                x => x.MatchLdfld<CharacterBody>("levelCrit")
            );
            c.Index += 8;
            c.Next.Operand = LensMakersCrit.Value;
        }

        //private void IncreaseMonsterToothHealing()
        //{
        //    var MonsterTooth = Resources.Load<GameObject>("Prefabs/NetworkedObjects/HealPack");
        //    HealthPickup cic = MonsterTooth.GetComponentInChildren<HealthPickup>();
        //    cic.flatHealing = MonsterToothFlatHealing.Value;
        //    cic.fractionalHealing = MonsterToothPercentHealing.Value;
        //}

        private void RapArmorAdd(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.inventory)
            {
                var stack = sender.inventory.GetItemCount(RoR2Content.Items.ArmorPlate);
                if (stack > 0)
                {
                    args.armorAdd += RapArmor.Value * stack;
                }
            }
        }

        private void RapFlatDmgDecreaseChange(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(1),
                x => x.MatchLdloc(out _),
                x => x.MatchLdcR4(5)
            );
            c.Index += 2;
            c.Next.Operand = RapFlatDmgDecrease.Value;
        }

        private void RapFlatDmgMinimumChange(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.GotoNext(MoveType.Before,
                //x => x.MatchLdflda<HealthComponent>("itemCounts"),
                //x => x.MatchLdfld<HealthComponent>("armorPlate"),
                x => x.MatchLdcI4(0),
                x => x.MatchBle(out _),
                x => x.MatchLdcR4(1)
            );
            c.Index += 2;
            c.Next.Operand = RapMinimumDmgTaken.Value;
        }
        private void StickyBombChanges()
        {
            var nowthatssticky = Resources.Load<GameObject>("prefabs/projectiles/stickybomb");
            var s = nowthatssticky.GetComponent<ProjectileImpactExplosion>();
            s.blastDamageCoefficient = StickyBombDamage.Value / 1.8f;
            s.blastRadius = StickyBombRadius.Value;
            s.lifetime = StickyBombDelay.Value;
            if (StickyBombFalloff.Value)
            {
                s.falloffModel = BlastAttack.FalloffModel.SweetSpot;
            }
            else
            {
                s.falloffModel = BlastAttack.FalloffModel.None;
            }
        }
        private void StickyBombChanceChange(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.GotoNext(MoveType.Before,
                x => x.MatchBle(out _),
                x => x.MatchLdcR4(5)
            );
            c.Index += 1;
            c.Next.Operand = StickyBombChance.Value;
        }
        private void StunGrenadeHyperbolicToLinear(ILContext il)
        {
            // yeah i have no clue
            ILCursor c = new ILCursor(il);
            c.GotoNext(MoveType.Before,
                x => x.MatchCallOrCallvirt(typeof(RoR2.Util), "ConvertAmplificationPercentageIntoReductionPercentage"),
                x => x.MatchLdloc(out _),
                x => x.MatchCallOrCallvirt(typeof(RoR2.Util), "CheckRoll"),
                x => x.MatchBrfalse(out _),
                x => x.MatchLdstr("Prefabs/Effects/ImpactEffects/ImpactStunGrenade")
            );
            //c.EmitDelegate<Func<>>() => { return };

        }

        private void TougherTimesArmorAdd(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.inventory)
            {
                var stack = sender.inventory.GetItemCount(RoR2Content.Items.Bear);
                if (stack > 0)
                {
                    args.armorAdd += TougherTimesArmor.Value * stack;
                }
            }
        }
        private void TougherTimesBlockChanceChange(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.GotoNext(MoveType.Before,
                 x => x.MatchLdcI4(0),
                 x => x.Match(OpCodes.Ble_S),
                 x => x.MatchLdcR4(15f)
            );
            c.Index += 2;
            c.Next.Operand = TougherTimesBlockChance.Value;
        }
        private void WarbannerDamageAdd(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.inventory)
            {
                var stack = sender.inventory.GetItemCount(RoR2Content.Items.WardOnLevel);
                if (sender.HasBuff(RoR2Content.Buffs.Warbanner.buffIndex))
                {
                    args.baseDamageAdd += WarbannerDamage.Value * stack;
                }
            }
        }
        private void WarbannerRadiusChange(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(8),
                x => x.MatchLdcR4(8)
            );
            c.Next.Operand = WarbannerRadius.Value;
            c.Index += 1;
            c.Next.Operand = WarbannerRadiusStack.Value;
        }
        private void WarbannerRadiusChangeTP(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(8),
                x => x.MatchLdcR4(8)
            );
            c.Next.Operand = WarbannerRadius.Value;
            c.Index += 1;
            c.Next.Operand = WarbannerRadiusStack.Value;
        }

        // Greens

        private void InfusionBaseHealthAdd(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.inventory)
            {
                var stack = sender.inventory.GetItemCount(RoR2Content.Items.Infusion);
                if (stack > 0)
                {
                    args.baseHealthAdd += InfusionBaseHealth.Value * stack;
                }
            }
        }

        private void InfusionPercentHealthAdd(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.inventory)
            {
                var stack = sender.inventory.GetItemCount(RoR2Content.Items.Infusion);
                if (stack > 0)
                {
                    args.healthMultAdd += InfusionPercentHealth.Value * stack;
                }
            }
        }

        private void RunaldChange(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            int itemCountLocation = 51;
            int totalDamageMultiplierLocation = 56;

            c.GotoNext(MoveType.After,
                x => x.MatchLdsfld("RoR2.RoR2Content/Items", "IceRing"),
                x => x.MatchCallOrCallvirt<Inventory>(nameof(Inventory.GetItemCount)),
                x => x.MatchStloc(out itemCountLocation)
                );

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(3f),
                x => x.MatchLdloc(itemCountLocation),
                x => x.MatchConvR4(),
                x => x.MatchMul(),
                x => x.MatchStloc(out totalDamageMultiplierLocation)
                );
            c.Remove();
            c.Emit(OpCodes.Ldc_R4, RunaldTotalDamage.Value);

            c.GotoNext(MoveType.After,
                x => x.MatchLdloc(totalDamageMultiplierLocation),
                x => x.MatchCallOrCallvirt(out _)
                );
            c.Emit(OpCodes.Ldloc_0);
            c.EmitDelegate<Func<float, CharacterBody, float>>((damage, self) =>
            {
                float dam = self.baseDamage * RunaldBaseDamage.Value;

                return damage + dam;
            });
        }

        private void KjaroChange(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            int itemCountLocation = 51;
            int totalDamageMultiplierLocation = 56;

            c.GotoNext(MoveType.After,
                x => x.MatchLdsfld("RoR2.RoR2Content/Items", "FireRing"),
                x => x.MatchCallOrCallvirt<Inventory>(nameof(Inventory.GetItemCount)),
                x => x.MatchStloc(out itemCountLocation)
                );

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(2.5f),
                x => x.MatchLdloc(itemCountLocation),
                x => x.MatchConvR4(),
                x => x.MatchMul(),
                x => x.MatchStloc(out totalDamageMultiplierLocation)
                );
            c.Remove();
            c.Emit(OpCodes.Ldc_R4, KjaroTotalDamage);

            c.GotoNext(MoveType.After,
                x => x.MatchLdloc(totalDamageMultiplierLocation),
                x => x.MatchCallOrCallvirt(out _)
                );
            c.Emit(OpCodes.Ldloc_0);
            c.EmitDelegate<Func<float, CharacterBody, float>>((damage, self) =>
            {
                float dam = self.baseDamage * KjaroBaseDamage.Value;

                return damage + dam;
            });
        }

        private void InfusionChange(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            int bodyLoc = 17;
            int countLoc = 33;
            int capLoc = 47;

            c.GotoNext(MoveType.After,
                x => x.MatchLdsfld("RoR2.RoR2Content/Items", "Infusion"),
                x => x.MatchCallOrCallvirt<RoR2.Inventory>(nameof(RoR2.Inventory.GetItemCount)),
                x => x.MatchStloc(out countLoc)
                );
            c.GotoNext(MoveType.Before,
                x => x.MatchStloc(out capLoc)
                );
            c.Emit(OpCodes.Ldloc, countLoc);
            c.Emit(OpCodes.Ldloc, 13);
            c.EmitDelegate<Func<int, int, RoR2.CharacterBody, int>>((currentInfusionCap, infusionCount, body) =>
            {
                float newInfusionCap = 100 * infusionCount;

                if (body != null)
                {
                    float levelBonus = 1 + 0.3f * (body.level - 1);

                    newInfusionCap = InfusionBaseCap.Value * levelBonus * infusionCount;
                }

                return (int)newInfusionCap;
            });
        }

        private void RoseBucklerConditionChange(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.GotoNext(MoveType.Before,
                x => x.MatchCallOrCallvirt(typeof(CharacterBody).GetPropertyGetter(nameof(CharacterBody.isSprinting)))
            );
            c.GotoNext(MoveType.Before,
                x => x.MatchCallOrCallvirt(typeof(CharacterBody).GetPropertyGetter(nameof(CharacterBody.isSprinting)))
            );
            c.GotoNext(MoveType.After,
                x => x.MatchCallOrCallvirt(typeof(CharacterBody).GetPropertyGetter(nameof(CharacterBody.isSprinting)))
            );
            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate<Func<bool, CharacterBody, bool>>((sprinting,body) => { return body.healthComponent.combinedHealthFraction < RoseBucklerThreshold.Value; });
        }
        private void RoseBucklerArmorChange(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.GotoNext(MoveType.Before,
                x => x.MatchCallOrCallvirt<RoR2.CharacterBody>("get_armor"),
                x => x.MatchLdloc(out _),
                x => x.MatchLdcI4(30)
            );
            c.Index += 3;
            c.EmitDelegate<Func<int, int>>((sdfgsdfhgsghdfv) =>
            { 
                return RoseBucklerArmor.Value;
            });
        }

        private void RoseBucklerVisualChange(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.GotoNext(MoveType.Before,
                x => x.MatchCallOrCallvirt<RoR2.CharacterBody>("get_isSprinting"),
                x => x.MatchBrfalse(out _),
                x => x.MatchLdarg(0),
                x => x.MatchCallOrCallvirt<RoR2.CharacterBody>("get_inventory"),
                x => x.MatchLdsfld<RoR2.ItemDef>("")
            );
        }

        private void RoseBucklerHpThresholdCheckYesThisIsHorribleAndInsaneCryAboutIt()
        {
            On.RoR2.HealthComponent.TakeDamage += (On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo) =>
            {
                float health = self.body.inventory.GetItemCount(RoR2Content.Items.SprintArmor) > 0 ? self.combinedHealthFraction : 0f;
                orig(self, damageInfo);
                if (health >= RoseBucklerThreshold.Value && self.combinedHealthFraction < RoseBucklerThreshold.Value)
                {
                    self.body.statsDirty = true;
                }

            };
            On.RoR2.HealthComponent.Heal += (On.RoR2.HealthComponent.orig_Heal orig, HealthComponent self, float amount, ProcChainMask procChainMask, bool nonRegen) =>
            {
                float health = self.body.inventory.GetItemCount(RoR2Content.Items.SprintArmor) > 0 ? self.combinedHealthFraction : 1f;
                float ret = orig(self, amount, procChainMask, nonRegen);
                if (health < RoseBucklerThreshold.Value && self.combinedHealthFraction >= RoseBucklerThreshold.Value)
                {
                    self.body.statsDirty = true;
                }
                return ret;
            };
        }

        private void RoseBucklerUnconditionalArmorAdd(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.inventory)
            {
                var stack = sender.inventory.GetItemCount(RoR2Content.Items.SprintArmor);
                if (stack > 0)
                {
                    args.armorAdd += RoseBucklerArmorAlways.Value * stack;
                }
            }
        }

        // Lunars

        private void VisionsChanges()
        {
            var vs = Resources.Load<SkillDef>("skilldefs/lunarreplacements/lunarprimaryreplacement");
            vs.baseMaxStock = VisionsCharges.Value;
            vs.baseRechargeInterval = VisionsCooldown.Value;
            vs.rechargeStock = VisionsCharges.Value;
            var vp = Resources.Load<GameObject>("prefabs/projectiles/lunarneedleprojectile").GetComponent<ProjectileController>();
            vp.procCoefficient = VisionsInitialProcCoefficient.Value;
            On.EntityStates.GlobalSkills.LunarNeedle.FireLunarNeedle.OnEnter += (orig,self) =>
            {
                EntityStates.GlobalSkills.LunarNeedle.FireLunarNeedle.damageCoefficient = VisionsInitialHitDamage.Value;
                orig(self);
            };
        }

        private void HooksChanges()
        {
            var hs = Resources.Load<SkillDef>("skilldefs/lunarreplacements/lunarsecondaryreplacement");
            hs.baseMaxStock = HooksCharges.Value;
            hs.baseRechargeInterval = HooksCooldown.Value;
            hs.rechargeStock = HooksCharges.Value;
            var he1 = Resources.Load<GameObject>("prefabs/projectiles/lunarsecondaryprojectile").GetComponent<ProjectileController>();
            var he2 = Resources.Load<GameObject>("prefabs/projectiles/lunarsecondaryprojectile").GetComponent<ProjectileExplosion>();
            he1.procCoefficient = HooksExplosionProcCoefficient.Value;
            he2.blastRadius = HooksExplosionRadius.Value;
            var hs2 = Resources.Load<GameObject>("prefabs/projectiles/lunarsecondaryprojectile").GetComponent<ProjectileDotZone>();
            hs2.overlapProcCoefficient = HooksProcCoefficient.Value;
        }
        private void StridesChanges()
        {
            var ss = Resources.Load<SkillDef>("skilldefs/lunarreplacements/lunarutilityreplacement");
            ss.baseMaxStock = StridesCharges.Value;
            ss.baseRechargeInterval = StridesCooldown.Value;
            ss.rechargeStock = StridesCharges.Value;
            On.EntityStates.GlobalSkills.
        }

        private void ChangeDescriptions()
        {
            LanguageAPI.Add("ITEM_WARDONLEVEL_PICKUP", "Drop a Warbanner on level up or starting the Teleporter event. Grants allies base damage, movement speed and attack speed.");
            if (WarbannerDamage.Value != 0f)
            {
                var j = WarbannerRadius.Value + WarbannerRadiusStack.Value;
                LanguageAPI.Add("ITEM_WARDONLEVEL_DESC", "On <style=cIsUtility>level up</style> or starting the <style=cIsUtility>Teleporter event</style>, drop a banner that strengthens all allies within <style=cIsUtility>" + j + "m</style> <style=cStack>(+" + WarbannerRadiusStack.Value + "m per stack)</style>. Raise <style=cIsDamage>base damage</style> by <style=cIsDamage>" + WarbannerDamage.Value + "</style>, <style=cIsUtility>movement speed</style> and <style=cIsDamage>attack speed</style> by <style=cIsDamage>30%</style>.");
            }
            else
            {
                var j = WarbannerRadius.Value + WarbannerRadiusStack.Value;
                LanguageAPI.Add("ITEM_WARDONLEVEL_DESC", "On <style=cIsUtility>level up</style> or starting the <style=cIsUtility>Teleporter event</style>, drop a banner that strengthens all allies within <style=cIsUtility>" + j + "m</style> <style=cStack>(+" + WarbannerRadiusStack.Value + "m per stack)</style>. Raise <style=cIsDamage>attack</style> and <style=cIsUtility>movement speed</style> by <style=cIsDamage>30%</style>.");
            }
            if (TougherTimesBlockChance.Value != 0f)
            {
                LanguageAPI.Add("ITEM_BEAR_PICKUP", "Chance to block incoming damage. Reduce incoming damage.");
                LanguageAPI.Add("ITEM_BEAR_DESC", "<style=cIsHealing>" + TougherTimesBlockChance.Value + "%</style> <style=cStack>(+" + TougherTimesBlockChance.Value + "% per stack)</style> chance to <style=cIsHealing>block</style> incoming damage. <style=cIsHealing>Increase armor</style> by <style=cIsHealing>" + TougherTimesArmor.Value + "</style> <style=cStack>(+" + TougherTimesArmor.Value + " per stack)</style>. <style=cIsUtility>Unaffected by luck</style>.");
            }
            else
            {
                LanguageAPI.Add("ITEM_BEAR_PICKUP", "Reduce incoming damage.");
                LanguageAPI.Add("ITEM_BEAR_DESC", "<style=cIsHealing>Increase armor</style> by <style=cIsHealing>" + TougherTimesArmor.Value + "</style> <style=cStack>(+" + TougherTimesArmor.Value + " per stack)</style>.");
            }
            LanguageAPI.Add("ITEM_STICKYBOMB_DESC", "<style=cIsDamage>" + StickyBombChance.Value +"%</style> <style=cStack>(+" + StickyBombChance.Value +"% per stack)</style> chance on hit to attach a <style=cIsDamage>bomb</style> to an enemy, detonating for <style=cIsDamage>" + StickyBombDamage.Value * 100f + "%</style> TOTAL damage.");
            LanguageAPI.Add("ITEM_CRITGLASSES_PICKUP", "Chance to 'Critically Strike', dealing " + GlobalCritDamageMultiplier.Value + "x damage.");
            LanguageAPI.Add("ITEM_CRITGLASSES_DESC", "Your attacks have a <style=cIsDamage>" + LensMakersCrit.Value + "%</style> <style=cStack>(+" + LensMakersCrit.Value + "% per stack)</style> chance to '<style=cIsDamage>Critically Strike</style>', dealing <style=cIsDamage>" + GlobalCritDamageMultiplier.Value + "x damage</style>.");
            if (RapArmor.Value != 0f)
            {
                LanguageAPI.Add("ITEM_REPULSIONARMORPLATE_DESC", "<style=cIsHealing>Increase armor</style> by <style=cIsHealing>" + RapArmor.Value + "</style> <style=cStack>(+" + RapArmor.Value + " per stack)</style>. Reduce all <style=cIsDamage>incoming damage</style> by <style=cIsDamage>" + RapFlatDmgDecrease.Value +"<style=cStack> (+" + RapFlatDmgDecrease.Value +" per stack)</style></style>. Cannot be reduced below <style=cIsDamage>" + RapMinimumDmgTaken.Value + "</style>.");
            }
            else
            {
                LanguageAPI.Add("ITEM_REPULSIONARMORPLATE_DESC", "Reduce all <style=cIsDamage>incoming damage</style> by <style=cIsDamage>" + RapFlatDmgDecrease.Value + "<style=cStack> (+" + RapFlatDmgDecrease.Value + " per stack)</style></style>. Cannot be reduced below <style=cIsDamage>" + RapMinimumDmgTaken.Value + "</style>.");
            }
            //LanguageAPI.Add("ITEM_TOOTH_DESC", "Killing an enemy spawns a <style=cIsHealing>healing orb</style> that heals for <style=cIsHealing>" + MonsterToothFlatHealing.Value + "</style> plus an additional <style=cIsHealing>" + MonsterToothPercentHealing.Value * 100f + "% <style=cStack>(+2% per stack)</style></style> of <style=cIsHealing>maximum health</style>.");
            LanguageAPI.Add("ITEM_CROWBAR_PICKUP", "Deal bonus damage to enemies above " + CrowbarThreshold.Value * 100f +"% health.");
            LanguageAPI.Add("ITEM_CROWBAR_DESC", "Deal <style=cIsDamage>+" + CrowbarDamage.Value * 100f + "%</style> <style=cStack>(+" + CrowbarDamage.Value * 100f + "% per stack)</style> damage to enemies above <style=cIsDamage>" + CrowbarThreshold.Value * 100f + "% health</style>.");
            LanguageAPI.Add("ITEM_NEARBYDAMAGEBONUS_DESC", "Increase damage to enemies within <style=cIsDamage>" + FocusCrystalRange.Value + "m</style> by <style=cIsDamage>" + FocusCrystalDamage.Value * 100f + "%</style> <style=cStack>(+" + FocusCrystalDamage.Value * 100f + "% per stack)</style>.");
            if (RoseBucklerArmorAlways.Value != 0 && ReplaceRoseBucklerSprintWithHpThreshold.Value == false)
            {
                LanguageAPI.Add("ITEM_SPRINTARMOR_DESC", "<style=cIsHealing>Increase armor</style> by <style=cIsHealing>" + RoseBucklerArmorAlways.Value + "</style> <style=cStack>(+" + RoseBucklerArmorAlways.Value + " per stack) and <style=cIsHealing>" + RoseBucklerArmor.Value + "</style> <style=cStack>(+" + RoseBucklerArmor.Value + " per stack)</style> while <style=cIsUtility>sprinting</style>.");
            }
            else if (RoseBucklerArmorAlways.Value == 0 && ReplaceRoseBucklerSprintWithHpThreshold.Value == false)
            {
                LanguageAPI.Add("ITEM_SPRINTARMOR_DESC", "<style=cIsHealing>Increase armor</style> by <style=cIsHealing>" + RoseBucklerArmor.Value + "</style> <style=cStack>(+" + RoseBucklerArmor.Value + " per stack)</style> while <style=cIsUtility>sprinting</style>.");
            }
            else if (RoseBucklerArmorAlways.Value != 0 && ReplaceRoseBucklerSprintWithHpThreshold.Value == true)
            {
                LanguageAPI.Add("ITEM_SPRINTARMOR_DESC", "<style=cIsHealing>Increase armor</style> by <style=cIsHealing>" + RoseBucklerArmor.Value + "</style> <style=cStack>(+" + RoseBucklerArmor.Value + " per stack)</style> while <style=cIsHealth>under " + RoseBucklerThreshold.Value * 100f + "% health</style>.");
            }
            else if (RoseBucklerArmorAlways.Value == 0 && ReplaceRoseBucklerSprintWithHpThreshold.Value == true)
            {
                LanguageAPI.Add("ITEM_SPRINTARMOR_DESC", "<style=cIsHealing>Increase armor</style> by <style=cIsHealing>" + RoseBucklerArmorAlways.Value + "</style> <style=cStack>(+" + RoseBucklerArmorAlways.Value + " per stack) and <style=cIsHealing>" + RoseBucklerArmor.Value + "</style> <style=cStack>(+" + RoseBucklerArmor.Value + " per stack)</style> while <style=cIsHealth>under " + RoseBucklerThreshold.Value * 100f + "% health</style>.");
            }
            LanguageAPI.Add("ITEM_BOSSDAMAGEBONUS_DESC", "Deal an additional <style=cIsDamage>" + AprDamage.Value * 100f + "%</style> damage <style=cStack>(+" + AprDamage.Value * 100f + "% per stack)</style> to bosses.");
            if (BackupMagCDR.Value != 0f)
            {
                LanguageAPI.Add("ITEM_SECONDARYSKILLMAGAZINE_PICKUP", "Add an extra charge of your Secondary skill and reduce its Cooldown.");
                LanguageAPI.Add("ITEM_SECONDARYSKILLMAGAZINE_DESC", "Add <style=cIsUtility>+1</style> <style=cStack>(+1 per stack)</style> charge of your <style=cIsUtility>Secondary skill</style>. Reduce your Secondary skill Cooldown by <style=cIsUtility>" + BackupMagCDR.Value * 100 + "%</style> <style=cStack>(+" + BackupMagCDR.Value * 100 + "% per stack)</style>.");
            }
            bool useFlatHealthSteak = BisonSteakHealth.Value != 0f;
            bool useLevelHealthSteak = BisonSteakLevelHealth.Value;
            bool useBothHealthSteak = useFlatHealthSteak && useLevelHealthSteak;
            if (useFlatHealthSteak || useLevelHealthSteak)
            {
                LanguageAPI.Add("ITEM_FLATHEALTH_PICKUP",
                    $"Gain" +
                    (useLevelHealthSteak ? $" 30% base health" : "") +
                    (useBothHealthSteak ? $" +" : "") +
                    (useFlatHealthSteak ? $" {BisonSteakHealth.Value} max health" : "") +
                    $".");
                LanguageAPI.Add("ITEM_FLATHEALTH_DESC",
                    $"Increases <style=cIsHealing>base health</style> by" +
                    (useLevelHealthSteak ? $" <style=cIsHealing>30%</style> <style=cStack>(+30% per stack)</style>" : "") +
                    (useBothHealthSteak ? $" +" : "") +
                    (useFlatHealthSteak ? $" <style=cIsHealing>{BisonSteakHealth.Value}</style> <style=cStack>(+{BisonSteakHealth.Value} per stack)</style>" : "") +
                    $".");
            }
            if (RunaldBaseDamage.Value != 0f)
            {
                LanguageAPI.Add("ITEM_ICERING_DESC", "Hits that deal <style=cIsDamage>more than 400% damage</style> also blasts enemies with a <style=cIsDamage>runic ice blast</style>, <style=cIsUtility>slowing</style> them by <style=cIsUtility>80%</style> for <style=cIsUtility>3s</style> <style=cStack>(+3s per stack)</style> and <style=cIsDamage>" + RunaldTotalDamage.Value * 100f + "%</style> <style=cStack>(+" + RunaldTotalDamage.Value * 100f + "% per stack)</style> TOTAL damage and <style=cIsDamage>" + RunaldBaseDamage.Value * 100f + "%</style> <style=cStack>(+" + RunaldBaseDamage.Value * 100f + "% per stack)</style> base damage. Recharges every <style=cIsUtility>10</style> seconds.");
            }
            else
            {
                LanguageAPI.Add("ITEM_ICERING_DESC", "Hits that deal <style=cIsDamage>more than 400% damage</style> also blasts enemies with a <style=cIsDamage>runic ice blast</style>, <style=cIsUtility>slowing</style> them by <style=cIsUtility>80%</style> for <style=cIsUtility>3s</style> <style=cStack>(+3s per stack)</style> and <style=cIsDamage>" + RunaldTotalDamage.Value * 100f + "%</style> <style=cStack>(+" + RunaldTotalDamage.Value * 100f + "% per stack)</style> TOTAL damage. Recharges every <style=cIsUtility>10</style> seconds.");
            }
            if (KjaroBaseDamage.Value != 0f)
            {
                LanguageAPI.Add("ITEM_FIRERING_DESC", "Hits that deal <style=cIsDamage>more than 400% damage</style> also blasts enemies with a <style=cIsDamage>runic flame tornado</style>, dealing <style=cIsDamage>" + KjaroTotalDamage.Value * 100f + "%</style> <style=cStack>(+" + KjaroTotalDamage.Value * 100f + "% per stack)</style> TOTAL damage over time and <style=cIsDamage>" + KjaroTotalDamage.Value * 100f + "%</style> <style=cStack>(+" + KjaroTotalDamage.Value * 100f + "% per stack)</style> base damage. Recharges every <style=cIsUtility>10</style> seconds.");
            }
            else
            {
                LanguageAPI.Add("ITEM_FIRERING_DESC", "Hits that deal <style=cIsDamage>more than 400% damage</style> also blasts enemies with a <style=cIsDamage>runic flame tornado</style>, dealing <style=cIsDamage>" + KjaroTotalDamage.Value * 100f +"%</style> <style=cStack>(+" + KjaroTotalDamage.Value * 100f + "% per stack)</style> TOTAL damage over time. Recharges every <style=cIsUtility>10</style> seconds.");
            }
            bool useBaseHealthInfusion = InfusionBaseHealth.Value != 0f;
            bool usePercentHealthInfusion = InfusionPercentHealth.Value != 0f;
            bool useBothHealthInfusion = useBaseHealthInfusion && usePercentHealthInfusion;
            bool useScalingInfusion = InfusionScaling.Value == true;
            if (useBaseHealthInfusion || usePercentHealthInfusion)
            {
                LanguageAPI.Add("ITEM_INFUSION_PICKUP",
                    $"Gain" +
                    (usePercentHealthInfusion ? $" {InfusionPercentHealth.Value * 100f}%" : "") +
                    (useBothHealthInfusion ? $" +" : "") +
                    (useBaseHealthInfusion ? $" {InfusionBaseHealth.Value}" : "") +
                    $" max health. Killing an enemy permanently increases your maximum health, up to 100" +
                    (useScalingInfusion ? $", scales with level" : "") +
                    $".");
                LanguageAPI.Add("ITEM_INFUSION_DESC",
                    $"Increases <style=cIsHealing>maximum health</style> by" +
                    (usePercentHealthInfusion ? $" <style=cIsHealing>{InfusionPercentHealth.Value * 100f}%</style> <style=cStack>(+{InfusionPercentHealth.Value * 100f}% per stack)</style>" : "") +
                    (useBothHealthInfusion ? $" +" : "") +
                    (useBaseHealthInfusion ? $" <style=cIsHealing>{InfusionBaseHealth.Value}</style> <style=cStack>(+{InfusionBaseHealth.Value} per stack)</style>" : "") +
                    $". Killing an enemy increases your <style=cIsHealing>health permanently</style>" +
                    $" by <style=cIsHealing>1</style> <style=cStack>(+1 per stack)</style>," +
                    $" up to a <style=cIsHealing>maximum</style> of" +
                    $" <style=cIsHealing>100 <style=cStack>(+100 per stack)</style> health</style>." +
                    (useScalingInfusion ? $" Scales with level." : ""));
            }
            var bru = FireworksCount.Value + FireworksCountStack.Value;
            LanguageAPI.Add("ITEM_FIREWORK_DESC", "Activating an interactable <style=cIsDamage>launches " + bru + " <style=cStack>(+" + FireworksCountStack.Value + " per stack)</style> fireworks</style> that deal <style=cIsDamage>" + d(FireworksDamage.Value) + "%</style> base damage.");
            LanguageAPI.Add("ITEM_LUNARPRIMARYREPLACEMENT_DESC", "<style=cIsUtility>Replace your Primary Skill</style> with <style=cIsUtility>Hungering Gaze</style>. \n\nFire a flurry of <style=cIsUtility>tracking shards</style> that detonate after a delay, dealing <style=cIsDamage>120%</style> base damage. Hold up to " + VisionsCharges.Value + " charges <style=cStack>(+12 per stack)</style> that reload after " + VisionsCooldown.Value + " seconds <style=cStack>(+2 per stack)</style>.");
        }
    }
}
