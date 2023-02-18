using BepInEx.Configuration;

namespace WellRoundedBalance.Mechanics.Monsters
{
    internal class SpeedBoost
    {
        public static ConfigEntry<bool> enable { get; set; }

        public static void Init()
        {
            enable = Main.WRBMechanicConfig.Bind(":: Mechanics :::::::: Monster Movement Speed Buff", "Enable?", true, "Vanilla is false");
            if (enable.Value)
            {
                RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
            }
        }

        private static void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.teamComponent.teamIndex != TeamIndex.Player)
            {
                args.armorAdd += 150 - 150 / Mathf.Pow(1 + 0.055f, Run.instance.stageClearCount);
                if (Main.IsInfernoDef())
                {
                    // pass
                }
                else
                {
                    args.baseMoveSpeedAdd += 1f;
                    args.moveSpeedMultAdd += 0.1f;
                }
            }
        }
    }
}