namespace WellRoundedBalance.Items.Whites
{
    public class CautiousSlug : ItemBase<CautiousSlug>
    {
        public override string Name => ":: Items : Whites :: Cautious Slug";
        public override ItemDef InternalPickup => RoR2Content.Items.HealWhileSafe;

        public override string PickupText => "Rapidly heal outside of danger.";

        public override string DescText => "Increases <style=cIsHealing>base health regeneration</style> by <style=cIsHealing>+3 hp/s</style> <style=cStack>(+3 hp/s per stack)</style> while outside of combat.";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
        }
    }
}