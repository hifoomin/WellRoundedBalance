using BepInEx.Configuration;

namespace WellRoundedBalance.Mechanic.Monsters
{
    internal class SpeedBoost
    {
        public static ConfigEntry<bool> enable { get; set; }

        [SystemInitializer(typeof(BodyCatalog))]
        public static void AddSpeedBoost()
        {
            enable = Main.WRBMechanicConfig.Bind(":: Mechanics ::::::: Monster Movement Speed Buff", "Enable?", true, "Vanilla is false");
            foreach (CharacterBody body in BodyCatalog.bodyPrefabBodyComponents)
            {
                if (SurvivorCatalog.FindSurvivorDefFromBody(body.gameObject) == null)
                {
                    body.baseMoveSpeed += 1f;
                    body.baseMoveSpeed *= 1.1f;
                }
            }
        }
    }
}