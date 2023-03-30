namespace WellRoundedBalance.Items.Reds
{
    public class _57LeafClover : ItemBase
    {
        public override string Name => ":: Items ::: Reds :: 57 Leaf Clover";
        public override ItemDef InternalPickup => RoR2Content.Items.Clover;

        public override string PickupText => "Luck is on your side.";

        public override string DescText => "All random effects are rolled <style=cIsUtility>+1</style> <style=cStack>(+1 per stack)</style> times for a <style=cIsUtility>favorable outcome</style>.";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
        }
    }
}