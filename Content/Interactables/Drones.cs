namespace WellRoundedBalance.Interactables
{
    internal class AllDrones : InteractableBase
    {
        public override string Name => ":: Interactables ::::::::: Drones";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            var drone1 = Utils.Paths.InteractableSpawnCard.iscBrokenDrone1.Load<InteractableSpawnCard>();
            drone1.maxSpawnsPerStage = 2;

            var drone2 = Utils.Paths.InteractableSpawnCard.iscBrokenDrone2.Load<InteractableSpawnCard>();
            drone2.maxSpawnsPerStage = 2;

            var drone3 = Utils.Paths.InteractableSpawnCard.iscBrokenEmergencyDrone.Load<InteractableSpawnCard>();
            drone3.maxSpawnsPerStage = 1;

            var drone3purchaseInteraction = Utils.Paths.GameObject.EmergencyDroneBroken.Load<GameObject>().GetComponent<PurchaseInteraction>();
            drone3purchaseInteraction.cost = 80;

            var drone4 = Utils.Paths.InteractableSpawnCard.iscBrokenEquipmentDrone.Load<InteractableSpawnCard>();
            drone4.maxSpawnsPerStage = 1;
            drone4.directorCreditCost = 13; // down from 15

            var drone5 = Utils.Paths.InteractableSpawnCard.iscBrokenFlameDrone.Load<InteractableSpawnCard>();
            drone5.maxSpawnsPerStage = 2;

            var drone5purchaseInteraction = Utils.Paths.GameObject.FlameDroneBroken.Load<GameObject>().GetComponent<PurchaseInteraction>();
            drone5purchaseInteraction.cost = 80;

            var drone6 = Utils.Paths.InteractableSpawnCard.iscBrokenMegaDrone.Load<InteractableSpawnCard>();
            drone6.maxSpawnsPerStage = 1;

            var drone6purchaseInteraction = Utils.Paths.GameObject.MegaDroneBroken.Load<GameObject>().GetComponent<PurchaseInteraction>();
            drone6purchaseInteraction.cost = 160;

            var drone7 = Utils.Paths.InteractableSpawnCard.iscBrokenMissileDrone.Load<InteractableSpawnCard>();
            drone7.maxSpawnsPerStage = 2;

            var gunnerTurret = Utils.Paths.InteractableSpawnCard.iscBrokenTurret1.Load<InteractableSpawnCard>();
            gunnerTurret.maxSpawnsPerStage = 2;
        }
    }
}