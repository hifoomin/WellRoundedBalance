namespace WellRoundedBalance.Interactables
{
    internal class AllDrones : InteractableBase<AllDrones>
    {
        public override string Name => ":: Interactables :::::::: Drones";

        [ConfigField("Max Spawns Per Stage", "", 2)]
        public static int maxSpawnsPerStage;

        [ConfigField("Emergency Drone Max Spawns Per Stage", "", 1)]
        public static int emergencyDroneMaxSpawnsPerStage;

        [ConfigField("Emergency Drone Cost", "", 80)]
        public static int emergencyDroneCost;

        [ConfigField("Equipment Drone Max Spawns Per Stage", "", 1)]
        public static int equipmentDroneMaxSpawnsPerStage;

        [ConfigField("Equipment Drone Director Credit Cost", "", 13)]
        public static int equipmentDroneDirectorCreditCost;

        [ConfigField("Incinerator Drone Cost", "", 80)]
        public static int incineratorDroneCost;

        [ConfigField("TC280 Prototype Max Spawns Per Stage", "", 1)]
        public static int TC280PrototypeMaxSpawnsPerStage;

        [ConfigField("TC280 Prototype Cost", "", 160)]
        public static int TC280PrototypeCost;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            var drone1 = Utils.Paths.InteractableSpawnCard.iscBrokenDrone1.Load<InteractableSpawnCard>();
            drone1.maxSpawnsPerStage = maxSpawnsPerStage;

            var drone2 = Utils.Paths.InteractableSpawnCard.iscBrokenDrone2.Load<InteractableSpawnCard>();
            drone2.maxSpawnsPerStage = maxSpawnsPerStage;

            var drone3 = Utils.Paths.InteractableSpawnCard.iscBrokenEmergencyDrone.Load<InteractableSpawnCard>();
            drone3.maxSpawnsPerStage = emergencyDroneMaxSpawnsPerStage;

            var drone3purchaseInteraction = Utils.Paths.GameObject.EmergencyDroneBroken.Load<GameObject>().GetComponent<PurchaseInteraction>();
            drone3purchaseInteraction.cost = emergencyDroneCost;

            var drone4 = Utils.Paths.InteractableSpawnCard.iscBrokenEquipmentDrone.Load<InteractableSpawnCard>();
            drone4.maxSpawnsPerStage = equipmentDroneMaxSpawnsPerStage;
            drone4.directorCreditCost = equipmentDroneDirectorCreditCost; // down from 15

            var drone5 = Utils.Paths.InteractableSpawnCard.iscBrokenFlameDrone.Load<InteractableSpawnCard>();
            drone5.maxSpawnsPerStage = maxSpawnsPerStage;

            var drone5purchaseInteraction = Utils.Paths.GameObject.FlameDroneBroken.Load<GameObject>().GetComponent<PurchaseInteraction>();
            drone5purchaseInteraction.cost = incineratorDroneCost;

            var drone6 = Utils.Paths.InteractableSpawnCard.iscBrokenMegaDrone.Load<InteractableSpawnCard>();
            drone6.maxSpawnsPerStage = TC280PrototypeMaxSpawnsPerStage;

            var drone6purchaseInteraction = Utils.Paths.GameObject.MegaDroneBroken.Load<GameObject>().GetComponent<PurchaseInteraction>();
            drone6purchaseInteraction.cost = TC280PrototypeCost;

            var drone7 = Utils.Paths.InteractableSpawnCard.iscBrokenMissileDrone.Load<InteractableSpawnCard>();
            drone7.maxSpawnsPerStage = maxSpawnsPerStage;

            var gunnerTurret = Utils.Paths.InteractableSpawnCard.iscBrokenTurret1.Load<InteractableSpawnCard>();
            gunnerTurret.maxSpawnsPerStage = maxSpawnsPerStage;
        }
    }
}