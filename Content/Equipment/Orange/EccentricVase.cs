using MonoMod.Cil;

namespace WellRoundedBalance.Equipment.Orange
{
    public class EccentricVase : EquipmentBase<EccentricVase>
    {
        public override string Name => ":: Equipment :: Eccentric Vase";
        public override EquipmentDef InternalPickup => RoR2Content.Equipment.Gateway;

        public override string PickupText => (chargeSpeedIncrease > 0 ? "Passively increase the speed of Teleporter Charging. " : "") + "Create a quantum tunnel between two locations.";

        public override string DescText => (chargeSpeedIncrease > 0 ? "Teleporters charge <style=cIsUtility>" + d(chargeSpeedIncrease) + " faster</style>. " : "") + "Create a <style=cIsUtility>quantum tunnel</style> of up to <style=cIsUtility>" + maxDistance + "m</style> in length. Lasts " + duration + " seconds.";

        [ConfigField("Cooldown", "", 20f)]
        public static float cooldown;

        [ConfigField("Max Distance", "", 1000f)]
        public static float maxDistance;

        [ConfigField("Duration", "", 15f)]
        public static float duration;

        // [ConfigField("Passive Holdout Zone Charge Speed Increase", "Decimal", 0.2f)]
        public static float chargeSpeedIncrease = 0;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.EquipmentSlot.FireGateway += Changes;

            var Vase = Utils.Paths.EquipmentDef.Gateway.Load<EquipmentDef>();
            Vase.cooldown = cooldown;
            // On.RoR2.HoldoutZoneController.Awake += HoldoutZoneController_Awake;
        }

        public static float increase = 0;

        private void HoldoutZoneController_Awake(On.RoR2.HoldoutZoneController.orig_Awake orig, HoldoutZoneController self)
        {
            orig(self);
            for (int i = 0; i < CharacterBody.readOnlyInstancesList.Count; i++)
            {
                var index = CharacterBody.readOnlyInstancesList[i];
                if (index.teamComponent.teamIndex == TeamIndex.Player)
                {
                    var inventory = index.inventory;
                    if (inventory && inventory.currentEquipmentIndex == RoR2Content.Equipment.Gateway.equipmentIndex)
                    {
                        increase += chargeSpeedIncrease;
                        Main.WRBLogger.LogError("vase increase is " + increase);
                    }
                }
            }
            self.calcChargeRate += Self_calcChargeRate;
        }

        private void Self_calcChargeRate(ref float rate)
        {
            rate *= 1f + increase;
        }

        private void Changes(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(1000f)))
            {
                c.Next.Operand = maxDistance;
            }
            else
            {
                Logger.LogError("Failed to apply Eccentric Vase Distance hook");
            }

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(30f)))
            {
                c.Next.Operand = duration;
            }
            else
            {
                Logger.LogError("Failed to apply Eccentric Vase Duration hook");
            }
        }
    }
}