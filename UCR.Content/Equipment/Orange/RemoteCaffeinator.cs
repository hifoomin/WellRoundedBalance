using MonoMod.Cil;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace UltimateCustomRun.Equipment
{
    public class RemoteCaffeinator : EquipmentBase
    {
        public override string Name => "::: Equipment :: Remote Caffeinator";
        public override string InternalPickupToken => "vendingMachine";

        public override bool NewPickup => false;

        public override bool NewDesc => true;

        public override string PickupText => "";

        public override string DescText => "Request an <style=cIsDamage>Eclipse Zero Vending Machine</style> from the <style=cIsDamage>UES Safe Travels</style>. Delivery guaranteed in <style=cIsUtility>5 seconds</style>, dealing <style=cIsDamage>2000% damage</style>. <style=cIsHealing>Heal</style> up to " + MaxTargets + " targets for <style=cIsHealing>" + d(Healing) + " of their maximum health</style>.";

        public static int MaxUses;
        public static float Healing;
        public static int MaxTargets;
        public static int GoldCost;
        public static float OrbSpeed;
        public static float OrbRange;
        public static int MaxCaffeinators;

        public override void Init()
        {
            MaxUses = ConfigOption(12, "Max Interactions", "Vanilla is 12");
            GoldCost = ConfigOption(5, "Base Gold Cost", "Vanilla is 5");
            Healing = ConfigOption(0.25f, "Percent Healing", "Decimal. Vanilla is 0.25");
            MaxTargets = ConfigOption(3, "Max Targets", "Vanilla is 3");
            OrbSpeed = ConfigOption(10f, "Healing Orb Speed", "Vanilla is 10");
            OrbRange = ConfigOption(10f, "Healing Orb Range", "Vanilla is 25");
            MaxCaffeinators = ConfigOption(1, "Max Remote Caffeinators", "Vanilla is 1");
            base.Init();
        }

        public override void Hooks()
        {
            On.RoR2.Orbs.VendingMachineOrb.Begin += ChangeHealing;
            On.RoR2.VendingMachineBehavior.Awake += Changes;
            On.RoR2.CharacterMaster.GetDeployableSameSlotLimit += ChangeLimit;
            Changess();
        }

        private int ChangeLimit(On.RoR2.CharacterMaster.orig_GetDeployableSameSlotLimit orig, CharacterMaster self, DeployableSlot slot)
        {
            if (slot is DeployableSlot.VendingMachine)
            {
                return MaxCaffeinators;
            }
            else
            {
                return orig(self, slot);
            }
        }

        private void Changes(On.RoR2.VendingMachineBehavior.orig_Awake orig, VendingMachineBehavior self)
        {
            self.maxPurchases = MaxUses;
            self.vendingRadius = OrbRange;
            self.numBonusOrbs = MaxTargets - 1;
            orig(self);
        }

        private void ChangeHealing(On.RoR2.Orbs.VendingMachineOrb.orig_Begin orig, RoR2.Orbs.VendingMachineOrb self)
        {
            self.healFraction = Healing;
            self.speed = OrbSpeed;
            orig(self);
        }

        private void Changess()
        {
            var vM = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VendingMachine/VendingMachine.prefab").WaitForCompletion();
            vM.GetComponent<PurchaseInteraction>().cost = GoldCost;
        }
    }
}