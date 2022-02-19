using MonoMod.Cil;

namespace UltimateCustomRun
{
    public class PersonalShieldGenerator : ItemBase
    {
        public static float percenthp;

        public override string Name => ":: Items : Whites :: Personal Shield Generator";
        public override string InternalPickupToken => "personalShield";
        public override bool NewPickup => false;

        public override string PickupText => "";

        public override string DescText => "Gain a <style=cIsHealing>shield</style> equal to <style=cIsHealing>" + d(percenthp) + "</style> <style=cStack>(+" + d(percenthp) + " per stack)</style> of your maximum health. Recharges outside of danger.";
        public override void Init()
        {
            percenthp = ConfigOption(0.08f, "Percent Shield", "Decimal. Per Stack. Vanilla is 0.08");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.RecalculateStats += ChangeShieldPercent;
        }
        public static void ChangeShieldPercent(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchConvR4(),
                x => x.MatchLdcR4(0.08f)
            );
            c.Index += 1;
            c.Next.Operand = percenthp;
        }
    }
}
