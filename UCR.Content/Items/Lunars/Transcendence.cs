using MonoMod.Cil;

namespace UltimateCustomRun.Items.Lunars
{
    public class Transcendence : ItemBase
    {
        public static float HpIncrease;
        public static float StackHpIncrease;
        public static float ShieldTimerIncrease;

        public override string Name => ":: Items ::::: Lunars :: Transcendence";
        public override string InternalPickupToken => "shieldOnly";
        public override bool NewPickup => false;
        public override string PickupText => "";
        public override string DescText => "<style=cIsHealing>Convert</style> all but <style=cIsHealing>1 health</style> into <style=cIsHealing>regenerating shields</style>. <style=cIsHealing>Gain 50% <style=cStack>(+25% per stack)</style> maximum health</style>.";

        public override void Init()
        {
            HpIncrease = ConfigOption(0.5f, "Base Max HP Increase", "Decimal. Vanilla is 0.5");
            StackHpIncrease = ConfigOption(0.25f, "Stack Max HP Increase", "Decimal. Per Stack. Vanilla is 0.25");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.RecalculateStats += ChangeHealth;
        }

        private void ChangeHealth(ILContext il)
        {
            ILCursor c = new(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(1.5f),
                x => x.MatchLdloc(out _),
                x => x.MatchLdcI4(1),
                x => x.MatchSub(),
                x => x.MatchConvR4(),
                x => x.MatchLdcR4(0.25f)
            );
            c.Next.Operand = 1f + HpIncrease;
            c.Index += 5;
            c.Next.Operand = StackHpIncrease;
        }
    }
}