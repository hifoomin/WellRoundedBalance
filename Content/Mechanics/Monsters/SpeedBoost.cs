namespace WellRoundedBalance.Mechanics.Monsters
{
    internal class SpeedBoost : MechanicBase<SpeedBoost>
    {
        public override string Name => ":: Mechanics ::::::::: Monster Movement Speed Buff";

        [ConfigField("Flat Movement Speed Gain", "", 1f)]
        public static float flatMovementSpeedGain;

        [ConfigField("Percent Movement Speed Gain", "Decimal.", 0.1f)]
        public static float percentMovementSpeedGain;

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
            if (sender.teamComponent.teamIndex != TeamIndex.Player)
            {
                if (Main.IsInfernoDef())
                {
                    // pass
                }
                else
                {
                    args.baseMoveSpeedAdd += flatMovementSpeedGain;
                    args.moveSpeedMultAdd += percentMovementSpeedGain;
                }
            }
        }
    }
}