using BepInEx.Configuration;
using UnityEngine;

namespace WellRoundedBalance.Mechanic.Bosses
{
    internal class BetterScaling
    {
        public static ConfigEntry<bool> enable { get; set; }

        private static void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.isChampion)
            {
                args.armorAdd += Mathf.Pow(100f - 100f / (1 + 0.07f), Run.instance.stageClearCount);
            }
        }

        [SystemInitializer(typeof(BodyCatalog))]
        public static void NerfHealthScaling()
        {
            enable = Main.WRBGlobalConfig.Bind(":: Mechanic ::: Bosses :: Better Scaling", "Enable?", true, "Vanilla is false");
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
            foreach (CharacterBody body in BodyCatalog.allBodyPrefabBodyBodyComponents)
            {
                if (body.isChampion)
                {
                    body.baseMaxHealth *= 0.75f;
                    body.levelMaxHealth *= 0.75f;
                }
            }
        }
    }
}