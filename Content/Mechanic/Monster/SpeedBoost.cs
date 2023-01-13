using BepInEx.Configuration;

namespace WellRoundedBalance.Mechanic.Monster
{
    internal class SpeedBoost
    {
        public static ConfigEntry<bool> enable { get; set; }

        [SystemInitializer(typeof(BodyCatalog))]
        public static void AddSpeedBoost()
        {
            enable = Main.WRBMechanicConfig.Bind(":: Mechanic ::::::: Monsters : Movement Speed Buff", "Enable?", true, "Vanilla is false");
            foreach (CharacterBody body in BodyCatalog.bodyPrefabBodyComponents)
            {
                body.baseMoveSpeed += 1f;
                body.baseMoveSpeed *= 1.1f;
            }
        }
    }
}