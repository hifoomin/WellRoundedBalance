using WellRoundedBalance.Interactables;

namespace WellRoundedBalance.Items.Greens
{
    public class RegeneratingScrap : ItemBase<RegeneratingScrap>
    {
        public override string Name => ":: Items :: Greens :: Regenerating Scrap";
        public override ItemDef InternalPickup => DLC1Content.Items.RegeneratingScrap;

        public override string PickupText => "Prioritized when used with <style=cIsHealing>Uncommon</style> 3D Printers. Usable once per stage.";
        public override string DescText => "Does nothing. Prioritized when used with <style=cIsHealing>Uncommon</style> 3D Printers. At the start of each stage, it regenerates.";

        public static GameObject glow = null;

        public static List<GameObject> printers = new();

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            glow = Main.wellroundedbalance.LoadAsset<GameObject>("Assets/WellRoundedBalance/Glow.prefab");
            Changes();
            GlobalEventManager.OnInteractionsGlobal += GlobalEventManager_OnInteractionsGlobal;
            On.RoR2.Stage.Start += Stage_Start;
        }

        private void Stage_Start(On.RoR2.Stage.orig_Start orig, Stage self)
        {
            orig(self);
            if (AnyoneHasRegenScrapStage())
            {
                var printerList = Object.FindObjectsOfType(typeof(PrinterUseCounter)) as PrinterUseCounter[];
                foreach (PrinterUseCounter puc in printerList)
                {
                    var trans = puc.transform;
                    var glow = trans.Find("Glow");
                    if (glow)
                    {
                        glow.gameObject.SetActive(true);
                        glow.transform.localPosition = Vector3.zero;
                        glow.transform.position = trans.position;
                        printers.Add(puc.gameObject);
                    }
                }
            }
        }

        private void GlobalEventManager_OnInteractionsGlobal(Interactor interactor, IInteractable interactable, GameObject interactableObject)
        {
            var purchaseInteraction = interactableObject.GetComponent<PurchaseInteraction>();
            if (!purchaseInteraction)
            {
                return;
            }

            if (AnyoneHasRegenScrapRuntime())
            {
                foreach (GameObject gameObject in printers)
                {
                    if (gameObject && gameObject.transform)
                    {
                        var glow = gameObject.transform.Find("Glow");
                        if (glow)
                        {
                            glow.transform.localPosition = Vector3.zero;
                            glow.transform.position = glow.transform.parent.position;
                            glow.gameObject.SetActive(true);
                        }
                    }
                }
            }
            else
            {
                foreach (GameObject gameObject in printers)
                {
                    if (gameObject && gameObject.transform)
                    {
                        var glow = gameObject.transform.Find("Glow");
                        if (glow)
                        {
                            glow.transform.localPosition = Vector3.zero;
                            glow.transform.position = glow.transform.parent.position;
                            glow.gameObject.SetActive(false);
                        }
                    }
                }
            }
        }

        private bool AnyoneHasRegenScrapStage()
        {
            bool anyRealers = false;
            foreach (CharacterMaster master in CharacterMaster.instancesList)
            {
                if (master.inventory)
                {
                    if (master.inventory.GetItemCount(DLC1Content.Items.RegeneratingScrap) > 0 || master.inventory.GetItemCount(DLC1Content.Items.RegeneratingScrapConsumed) > 0)
                    {
                        anyRealers = true;
                        break;
                    }
                }
            }
            // Logger.LogError("anyone has regen scrap: " + anyRealers);
            return anyRealers;
        }

        private bool AnyoneHasRegenScrapRuntime()
        {
            bool anyRealers = false;
            foreach (CharacterMaster master in CharacterMaster.instancesList)
            {
                if (master.inventory)
                {
                    if (master.inventory.GetItemCount(DLC1Content.Items.RegeneratingScrap) > 0)
                    {
                        anyRealers = true;
                        break;
                    }
                }
            }
            // Logger.LogError("anyone has regen scrap: " + anyRealers);
            return anyRealers;
        }

        private bool IsGreenPrinter(PurchaseInteraction purchaseInteraction)
        {
            if (purchaseInteraction.displayNameToken == "DUPLICATOR_LARGE_NAME" && purchaseInteraction.costType == CostTypeIndex.GreenItem)
            {
                return true;
            }
            else return false;
        }

        private void Changes()
        {
            var greenPrinter = Utils.Paths.GameObject.DuplicatorLarge.Load<GameObject>();
            glow.transform.SetParent(greenPrinter.transform);
            glow.transform.position = Vector3.zero;
            glow.transform.localPosition = Vector3.zero;
            glow.SetActive(false);
        }
    }
}