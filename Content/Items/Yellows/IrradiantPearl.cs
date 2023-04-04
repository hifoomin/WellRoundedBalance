namespace WellRoundedBalance.Items.Yellows
{
    public class IrradiantPearl : ItemBase<IrradiantPearl>
    {
        public override string Name => ":: Items :::: Yellows :: Irradiant Pearl";
        public override ItemDef InternalPickup => RoR2Content.Items.ShinyPearl;

        public override string PickupText => "Increase ALL of your stats.";

        public override string DescText => "Increases <style=cIsUtility>ALL stats</style> by <style=cIsUtility>10%</style> <style=cStack>(+10% per stack)</style>.";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += AddBehavior;
        }

        private void AddBehavior(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.inventory)
            {
                var stack = sender.inventory.GetItemCount(RoR2Content.Items.ShinyPearl);
                if (stack > 0)
                {
                    args.regenMultAdd += 0.1f * stack;
                    args.armorAdd += sender.armor * 0.1f * stack;
                }
            }
        }
    }
}