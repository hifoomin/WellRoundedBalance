using MonoMod.Cil;

namespace WellRoundedBalance.Equipment
{
    public class ExecutiveCard : EquipmentBase
    {
        public override string Name => ":: Equipment :: Executive Card";

        public override EquipmentDef InternalPickup => DLC1Content.Equipment.MultiShopCard;

        public override string PickupText => "Gain " + d(cashBackPercent) + " cash back on all purchases. Multishops remain open.";

        public override string DescText => "Whenever you make a gold purchase, get <style=cIsUtility>" + d(cashBackPercent) + "</style> of the spent gold back. If the purchase is a <style=cIsUtility>multishop</style> terminal, the other terminals will <style=cIsUtility>remain open</style>.";

        [ConfigField("Cooldown", "", 20f)]
        public static float cooldown;

        [ConfigField("Cash Back Percent", "Decimal.", 0.1f)]
        public static float cashBackPercent;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            var Card = Utils.Paths.EquipmentDef.MultiShopCard.Load<EquipmentDef>();
            Card.cooldown = cooldown;

            IL.RoR2.Items.MultiShopCardUtils.OnPurchase += MultiShopCardUtils_OnPurchase;
        }

        private void MultiShopCardUtils_OnPurchase(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(0.1f)))
            {
                c.Next.Operand = cashBackPercent;
            }
            else
            {
                Logger.LogError("Failed to apply Disposable Missile Launcher Missile Count hook");
            }
        }
    }
}