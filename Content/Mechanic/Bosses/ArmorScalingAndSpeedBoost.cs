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
            CharacterBody.onBodyStartGlobal += CharacterBody_onBodyStartGlobal;
        }

        private void CharacterBody_onBodyStartGlobal(CharacterBody body)
        {
            var teamComponent = body.teamComponent;
            if (teamComponent && teamComponent.teamIndex != TeamIndex.Player)
            {
                if (body.GetComponent<WRBStatComponent>() == null)
                {
                    body.gameObject.AddComponent<WRBStatComponent>();
                }
            }
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.isChampion)
            {
                args.armorAdd += Mathf.Pow(100f - 100f / (1 + 0.07f), Run.instance.stageClearCount);
            }
        }

        public class WRBStatComponent : MonoBehaviour
        {
            public CharacterBody body;

            private void Start()
            {
                body = GetComponent<CharacterBody>();
                if (body)
                {
                    if (body.isChampion)
                    {
                        body.baseMaxHealth *= 0.75f;
                        body.levelMaxHealth *= 0.75f;
                    }
                    body.baseMoveSpeed += 1f;
                    body.baseMoveSpeed *= 1.1f;
                }
            }
        }
    }
}