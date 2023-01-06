using UnityEngine;

namespace WellRoundedBalance.Mechanic.Bosses
{
    internal class EnrageTwo : GlobalBase
    {
        private static BuffDef enraged;
        public override string Name => ":: Mechanic ::: Minibosses : Enrage";

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
            if (sender. && sender.healthComponent)
            {
                var increase = Mathf.Clamp01(1f - sender.healthComponent.combinedHealthFraction);
                args.moveSpeedMultAdd += increase * 0.3f;
                args.attackSpeedMultAdd += increase * 0.2f;
                args.cooldownMultAdd -= increase * 0.1f;
                args.armorAdd += 2f * Run.instance.stageClearCount;
            }
        }
    }
}