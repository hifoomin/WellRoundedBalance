using MonoMod.Cil;

namespace UltimateCustomRun
{
    public class SoldiersSyringe : ItemBase
    {
        public static float aspd;

        public override string Name => ":: Items : Whites :: Soldiers Syringe";
        public override string InternalPickupToken => "syringe";
        public override bool NewPickup => false;

        public override string PickupText => "";

        public override string DescText => "Increases <style=cIsDamage>attack speed</style> by <style=cIsDamage>" + d(aspd) + " <style=cStack>(+" + d(aspd) + " per stack)</style></style>.";
        public override void Init()
        {
            aspd = ConfigOption(0.15f, "Attack Speed", "Decimal. Per Stack. Vanilla is 0.15");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.RecalculateStats += SoldiersSyringe.ChangeAS;
        }
        public static void ChangeAS(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchConvR4(),
                x => x.MatchLdcR4(0.15f)
            );
            c.Index += 1;
            c.Next.Operand = aspd;
        }
    }
}
