namespace WellRoundedBalance.Interactables
{
    public class AllPrinters : InteractableBase
    {
        public override string Name => ":: Interactables :::: Printers";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            var whitePrinter = Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/Base/Duplicator/iscDuplicator.asset").WaitForCompletion();
            whitePrinter.maxSpawnsPerStage = 2;

            var greenPrinter = Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/Base/DuplicatorLarge/iscDuplicatorLarge.asset").WaitForCompletion();
            greenPrinter.directorCreditCost = 7; // down from 10
            greenPrinter.maxSpawnsPerStage = 2;

            var redPrinter = Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/Base/DuplicatorMilitary/iscDuplicatorMilitary.asset").WaitForCompletion();
            redPrinter.maxSpawnsPerStage = 1;

            var yellowPrinter = Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/Base/DuplicatorWild/iscDuplicatorWild.asset").WaitForCompletion();
            yellowPrinter.maxSpawnsPerStage = 1;
        }
    }
}