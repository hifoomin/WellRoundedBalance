namespace WellRoundedBalance.Items.Greens
{
    public class WaxQuail : ItemBase
    {
        public override string Name => ":: Items :: Greens :: Wax Quail";
        public override ItemDef InternalPickup => RoR2Content.Items.JumpBoost;

        public override string PickupText => "Jumping while sprinting boosts you forward.";
        public override string DescText => "<style=cIsUtility>Jumping</style> while <style=cIsUtility>sprinting</style> boosts you forward by <style=cIsUtility>10m</style> <style=cStack>(+10m per stack)</style>.";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
        }
    }
}