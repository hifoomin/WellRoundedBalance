using UnityEngine;

namespace WellRoundedBalance.Mechanic.Bosses
{
    internal class Enrage : MechanicBase
    {
        public override string Name => ":: Mechanics ::: Bosses : Enrage";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.isChampion && sender.healthComponent)
            {
                var increase = Mathf.Clamp01(1f - sender.healthComponent.combinedHealthFraction);
                if (Run.instance.selectedDifficulty >= DifficultyIndex.Eclipse6)
                {
                    args.moveSpeedMultAdd += increase * 0.6f;
                    args.attackSpeedMultAdd += increase * 0.5f;
                    args.cooldownMultAdd -= increase * 0.5f;
                }
                else
                {
                    args.moveSpeedMultAdd += increase * 0.3f;
                    args.attackSpeedMultAdd += increase * 0.25f;
                    args.cooldownMultAdd -= increase * 0.25f;
                }
            }
        }
    }
}