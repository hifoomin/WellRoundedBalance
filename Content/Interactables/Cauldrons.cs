using Mono.Cecil;

namespace WellRoundedBalance.Interactables
{
    internal class Cauldrons : InteractableBase<Cauldrons>
    {
        public override string Name => ":: Interactables ::::::::: Cauldrons";

        [ConfigField("Enable Red Scrap to Red HRT?", "Makes it possible to convert X Red Scrap into a Red Item", true)]
        public static bool hrt;

        [ConfigField("White Item/Scrap to Green amount", "The amount of White Items/Scrap required to convert to a Green Item", 3)]
        public static int whiteToGreenAmount;

        [ConfigField("Green Item/Scrap to Red amount", "The amount of Green Items/Scrap required to convert to a Red Item", 5)]
        public static int greenToRedAmount;

        [ConfigField("Red Scrap to Red amount", "The amount of Red Scrap required to convert to a Red Item", 2)]
        public static int redScrapToRedAmount;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            Changes();
            On.RoR2.PurchaseInteraction.CanBeAffordedByInteractor += PurchaseInteraction_CanBeAffordedByInteractor;
            On.RoR2.PurchaseInteraction.OnInteractionBegin += PurchaseInteraction_OnInteractionBegin;
            On.RoR2.PurchaseInteraction.GetContextString += PurchaseInteraction_GetContextString;
            On.RoR2.CostHologramContent.FixedUpdate += CostHologramContent_FixedUpdate;
        }

        private void CostHologramContent_FixedUpdate(On.RoR2.CostHologramContent.orig_FixedUpdate orig, CostHologramContent self)
        {
            if (IsGreenToRedCauldron(null, self))
            {
                CostHologramContent.sharedStringBuilder.Clear();
                self.targetTextMesh.color = Color.white;
                self.targetTextMesh.SetText("<nobr><color=#E7543A>" + redScrapToRedAmount + " Scrap</color></nobr><br>OR<br><nobr><color=#77FF17>" + greenToRedAmount + " Item(s)</color></nobr>", true);
            }
            else
                orig(self);
        }

        private string PurchaseInteraction_GetContextString(On.RoR2.PurchaseInteraction.orig_GetContextString orig, PurchaseInteraction self, Interactor activator)
        {
            string text;
            if (IsGreenToRedCauldron(self))
            {
                PurchaseInteraction.sharedStringBuilder.Clear();
                PurchaseInteraction.sharedStringBuilder.Append(Language.GetString(self.contextToken));
                var hasCostType = self.costType > CostTypeIndex.None;
                if (hasCostType)
                {
                    PurchaseInteraction.sharedStringBuilder.Append(" <nobr>(<color=#E7543A>" + redScrapToRedAmount + " Scrap</color>)</nobr> / ");
                    PurchaseInteraction.sharedStringBuilder.Append(" <nobr>(");
                    CostTypeCatalog.GetCostTypeDef(self.costType).BuildCostStringStyled(self.cost, PurchaseInteraction.sharedStringBuilder, false, true);
                    PurchaseInteraction.sharedStringBuilder.Append(")</nobr>");
                }
                text = PurchaseInteraction.sharedStringBuilder.ToString();
            }
            else
            {
                text = orig(self, activator);
            }
            return text;
        }

        private void PurchaseInteraction_OnInteractionBegin(On.RoR2.PurchaseInteraction.orig_OnInteractionBegin orig, PurchaseInteraction self, Interactor activator)
        {
            if (IsGreenToRedCauldron(self) && GetRedScrapCount(activator) > 0)
            {
                self.costType = CostTypeIndex.RedItem;
                self.cost = redScrapToRedAmount;
                orig(self, activator);
                self.costType = CostTypeIndex.GreenItem;
                self.cost = greenToRedAmount;
            }
            else
                orig(self, activator);
        }

        private bool PurchaseInteraction_CanBeAffordedByInteractor(On.RoR2.PurchaseInteraction.orig_CanBeAffordedByInteractor orig, PurchaseInteraction self, Interactor activator)
        {
            return (IsGreenToRedCauldron(self) && GetRedScrapCount(activator) > 0) || orig(self, activator);
        }

        private int GetRedScrapCount(Interactor activator)
        {
            var body = activator.GetComponent<CharacterBody>();
            if (body)
            {
                var inventory = body.inventory;
                if (inventory)
                    return inventory.GetItemCount(RoR2Content.Items.ScrapRed);
            }
            return 0;
        }

        private bool IsGreenToRedCauldron(PurchaseInteraction purchaseInteraction, CostHologramContent costHologramContent = null)
        {
            if (costHologramContent)
                return costHologramContent.costType == CostTypeIndex.GreenItem && costHologramContent.displayValue == greenToRedAmount;
            if (purchaseInteraction)
                return purchaseInteraction.displayNameToken == "BAZAAR_CAULDRON_NAME" && purchaseInteraction.costType == CostTypeIndex.GreenItem;
            else
                return false;
        }

        private void Changes()
        {
            var greenToRedCauldron = Utils.Paths.GameObject.LunarCauldronGreenToRedVariant.Load<GameObject>();
            var purchaseInteractionGTR = greenToRedCauldron.GetComponent<PurchaseInteraction>();
            purchaseInteractionGTR.cost = greenToRedAmount;

            var whiteToGreenCauldron = Utils.Paths.GameObject.LunarCauldronWhiteToGreen.Load<GameObject>();
            var purchaseInteractionWTG = whiteToGreenCauldron.GetComponent<PurchaseInteraction>();
            purchaseInteractionWTG.cost = whiteToGreenAmount;
        }
    }
}