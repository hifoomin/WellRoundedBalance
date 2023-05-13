using WellRoundedBalance.Gamemodes.Eclipse;

namespace WellRoundedBalance.Mechanics.Bosses
{
    internal class Enrage : MechanicBase<Enrage>
    {
        public override string Name => ":: Mechanics ::::: Boss Enrage";

        [ConfigField("Maximum Movement Speed Gain", "Decimal.", 0.3f)]
        public static float maximumMovementSpeedGain;

        [ConfigField("Maximum Attack Speed Gain", "Decimal.", 0.25f)]
        public static float maximumAttackSpeedGain;

        [ConfigField("Maximum Cooldown Reduction Gain", "Decimal.", 0.25f)]
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
                if (Run.instance.selectedDifficulty >= DifficultyIndex.Eclipse6 && Eclipse6.instance.isEnabled)
                {
                    if (sender.bodyIndex == BodyCatalog.FindBodyIndex("BrotherBody(Clone)") || sender.bodyIndex == BodyCatalog.FindBodyIndex("BrotherHurtBody(Clone)"))
                    {
                        // Main.WRBLogger.LogError("Affecting Mithrix");
                        args.moveSpeedMultAdd += increase * (maximumMovementSpeedGain * 1.25f);
                        args.attackSpeedMultAdd += increase * (maximumAttackSpeedGain * 1.25f);
                        args.cooldownMultAdd -= increase * (maximumCooldownReductionGain * 1.25f);
                    }
                    else
                    {
                        args.moveSpeedMultAdd += increase * (maximumMovementSpeedGain * 1.33f);
                        args.attackSpeedMultAdd += increase * (maximumAttackSpeedGain * 1.33f);
                        args.cooldownMultAdd -= increase * (maximumCooldownReductionGain * 1.33f);
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