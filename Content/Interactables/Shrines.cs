namespace WellRoundedBalance.Interactables
{
    internal class AllShrines : InteractableBase
    {
        public override string Name => ":: Interactables ::: Shrines";

        [ConfigField("Shirne Of Blood Max Spawns Per Stage", "", 2)]
        public static int shrineOfBloodMaxSpawnsPerStage;

        [ConfigField("Shrine Of Combat Max Spawns Per Stage", "", 2)]
        public static int shrineOfCombatMaxSpawnsPerStage;

        [ConfigField("Shrine Of Order Max Spawns Per Stage", "", 2)]
        public static int shrineOfOrderMaxSpawnsPerStage;

        [ConfigField("Altar Of Gold Max Spawns Per Stage", "", 1)]
        public static int altarOfGoldMaxSpawnsPerStage;

        [ConfigField("Altar Of Gold Cost", "", 100)]
        public static int altarOfGoldCost;

        [ConfigField("Shrine Of Wood Max Spawns Per Stage", "", 2)]
        public static int shrineOfWoodMaxSpawnsPerStage;

        [ConfigField("Shrine Of Wood Cost", "", 15)]
        public static int shrineOfWoodInitialCost;

        [ConfigField("Shrine Of Wood Base Radius", "", 13f)]
        public static float shrineOfWoodBaseRadius;

        [ConfigField("Shrine Of Wood Radius Per Upgrade", "", 13f)]
        public static float shrineOfWoodRadiusPerUpgrade;

        [ConfigField("Shrine Of Wood Percent Healing Per Second", "Decimal.", 0.02f)]
        public static float shrineOfWoodPercentHealingPerSecond;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            var shrineBlood = Utils.Paths.InteractableSpawnCard.iscShrineBlood.Load<InteractableSpawnCard>();
            shrineBlood.maxSpawnsPerStage = shrineOfBloodMaxSpawnsPerStage;

            var shrineBlood2 = Utils.Paths.InteractableSpawnCard.iscShrineBloodSandy.Load<InteractableSpawnCard>();
            shrineBlood2.maxSpawnsPerStage = shrineOfBloodMaxSpawnsPerStage;

            var shrineBlood3 = Utils.Paths.InteractableSpawnCard.iscShrineBloodSnowy.Load<InteractableSpawnCard>();
            shrineBlood3.maxSpawnsPerStage = shrineOfBloodMaxSpawnsPerStage;

            var shrineCombat = Utils.Paths.InteractableSpawnCard.iscShrineCombat.Load<InteractableSpawnCard>();
            shrineCombat.maxSpawnsPerStage = shrineOfCombatMaxSpawnsPerStage;

            var shrineCombat2 = Utils.Paths.InteractableSpawnCard.iscShrineCombatSandy.Load<InteractableSpawnCard>();
            shrineCombat2.maxSpawnsPerStage = shrineOfCombatMaxSpawnsPerStage;

            var shrineCombat3 = Utils.Paths.InteractableSpawnCard.iscShrineCombatSnowy.Load<InteractableSpawnCard>();
            shrineCombat3.maxSpawnsPerStage = shrineOfCombatMaxSpawnsPerStage;

            var shrineRestack = Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/Base/ShrineRestack/iscShrineRestack.asset").WaitForCompletion();
            shrineRestack.maxSpawnsPerStage = shrineOfOrderMaxSpawnsPerStage;
            shrineRestack.directorCreditCost = 0;

            var shrineRestack2 = Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/Base/ShrineRestack/iscShrineRestackSandy.asset").WaitForCompletion();
            shrineRestack2.maxSpawnsPerStage = shrineOfOrderMaxSpawnsPerStage;
            shrineRestack2.directorCreditCost = 0;

            var shrineRestack3 = Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/Base/ShrineRestack/iscShrineRestackSnowy.asset").WaitForCompletion();
            shrineRestack3.maxSpawnsPerStage = shrineOfOrderMaxSpawnsPerStage;
            shrineRestack3.directorCreditCost = 0;

            var shrineRestackGO = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ShrineRestack/ShrineRestack.prefab").WaitForCompletion();
            var purchaseInteraction = shrineRestackGO.GetComponent<PurchaseInteraction>();
            purchaseInteraction.costType = CostTypeIndex.None;
            purchaseInteraction.contextToken = "WRB_SHRINE_RESTACK_CONTEXT";

            var shrineRestackGO2 = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ShrineRestack/ShrineRestackSandy Variant.prefab").WaitForCompletion();
            var purchaseInteraction2 = shrineRestackGO2.GetComponent<PurchaseInteraction>();
            purchaseInteraction2.costType = CostTypeIndex.None;
            purchaseInteraction2.contextToken = "WRB_SHRINE_RESTACK_CONTEXT";

            var shrineRestackGO3 = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ShrineRestack/ShrineRestackSnowy Variant.prefab").WaitForCompletion();
            var purchaseInteraction3 = shrineRestackGO3.GetComponent<PurchaseInteraction>();
            purchaseInteraction3.costType = CostTypeIndex.None;
            purchaseInteraction3.contextToken = "WRB_SHRINE_RESTACK_CONTEXT";

            var shrineWood = Utils.Paths.InteractableSpawnCard.iscShrineHealing.Load<InteractableSpawnCard>();
            shrineWood.maxSpawnsPerStage = shrineOfWoodMaxSpawnsPerStage;

            var shrineWoodGO = Utils.Paths.GameObject.ShrineHealing.Load<GameObject>();
            var purchaseInteraction4 = shrineWoodGO.GetComponent<PurchaseInteraction>();
            purchaseInteraction4.cost = shrineOfWoodInitialCost;

            var behavior = shrineWoodGO.GetComponent<ShrineHealingBehavior>();
            behavior.baseRadius = shrineOfWoodBaseRadius;
            behavior.radiusBonusPerPurchase = shrineOfWoodRadiusPerUpgrade;

            var shrineHealingWard = Utils.Paths.GameObject.ShrineHealingWard.Load<GameObject>().GetComponent<HealingWard>();
            shrineHealingWard.healFraction = shrineOfWoodPercentHealingPerSecond;

            var goldShrine = Utils.Paths.GameObject.ShrineGoldshoresAccess.Load<GameObject>();
            var goldShrinePurchaseInteraction = goldShrine.GetComponent<PurchaseInteraction>();
            goldShrinePurchaseInteraction.cost = altarOfGoldCost;

            var goldShrineIsc = Utils.Paths.InteractableSpawnCard.iscShrineGoldshoresAccess.Load<InteractableSpawnCard>();
            goldShrineIsc.maxSpawnsPerStage = altarOfGoldMaxSpawnsPerStage;

            GlobalEventManager.OnInteractionsGlobal += GlobalEventManager_OnInteractionsGlobal;

            LanguageAPI.Add("WRB_SHRINE_RESTACK_CONTEXT", "Offer to Shrine of Order (+3 Lunar Coins)");

            AddShrineOfOrderToMoreStages();
        }

        private void GlobalEventManager_OnInteractionsGlobal(Interactor interactor, IInteractable interactable, GameObject interactableObject)
        {
            if (interactableObject.name.Contains("ShrineRestack"))
            {
                var symbol = interactableObject.transform.GetChild(2);
                PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(RoR2Content.MiscPickups.LunarCoin.miscPickupIndex), symbol.transform.position, Vector3.up * 10f);
                PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(RoR2Content.MiscPickups.LunarCoin.miscPickupIndex), symbol.transform.position + new Vector3(-3f, 0f, 0f), Vector3.up * 10f);
                PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(RoR2Content.MiscPickups.LunarCoin.miscPickupIndex), symbol.transform.position + new Vector3(3f, 0f, 0f), Vector3.up * 10f);
            }
        }

        private void AddShrineOfOrderToMoreStages()
        {
            var shrineOfOrder = Utils.Paths.InteractableSpawnCard.iscShrineRestack.Load<InteractableSpawnCard>();
            var shrineOfOrderCard = new DirectorCard { spawnCard = shrineOfOrder, minimumStageCompletions = 0, selectionWeight = 40 };
            var shrineOfOrderCardHolder = new DirectorAPI.DirectorCardHolder { Card = shrineOfOrderCard, InteractableCategory = DirectorAPI.InteractableCategory.Shrines };

            var wetlandDCCS = Utils.Paths.DirectorCardCategorySelection.dccsFoggySwampInteractables.Load<DirectorCardCategorySelection>();
            var wetlandDCCSDLC = Utils.Paths.DirectorCardCategorySelection.dccsFoggySwampInteractablesDLC1.Load<DirectorCardCategorySelection>();

            DirectorAPI.AddCard(wetlandDCCS, shrineOfOrderCardHolder);
            DirectorAPI.AddCard(wetlandDCCSDLC, shrineOfOrderCardHolder);

            var sirensDCCS = Utils.Paths.DirectorCardCategorySelection.dccsShipgraveyardInteractables.Load<DirectorCardCategorySelection>();
            var sirensDCCSDLC = Utils.Paths.DirectorCardCategorySelection.dccsShipgraveyardInteractablesDLC1.Load<DirectorCardCategorySelection>();

            DirectorAPI.AddCard(sirensDCCS, shrineOfOrderCardHolder);
            DirectorAPI.AddCard(sirensDCCSDLC, shrineOfOrderCardHolder);
        }
    }
}