using MonoMod.Cil;

namespace WellRoundedBalance.Items.Lunars
{
    public class EulogyZero : ItemBase
    {
        public override string Name => ":: Items ::::: Lunars :: Eulogy Zero";
        public override string InternalPickupToken => "randomlyLunar";

        public override string PickupText => "Items and equipment have a small chance to transform into a Lunar item instead.";
        public override string DescText => "Items have a <style=cIsUtility>10% <style=cStack>(+10% per stack)</style></style> chance to become a <style=cIsLunar>Lunar</style> item instead.";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.Items.RandomlyLunarUtils.CheckForLunarReplacement += ChangeChance1;
            IL.RoR2.Items.RandomlyLunarUtils.CheckForLunarReplacementUniqueArray += ChangeChance2;
        }

        private void ChangeChance2(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(0.05f)))
            {
                c.Next.Operand = 0.1f;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Eulogy Zero Chance2 hook");
            }
        }

        private void ChangeChance1(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(0.05f)))
            {
                c.Next.Operand = 0.1f;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Eulogy Zero Chance1 hook");
            }
        }
    }
}