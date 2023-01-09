using UnityEngine;

namespace WellRoundedBalance.Mechanic.Bosses
{
    internal class ArmorScalingAndSpeedBoost : GlobalBase
    {
        public override string Name => ":: Mechanic ::: Bosses :: Armor Scaling and Health Nerf : All Monsters : Movement Speed Buff";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
            NerfHealthScaling();
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.isChampion)
            {
                args.armorAdd += Mathf.Pow(100f - 100f / (1 + 0.07f), Run.instance.stageClearCount);
            }
        }

        [SystemInitializer(typeof(BodyCatalog))]
        private void NerfHealthScaling()
        {
            if (BodyCatalog.bodyPrefabBodyComponents != null)
            {
                for (int i = 0; i < BodyCatalog.bodyPrefabBodyComponents.Length; i++)
                {
                    var body = BodyCatalog.bodyPrefabBodyComponents[i];
                    if (body != null && i < BodyCatalog.bodyPrefabBodyComponents.Length)
                    {
                        body.baseMoveSpeed += 1f;
                        body.baseMoveSpeed *= 1.1f;
                        if (body.isChampion)
                        {
                            body.baseMaxHealth *= 0.75f;
                            body.levelMaxHealth *= 0.75f;
                        }
                    }
                    else
                    {
                        Main.WRBLogger.LogError("body in bodyPrefabBodyComponents is null somewhere in the middle");
                    }
                }
            }
            else
            {
                Main.WRBLogger.LogError("bodyPrefabBodyComponents is null at the beginning");
            }
        }
    }
}