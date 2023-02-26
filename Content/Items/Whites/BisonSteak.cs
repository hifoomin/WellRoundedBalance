using MonoMod.Cil;

namespace WellRoundedBalance.Items.Whites
{
    public class BisonSteak : ItemBase
    {
        public override string Name => ":: Items : Whites :: Bison Steak";
        public override string InternalPickupToken => "flatHealth";

        public override string PickupText => "Gain 45 max health.";

        public override string DescText => StackDesc(maximumHealthGain, maximumHealthGainStack, 
            init => $"Increases <style=cIsHealing>maximum health</style> by <style=cIsHealing>{init}</style>{{Stack}}.", 
            stack => stack.ToString());

        [ConfigField("Maximum Health Gain", "", 45f)]
        public static float maximumHealthGain;

        [ConfigField("Maximum Health Gain per Stack", "", 45f)]
        public static float maximumHealthGainStack;

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
            if (sender.inventory)
            {
                args.baseHealthAdd += StackAmount(maximumHealthGain - 25, maximumHealthGainStack - 25,
                    sender.inventory.GetItemCount(RoR2Content.Items.FlatHealth));
            }
        }
    }
}