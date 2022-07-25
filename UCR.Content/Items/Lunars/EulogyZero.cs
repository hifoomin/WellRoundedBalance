using MonoMod.Cil;

namespace UltimateCustomRun.Items.Lunars
{
    public class EulogyZero : ItemBase
    {
        public static float Chance;

        public override string Name => ":: Items ::::: Lunars :: Eulogy Zero";
        public override string InternalPickupToken => "randomlyLunar";
        public override bool NewPickup => false;
        public override string PickupText => "";
        public override string DescText => "Items have a <style=cIsUtility>" + d(Chance) + "% <style=cStack>(+" + d(Chance) + "% per stack)</style></style> chance to become a <style=cIsLunar>Lunar</style> item instead.";

        public override void Init()
        {
            Chance = ConfigOption(0.05f, "Lunar Chance", "Per Stack. Vanilla is 0.05");
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

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(0.05f)
            );
            c.Next.Operand = Chance;
        }

        private void ChangeChance1(ILContext il)
        {
            ILCursor c = new(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(0.05f)
            );
            c.Next.Operand = Chance;
        }
    }
}