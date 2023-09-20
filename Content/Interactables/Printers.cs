using RoR2.Hologram;
using static Rewired.Utils.Classes.Utility.ObjectInstanceTracker;

namespace WellRoundedBalance.Interactables
{
    public class AllPrinters : InteractableBase<AllPrinters>
    {
        public override string Name => ":: Interactables ::::: Printers";

        [ConfigField("Common Printer Max Spawns Per Stage", "", 2)]
        public static int commonPrinterMaxSpawnsPerStage;

        [ConfigField("Common Printer Director Credit Cost", "", 6)]
        public static int commonPrinterDirectorCreditCost;

        [ConfigField("Common Printer Max Uses", "", 5)]
        public static int maxCommonUses;

        [ConfigField("Additional Common Printer Max Uses Per Player", "Only affects multiplayer. Rounded down.", 1f)]
        public static float commonPlayer;

        [ConfigField("Common Printer Weight Multiplier", "", 0.6f)]
        public static float commonWeightMultiplier;

        [ConfigField("Uncommon Printer Max Spawns Per Stage", "", 2)]
        public static int uncommonPrinterMaxSpawnsPerStage;

        [ConfigField("Uncommon Printer Director Credit Cost", "", 5)]
        public static int uncommonPrinterDirectorCreditCost;

        [ConfigField("Uncommon Printer Max Uses", "", 2)]
        public static int maxUncommonUses;

        [ConfigField("Additional Uncommon Printer Max Uses Per Player", "Only affects multiplayer. Rounded down.", 1f)]
        public static float uncommonPlayer;

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
        public static InteractableSpawnCard whitePrinter = null;

        public static Dictionary<GameObject, int> uses;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            whitePrinter = Utils.Paths.InteractableSpawnCard.iscDuplicator.Load<InteractableSpawnCard>();

            var cPrinter = Utils.Paths.GameObject.Duplicator.Load<GameObject>();
            cPrinter.AddComponent<PrinterUseCounter>();
            cPrinter.AddComponent<PrinterHologram>();

            var uPrinter = Utils.Paths.GameObject.DuplicatorLarge.Load<GameObject>();
            uPrinter.AddComponent<PrinterUseCounter>();
            uPrinter.AddComponent<PrinterHologram>();

            var lPrinter = Utils.Paths.GameObject.DuplicatorMilitary.Load<GameObject>();
            lPrinter.AddComponent<PrinterUseCounter>();
            lPrinter.AddComponent<PrinterHologram>();

            var yPrinter = Utils.Paths.GameObject.DuplicatorWild.Load<GameObject>();
            yPrinter.AddComponent<PrinterUseCounter>();
            yPrinter.AddComponent<PrinterHologram>();

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
            On.RoR2.ClassicStageInfo.Start += ClassicStageInfo_Start;
            Changes();
        }

        private void ClassicStageInfo_Start(On.RoR2.ClassicStageInfo.orig_Start orig, ClassicStageInfo self)
        {
            orig(self);
            var categories = self.interactableCategories.categories;
            for (int i = 0; i < categories.Length; i++)
            {
                var categoryIndex = categories[i];
                for (int j = 0; j < categoryIndex.cards.Length; j++)
                {
                    var cardIndex = categoryIndex.cards[j];
                    if (cardIndex.spawnCard == whitePrinter)
                    {
                        cardIndex.selectionWeight = Mathf.RoundToInt(cardIndex.selectionWeight * commonWeightMultiplier);
                        break;
                    }
                }
            }
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
            if (self.CanBeAffordedByInteractor(activator) && self.name.Contains("Duplicator") && ShouldSubtractUsesCount(activator))
            {
                uses[self.gameObject]--;
                var counter = self.gameObject.GetComponent<PrinterUseCounter>();
                if (counter)
                {
                    counter.useCount--;
                }
            }

            orig(self, activator);
        }

        private bool ShouldSubtractUsesCount(Interactor interactor)
        {
            var body = interactor.GetComponent<CharacterBody>();
            if (body)
            {
                var inventory = body.inventory;
                if (inventory)
                {
                    if (inventory.GetItemCount(DLC1Content.Items.RegeneratingScrap) > 0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private void PurchaseInteraction_Awake(On.RoR2.PurchaseInteraction.orig_Awake orig, PurchaseInteraction self)
        {
            orig(self);
            // Main.WRBLogger.LogError("participating player count is " + Run.instance.participatingPlayerCount);
            var maxCommons = Mathf.FloorToInt(maxCommonUses + (Run.instance.participatingPlayerCount - 1) * commonPlayer);
            var maxUncommons = Mathf.FloorToInt(maxUncommonUses + (Run.instance.participatingPlayerCount - 1) * uncommonPlayer);
            if (!NetworkServer.active) return;
            var counter = self.gameObject.GetComponent<PrinterUseCounter>();
            if (counter == null) return;
            switch (self.name)
            {
                case "Duplicator(Clone)":
                    InitUses(self.gameObject, maxCommons);
                    counter.useCount = maxCommons;
                    break;

                case "DuplicatorLarge(Clone)":
                    InitUses(self.gameObject, maxUncommons);
                    counter.useCount = maxCommons;
                    break;

                case "DuplicatorMilitary(Clone)":
                    InitUses(self.gameObject, maxLegendaryUses);
                    counter.useCount = maxLegendaryUses;
                    break;

                case "DuplicatorWild(Clone)":
                    InitUses(self.gameObject, maxBossUses);
                    counter.useCount = maxBossUses;
                    break;
            }
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

    public class PrinterUseCounter : MonoBehaviour
    {
        public int useCount;
        public float timer;
        public float explosionInterval = 0.7f;
        public float deleteInterval = 0.8f;

        private void FixedUpdate()
        {
            if (useCount <= 0)
            {
                timer += Time.fixedDeltaTime;
                if (timer >= explosionInterval)
                {
                    EffectManager.SpawnEffect(Utils.Paths.GameObject.ExplosionVFX.Load<GameObject>(), new EffectData
                    {
                        origin = transform.position,
                        scale = 4.5f
                    }, true);
                }
                if (timer >= deleteInterval)
                {
                    gameObject.SetActive(false);
                }
            }
        }
    }

    public class PrinterHologram : MonoBehaviour, IHologramContentProvider
    {
        public PrinterUseCounter counter;

        private void Start()
        {
            counter = gameObject.GetComponent<PrinterUseCounter>();
        }

        public GameObject GetHologramContentPrefab()
        {
            return PlainHologram.hologramContentPrefab;
        }

        public bool ShouldDisplayHologram(GameObject viewer)
        {
            var distance = Vector3.Distance(viewer.transform.position, gameObject.transform.position);
            if (distance <= 15f)
            {
                return true;
            }
            return false;
        }

        public void UpdateHologramContent(GameObject self)
        {
            var hologram = self.GetComponent<PlainHologram.PlainHologramContent>();
            if (hologram)
            {
                hologram.text = counter.useCount + (counter.useCount == 1 ? " use left" : " uses left");
                hologram.color = Color.white;
            }
        }
    }
}