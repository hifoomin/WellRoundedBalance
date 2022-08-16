using MonoMod.Cil;

namespace UltimateCustomRun.Equipment
{
    public class ExecutiveCard : EquipmentBase
    {
        public override string Name => "::: Equipment :: Executive Card";
        public override string InternalPickupToken => "multishopCard";

        public override bool NewPickup => true;

        public override bool NewDesc => true;

        public override string PickupText => "Gain " + d(Cashback) + " cash back on all purchases. Multishops remain open.";

        public override string DescText => "Whenever you make a gold purchase, get <style=cIsUtility>" + d(Cashback) + "</style> of the spent gold back. If the purchase is a <style=cIsUtility>multishop</style> terminal, the other terminals will <style=cIsUtility>remain open</style>.";

        public static float Cashback;

        public override void Init()
        {
            Cashback = ConfigOption(0.1f, "Cashback Percent", "Decimal. Vanilla is 0.1");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.Items.MultiShopCardUtils.OnPurchase += ChangeCashback;
        }

        private void ChangeCashback(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(0.1f)))
            {
                c.Next.Operand = Cashback;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Executive Card Cashback hook");
            }
        }
    }
}