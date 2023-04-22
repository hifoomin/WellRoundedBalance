using RoR2.Orbs;

namespace WellRoundedBalance.Equipment.Orange
{
    public class ExecutiveCard : EquipmentBase<ExecutiveCard>
    {
        public override string Name => ":: Equipment :: Executive Card";

        public override EquipmentDef InternalPickup => DLC1Content.Equipment.MultiShopCard;

        public override string PickupText => "Gain " + d(cashBackPercent) + " cash back on all purchases. Multishops remain open.";

        public override string DescText => "Whenever you make a gold purchase, get <style=cIsUtility>" + d(cashBackPercent) + "</style> of the spent gold back. If the purchase is a <style=cIsUtility>multishop</style> terminal, the other terminals will <style=cIsUtility>remain open</style>.";

        [ConfigField("Cooldown", "", 25f)]
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

            On.RoR2.Items.MultiShopCardUtils.OnPurchase += MultiShopCardUtils_OnPurchase1;
        }

        private void MultiShopCardUtils_OnPurchase1(On.RoR2.Items.MultiShopCardUtils.orig_OnPurchase orig, CostTypeDef.PayCostContext context, int moneyCost)
        {
            var activatorMaster = context.activatorMaster;
            if (activatorMaster && activatorMaster.hasBody && activatorMaster.inventory && activatorMaster.inventory.currentEquipmentIndex == DLC1Content.Equipment.MultiShopCard.equipmentIndex)
            {
                var body = activatorMaster.GetBody();
                var shouldActivate = false;
                var purchasedObject = context.purchasedObject;
                if (moneyCost > 0)
                {
                    var goldOrb = new GoldOrb();
                    Orb orb = goldOrb;

                    Vector3? vector;
                    if (purchasedObject == null)
                    {
                        vector = null;
                    }
                    else
                    {
                        Transform transform = purchasedObject.transform;
                        vector = ((transform != null) ? new Vector3?(transform.position) : null);
                    }
                    orb.origin = vector ?? body.corePosition;
                    goldOrb.target = body.mainHurtBox;
                    goldOrb.goldAmount = (uint)(cashBackPercent * moneyCost);
                    OrbManager.instance.AddOrb(goldOrb);
                }
                if (body.equipmentSlot.stock > 0)
                {
                    var shopTerminalBehavior = (purchasedObject != null) ? purchasedObject.GetComponent<ShopTerminalBehavior>() : null;
                    if (shopTerminalBehavior && shopTerminalBehavior.serverMultiShopController)
                    {
                        shouldActivate = true;
                        shopTerminalBehavior.serverMultiShopController.SetCloseOnTerminalPurchase(context.purchasedObject.GetComponent<PurchaseInteraction>(), false);
                    }
                    if (shouldActivate)
                    {
                        if (body.hasAuthority)
                        {
                            body.equipmentSlot.OnEquipmentExecuted();
                            return;
                        }
                        body.equipmentSlot.CallCmdOnEquipmentExecuted();
                    }
                }
            }
        }
    }
}