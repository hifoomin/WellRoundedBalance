namespace WellRoundedBalance.Interactables
{
    public class AllPrinters : InteractableBase<AllPrinters>
    {
        public override string Name => ":: Interactables :::: Printers";

        [ConfigField("Common Printer Max Spawns Per Stage", "", 2)]
        public static int commonPrinterMaxSpawnsPerStage;

        [ConfigField("Common Printer Director Credit Cost", "", 6)]
        public static int commonPrinterDirectorCreditCost;

        [ConfigField("Uncommon Printer Max Spawns Per Stage", "", 2)]
        public static int uncommonPrinterMaxSpawnsPerStage;

        [ConfigField("Uncommon Printer Director Credit Cost", "", 7)]
        public static int uncommonPrinterDirectorCreditCost;

        [ConfigField("Legendary Printer Max Spawns Per Stage", "", 1)]
        public static int legendaryPrinterMaxSpawnsPerStage;

        [ConfigField("Boss Printer Max Spawns Per Stage", "", 1)]
        public static int bossPrinterMaxSpawnsPerStage;

        [ConfigField("Boss Printer is Loop Only", true)]
        public static bool bossPrinterLoopOnly;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            var whitePrinter = Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/Base/Duplicator/iscDuplicator.asset").WaitForCompletion();
            whitePrinter.maxSpawnsPerStage = commonPrinterMaxSpawnsPerStage;
            whitePrinter.directorCreditCost = commonPrinterDirectorCreditCost; // up from 5

            var greenPrinter = Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/Base/DuplicatorLarge/iscDuplicatorLarge.asset").WaitForCompletion();
            greenPrinter.directorCreditCost = uncommonPrinterDirectorCreditCost; // down from 10
            greenPrinter.maxSpawnsPerStage = uncommonPrinterMaxSpawnsPerStage;

            var redPrinter = Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/Base/DuplicatorMilitary/iscDuplicatorMilitary.asset").WaitForCompletion();
            redPrinter.maxSpawnsPerStage = legendaryPrinterMaxSpawnsPerStage;

            var yellowPrinter = Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/Base/DuplicatorWild/iscDuplicatorWild.asset").WaitForCompletion();
            yellowPrinter.maxSpawnsPerStage = bossPrinterMaxSpawnsPerStage;
            if (bossPrinterLoopOnly) On.RoR2.ClassicStageInfo.RebuildCards += (orig, self) =>
            {
                orig(self);
                if (Run.instance.loopClearCount <= 0) self.interactableCategories.RemoveCardsThatFailFilter(x => x.spawnCard != yellowPrinter);
            };
        }
    }
}