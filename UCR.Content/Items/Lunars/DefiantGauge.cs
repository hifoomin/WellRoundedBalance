using MonoMod.Cil;

namespace UltimateCustomRun.Items.Lunars
{
    public class DefiantGauge : ItemBase
    {
        public static float Credits;

        public override string Name => ":: Items ::::: Lunars :: Defiant Gauge";
        public override string InternalPickupToken => "monstersOnShrineUse";
        public override bool NewPickup => false;
        public override string PickupText => "";
        public override string DescText => "Using a Shrine summons <style=cIsHealth>enemies</style> nearby. <style=cIsUtility>Scales over time.</style>";

        public override void Init()
        {
            Credits = ConfigOption(40f, "Monster Credits", "Per Stack. Vanilla is 40");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnInteractionBegin += ChangeCredits;
        }

        private void ChangeCredits(ILContext il)
        {
            ILCursor c = new(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(40f)
            );
            c.Next.Operand = Credits;
        }
    }
}