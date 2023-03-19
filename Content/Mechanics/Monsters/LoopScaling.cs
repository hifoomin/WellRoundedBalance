namespace WellRoundedBalance.Mechanics.Monsters
{
    internal class LoopScaling : MechanicBase
    {
        public override string Name => ":: Mechanics ::::::::: Monster Loop Armor";

        [ConfigField("Armor Per Loop", "", 7f)]
        public static float armorPerLoop;

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
                args.armorAdd += armorPerLoop * Run.instance.loopClearCount;
            }
        }
    }
}