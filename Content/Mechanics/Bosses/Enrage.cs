using WellRoundedBalance.Gamemodes.Eclipse;

namespace WellRoundedBalance.Mechanics.Bosses
{
    internal class Enrage : MechanicBase<Enrage>
    {
        public override string Name => ":: Mechanics ::::: Boss Enrage";

        [ConfigField("Maximum Movement Speed Gain", "Decimal. Only applies below Eclipse 6.", 0.3f)]
        public static float maximumMovementSpeedGain;

        [ConfigField("Maximum Attack Speed Gain", "Decimal. Only applies below Eclipse 6.", 0.25f)]
        public static float maximumAttackSpeedGain;

        [ConfigField("Maximum Cooldown Reduction Gain", "Decimal. Only applies below Eclipse 6.", 0.25f)]
        public static float maximumCooldownReductionGain;

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
                if (Eclipse6.instance.isEnabled)
                {
                    if (Run.instance.selectedDifficulty < DifficultyIndex.Eclipse6)
                    {
                        args.moveSpeedMultAdd += increase * maximumMovementSpeedGain;
                        args.attackSpeedMultAdd += increase * maximumAttackSpeedGain;
                        args.cooldownMultAdd -= increase * maximumCooldownReductionGain;
                    }
                }
                else
                {
                    args.moveSpeedMultAdd += increase * maximumMovementSpeedGain;
                    args.attackSpeedMultAdd += increase * maximumAttackSpeedGain;
                    args.cooldownMultAdd -= increase * maximumCooldownReductionGain;
                }
            }
        }
    }
}