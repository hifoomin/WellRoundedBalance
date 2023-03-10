using BepInEx.Configuration;

namespace WellRoundedBalance.Mechanics.Monsters
{
    internal class SpeedBoost
    {
        public static ConfigEntry<bool> enable { get; set; }

        [ConfigField("Flat Movement Speed Gain", "", 1f)]
        public static float flatMovementSpeedGain;

        [ConfigField("Percent Movement Speed Gain", "Decimal.", 0.1f)]
        public static float percentMovementSpeedGain;

        public static void Init()
        {
            enable = Main.WRBMechanicConfig.Bind(":: Mechanics ::::::::: Monster Movement Speed Buff", "Enable?", true, "Vanilla is false");
            if (enable.Value)
            {
                RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
            }
        }

        private static void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.teamComponent.teamIndex != TeamIndex.Player)
            {
                if (Main.IsInfernoDef())
                {
                    // pass
                }
                else
                {
                    args.baseMoveSpeedAdd += flatMovementSpeedGain;
                    args.moveSpeedMultAdd += percentMovementSpeedGain;
                }
            }
        }
    }
}