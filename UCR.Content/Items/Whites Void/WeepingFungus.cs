using MonoMod.Cil;

namespace UltimateCustomRun.Items.VoidWhites
{
    public class WeepingFungus : ItemBase
    {
        public static float HealPercent;
        public static float Interval;

        public override string Name => ":: Items :::::: Void Whites :: Weeping Fungus";
        public override string InternalPickupToken => "mushroomVoid";
        public override bool NewPickup => false;
        public override string PickupText => "";
        public override string DescText => "<style=cIsHealing>Heals</style> for <style=cIsHealing>" + d(HealPercent) + "</style> <style=cStack>(+" + d(HealPercent) + " per stack)</style> of your <style=cIsHealing>health</style> every second <style=cIsUtility>while sprinting</style>. <style=cIsVoid>Corrupts all Bustling Fungi</style>.";

        public override void Init()
        {
            HealPercent = ConfigOption(0.02f, "Percent Healing", "Decimal. Per Stack. Vanilla is 0.02");
            Interval = ConfigOption(0.5f, "Healing Interval", "Vanilla is 0.5");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.MushroomVoidBehavior.FixedUpdate += ChangeHealing;
        }

        private void ChangeHealing(ILContext il)
        {
            ILCursor c = new(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(0.01f)
            );
            c.Next.Operand = HealPercent * Interval;

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(0.5f),
                x => x.MatchSub()
            );
            c.Next.Operand = Interval;

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(0.5f),
                x => x.MatchBgt(out _)
            );
            c.Next.Operand = Interval;
        }
    }
}