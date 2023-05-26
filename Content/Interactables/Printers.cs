namespace WellRoundedBalance.Interactables
{
    public class AllPrinters : InteractableBase<AllPrinters>
    {
        public override string Name => ":: Interactables ::::: Printers";

        [ConfigField("Common Printer Max Spawns Per Stage", "", 2)]
        public static int commonPrinterMaxSpawnsPerStage;

        [ConfigField("Common Printer Director Credit Cost", "", 6)]
        public static int commonPrinterDirectorCreditCost;

        [ConfigField("Common Printer Max Uses", "", 3)]
        public static int maxCommonUses;

        [ConfigField("Uncommon Printer Max Spawns Per Stage", "", 2)]
        public static int uncommonPrinterMaxSpawnsPerStage;

        [ConfigField("Uncommon Printer Director Credit Cost", "", 7)]
        public static int uncommonPrinterDirectorCreditCost;

        [ConfigField("Uncommon Printer Max Uses", "", 2)]
        public static int maxUncommonUses;

        [ConfigField("Legendary Printer Max Spawns Per Stage", "", 1)]
        public static int legendaryPrinterMaxSpawnsPerStage;

        [ConfigField("Legendary Printer Max Uses", "", 1)]
        public static int maxLegendaryUses;

        [ConfigField("Boss Printer Max Spawns Per Stage", "", 1)]
        public static int bossPrinterMaxSpawnsPerStage;

        [ConfigField("Boss Printer Max Uses", "", 2)]
        public static int maxBossUses;

        [ConfigField("Boss Printer is Loop Only", true)]
        public static bool bossPrinterLoopOnly;

        public static InteractableSpawnCard yellowPrinter = Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/Base/DuplicatorWild/iscDuplicatorWild.asset").WaitForCompletion();

        public static Dictionary<GameObject, int> uses;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            uses = new();

            Stage.onServerStageComplete += stage => uses.Clear();

            if (bossPrinterLoopOnly) On.RoR2.ClassicStageInfo.RebuildCards += (orig, self) =>
            {
                orig(self);
                if (Run.instance.loopClearCount <= 0) self.interactableCategories.RemoveCardsThatFailFilter(x => x.spawnCard != yellowPrinter);
            };

            On.RoR2.PurchaseInteraction.Awake += PurchaseInteraction_Awake;
            On.RoR2.PurchaseInteraction.OnInteractionBegin += PurchaseInteraction_OnInteractionBegin;
            On.EntityStates.Duplicator.Duplicating.DropDroplet += Duplicating_DropDroplet;

            Changes();
        }

        private void Duplicating_DropDroplet(On.EntityStates.Duplicator.Duplicating.orig_DropDroplet orig, EntityStates.Duplicator.Duplicating self)
        {
            orig(self);
            if (uses.ContainsKey(self.gameObject) && uses[self.gameObject] == 0)
            {
                self.outer.GetComponent<ShopTerminalBehavior>().SetHasBeenPurchased(true);
                self.outer.GetComponent<ShopTerminalBehavior>().SetNoPickup();
                self.outer.GetComponent<PurchaseInteraction>().enabled = false;
            }
        }

        private void PurchaseInteraction_OnInteractionBegin(On.RoR2.PurchaseInteraction.orig_OnInteractionBegin orig, PurchaseInteraction self, Interactor activator)
        {
            if (self.CanBeAffordedByInteractor(activator) && self.name.Contains("Duplicator")) uses[self.gameObject]--;
            orig(self, activator);
        }

        private void PurchaseInteraction_Awake(On.RoR2.PurchaseInteraction.orig_Awake orig, PurchaseInteraction self)
        {
            orig(self);
            // Debug.LogErrorFormat("max boss, legendary, uncommon, common uses: {0} {1} {2} {3}", maxBossUses, maxLegendaryUses, maxUncommonUses, maxCommonUses);
            if (!NetworkServer.active) return;
            if (self.name.Contains("DuplicatorWild")) InitUses(self.gameObject, maxBossUses);
            else if (self.name.Contains("DuplicatorMilitary")) InitUses(self.gameObject, maxLegendaryUses);
            else if (self.name.Contains("DuplicatorLarge")) InitUses(self.gameObject, maxUncommonUses);
            else if (self.name.Contains("Duplicator")) InitUses(self.gameObject, maxCommonUses);
        }

        private void Changes()
        {
            var whitePrinter = Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/Base/Duplicator/iscDuplicator.asset").WaitForCompletion();
            whitePrinter.maxSpawnsPerStage = commonPrinterMaxSpawnsPerStage;
            whitePrinter.directorCreditCost = commonPrinterDirectorCreditCost; // up from 5

            var greenPrinter = Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/Base/DuplicatorLarge/iscDuplicatorLarge.asset").WaitForCompletion();
            greenPrinter.directorCreditCost = uncommonPrinterDirectorCreditCost; // down from 10
            greenPrinter.maxSpawnsPerStage = uncommonPrinterMaxSpawnsPerStage;

            var redPrinter = Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/Base/DuplicatorMilitary/iscDuplicatorMilitary.asset").WaitForCompletion();
            redPrinter.maxSpawnsPerStage = legendaryPrinterMaxSpawnsPerStage;

            yellowPrinter.maxSpawnsPerStage = bossPrinterMaxSpawnsPerStage;
        }

        private void InitUses(GameObject self, int use)
        {
            if (!NetworkServer.active) return;
            if (!uses.ContainsKey(self)) uses.Add(self, use);
            else uses[self] = use;
        }
    }
}