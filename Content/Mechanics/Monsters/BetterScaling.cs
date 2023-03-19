using BepInEx.Configuration;

namespace WellRoundedBalance.Mechanics.Monsters
{
    internal class BetterScaling
    {
        public static ConfigEntry<bool> enable { get; set; }

        [ConfigField("Health Multiplier", "", 0.7f)]
        public static float healthMultiplier;

        [ConfigField("Armor Cap", "Formula for current armor gain: Armor Cap - Armor Cap / Base Value ^ Stages Cleared", 200f)]
        public static float armorCap;

        [ConfigField("Base Value", "Formula for current armor gain: Armor Cap - Armor Cap / Base Value ^ Stages Cleared", 1.055f)]
        public static float baseValue;

        public static void Init()
        {
            enable = Main.WRBMechanicConfig.Bind(":: Mechanics ::::::::: Monster Health and Armor Adjustment", "Enable?", true, "Vanilla is false");
            if (enable.Value)
            {
                RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
            }
        }

        private static void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.teamComponent.teamIndex != TeamIndex.Player)
            {
                args.armorAdd += armorCap - armorCap / Mathf.Pow(baseValue, Run.instance.stageClearCount);
                args.healthMultAdd *= healthMultiplier; // no inferno check intentionally
            }
        }
    }
}