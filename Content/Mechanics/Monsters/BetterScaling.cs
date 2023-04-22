namespace WellRoundedBalance.Mechanics.Monsters
{
    internal class BetterScaling : MechanicBase<BetterScaling>
    {
        public override string Name => ":: Mechanics ::::::::: Monster Health and Armor Adjustment";

        [ConfigField("Health Multiplier", "", 0.7f)]
        public static float healthMultiplier;

        [ConfigField("Armor Cap", "Formula for current armor gain: Armor Cap - Armor Cap / Base Value ^ Stages Cleared", 200f)]
        public static float armorCap;

        [ConfigField("Base Value", "Formula for current armor gain: Armor Cap - Armor Cap / Base Value ^ Stages Cleared", 1.045f)]
        public static float baseValue;

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
                args.armorAdd += armorCap - armorCap / Mathf.Pow(baseValue, Run.instance.stageClearCount);
                args.healthMultAdd *= healthMultiplier; // no inferno check intentionally
            }
        }
    }
}