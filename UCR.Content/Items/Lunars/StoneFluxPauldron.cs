using MonoMod.Cil;
using RoR2;

namespace UltimateCustomRun.Items.Lunars
{
    public class StoneFluxPauldron : ItemBase
    {
        public static float HpIncrease;
        public static float SpeedDecrease;

        public override string Name => ":: Items ::::: Lunars :: Stone Flux Pauldron";
        public override string InternalPickupToken => "halfSpeedDoubleHealth";
        public override bool NewPickup => true;
        public override string PickupText => "Increase your health by " + d(HpIncrease) + "... <color=#FF7F7F>BUT reduce your speed by " + Util.ConvertAmplificationPercentageIntoReductionPercentage(SpeedDecrease * 100f) + "%.</color>";
        public override string DescText => "Increase <style=cIsHealing>max health</style> by <style=cIsHealing>" + d(HpIncrease) + " <style=cStack>(+" + d(HpIncrease) + " per stack)</style></style>. Reduce <style=cIsUtility>movement speed</style> by <style=cIsUtility>" + Util.ConvertAmplificationPercentageIntoReductionPercentage(SpeedDecrease * 100f) + "%</style> <style=cStack>(+" + (Util.ConvertAmplificationPercentageIntoReductionPercentage(SpeedDecrease * 500f) - (Util.ConvertAmplificationPercentageIntoReductionPercentage(SpeedDecrease * 100f))) + "% per stack)</style>.";

        public override void Init()
        {
            HpIncrease = ConfigOption(1f, "Max HP Increase", "Decimal. Per Stack. Vanilla is 1");
            SpeedDecrease = ConfigOption(1f, "Move Speed Decrease", "Decimal. Per Stack. Vanilla is 1");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.RecalculateStats += ChangeHealth;
            IL.RoR2.CharacterBody.RecalculateStats += ChangeSpeed;
        }

        private void ChangeSpeed(ILContext il)
        {
            ILCursor c = new(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdloc(76),
                x => x.MatchLdloc(44),
                x => x.MatchConvR4(),
                x => x.MatchLdcR4(1f)
            );
            c.Index += 3;
            c.Next.Operand = SpeedDecrease;
        }

        private void ChangeHealth(ILContext il)
        {
            ILCursor c = new(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdloc(63),
                x => x.MatchLdloc(44),
                x => x.MatchConvR4(),
                x => x.MatchLdcR4(1f)
            );
            c.Index += 3;
            c.Next.Operand = HpIncrease;
        }
    }
}