using UnityEngine;

namespace WellRoundedBalance.Mechanic.Bosses
{
    internal class Enrage : GlobalBase
    {
        public override string Name => ":: Mechanic ::: Bosses : Enrage";

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
                args.moveSpeedMultAdd += increase * 0.3f;
                args.attackSpeedMultAdd += increase * 0.25f;
                args.cooldownMultAdd -= increase * 0.25f;
                args.armorAdd += 2f * Run.instance.stageClearCount;
            }
        }
    }
}