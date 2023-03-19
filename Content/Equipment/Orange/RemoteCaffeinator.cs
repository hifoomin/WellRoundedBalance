namespace WellRoundedBalance.Equipment.Orange
{
    public class RemoteCaffeinator : EquipmentBase
    {
        public override string Name => ":: Equipment :: Remote Caffeinator";
        public override EquipmentDef InternalPickup => DLC1Content.Equipment.VendingMachine;

        public override string PickupText => "Request a healing soda machine.";

        public override string DescText => "Request an <style=cIsDamage>Eclipse Zero Vending Machine</style> from the <style=cIsDamage>UES Safe Travels</style>. Delivery guaranteed in <style=cIsUtility>5 seconds</style>, dealing <style=cIsDamage>2000% damage</style>. <style=cIsHealing>Heal</style> up to " + maxTargets + " targets for <style=cIsHealing>" + d(percentHealing) + " of their maximum health</style>.";

        [ConfigField("Cooldown", "", 60f)]
        public static float cooldown;

        [ConfigField("Max Uses", "", 12)]
        public static int maxUses;

        [ConfigField("Gold Cost", "", 5)]
        public static int goldCost;

        [ConfigField("Percent Healing", "Decimal.", 0.25f)]
        public static float percentHealing;

        [ConfigField("Max Targets", "", 3)]
        public static int maxTargets;

        [ConfigField("Range", "", 10f)]
        public static float range;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.RoR2.Orbs.VendingMachineOrb.Begin += ChangeHealing;
            On.RoR2.VendingMachineBehavior.Awake += Changes;
            Changess();

            var Vent = Utils.Paths.EquipmentDef.VendingMachine.Load<EquipmentDef>();
            Vent.cooldown = cooldown;
        }

        private void Changes(On.RoR2.VendingMachineBehavior.orig_Awake orig, VendingMachineBehavior self)
        {
            self.maxPurchases = maxUses;
            self.vendingRadius = range;
            self.numBonusOrbs = maxTargets - 1;
            orig(self);
        }

        private void ChangeHealing(On.RoR2.Orbs.VendingMachineOrb.orig_Begin orig, RoR2.Orbs.VendingMachineOrb self)
        {
            self.healFraction = percentHealing;
            orig(self);
        }

        private void Changess()
        {
            var vM = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VendingMachine/VendingMachine.prefab").WaitForCompletion();
            vM.GetComponent<PurchaseInteraction>().cost = goldCost;
        }
    }
}