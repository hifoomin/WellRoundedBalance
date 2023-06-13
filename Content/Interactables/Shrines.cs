using System;

namespace WellRoundedBalance.Interactables
{
    internal class AllShrines : InteractableBase<AllShrines>
    {
        public override string Name => ":: Interactables ::: Shrines";

        [ConfigField("Shirne Of Blood Max Spawns Per Stage", "", 2)]
        public static int shrineOfBloodMaxSpawnsPerStage;

        [ConfigField("Shrine Of Combat Max Spawns Per Stage", "", 2)]
        public static int shrineOfCombatMaxSpawnsPerStage;

        [ConfigField("Shrine Of Order Max Spawns Per Stage", "", 1)]
        public static int shrineOfOrderMaxSpawnsPerStage;

        [ConfigField("Altar Of Gold Max Spawns Per Stage", "", 1)]
        public static int altarOfGoldMaxSpawnsPerStage;

        [ConfigField("Altar Of Gold Cost", "", 100)]
        public static int altarOfGoldCost;

        [ConfigField("Shrine of The Woods Max Spawns Per Stage", "", 2)]
        public static int shrineOfWoodMaxSpawnsPerStage;

        [ConfigField("Shrine of The Woods Cost", "", 15)]
        public static int shrineOfWoodInitialCost;

        [ConfigField("Remove Shrine of The Woods from Distant Roost?", "", true)]
        public static bool removeShrineWood;

        [ConfigField("Shrine of The Woods Base Radius", "", 20f)]
        public static float shrineOfWoodBaseRadius;

        [ConfigField("Shrine of Woods Radius Per Upgrade", "", 20f)]
        public static float shrineOfWoodRadiusPerUpgrade;

        [ConfigField("Shrine of Woods Percent Healing Per Second", "Decimal.", 0.08f)]
        public static float shrineOfWoodPercentHealingPerSecond;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            var shrineBlood = Utils.Paths.InteractableSpawnCard.iscShrineBlood.Load<InteractableSpawnCard>();
            shrineBlood.maxSpawnsPerStage = shrineOfBloodMaxSpawnsPerStage;
            shrineBlood.directorCreditCost = 0;

            var shrineBlood2 = Utils.Paths.InteractableSpawnCard.iscShrineBloodSandy.Load<InteractableSpawnCard>();
            shrineBlood2.maxSpawnsPerStage = shrineOfBloodMaxSpawnsPerStage;
            shrineBlood2.directorCreditCost = 0;

            var shrineBlood3 = Utils.Paths.InteractableSpawnCard.iscShrineBloodSnowy.Load<InteractableSpawnCard>();
            shrineBlood3.maxSpawnsPerStage = shrineOfBloodMaxSpawnsPerStage;
            shrineBlood3.directorCreditCost = 0;

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
            shrineWood.directorCreditCost = 0;

            var shrineWoodGO = Utils.Paths.GameObject.ShrineHealing.Load<GameObject>();
            var purchaseInteraction4 = shrineWoodGO.GetComponent<PurchaseInteraction>();
            purchaseInteraction4.cost = shrineOfWoodInitialCost;

            var behavior = shrineWoodGO.GetComponent<ShrineHealingBehavior>();
            behavior.baseRadius = shrineOfWoodBaseRadius;
            behavior.radiusBonusPerPurchase = shrineOfWoodRadiusPerUpgrade;

            var shrineHealingWard = Utils.Paths.GameObject.ShrineHealingWard.Load<GameObject>().GetComponent<HealingWard>();
            shrineHealingWard.healFraction = shrineOfWoodPercentHealingPerSecond * 0.25f;

            var goldShrine = Utils.Paths.GameObject.ShrineGoldshoresAccess.Load<GameObject>();
            var goldShrinePurchaseInteraction = goldShrine.GetComponent<PurchaseInteraction>();
            goldShrinePurchaseInteraction.cost = altarOfGoldCost;

            var goldShrineIsc = Utils.Paths.InteractableSpawnCard.iscShrineGoldshoresAccess.Load<InteractableSpawnCard>();
            goldShrineIsc.maxSpawnsPerStage = altarOfGoldMaxSpawnsPerStage;

            GlobalEventManager.OnInteractionsGlobal += GlobalEventManager_OnInteractionsGlobal;

            LanguageAPI.Add("WRB_SHRINE_RESTACK_CONTEXT", "Offer to Shrine of Order (+3 Lunar Coins)");

            AddShrineOfOrderToMoreStages();
            if (removeShrineWood)
            {
                Unroost();
            }
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

        private void Unroost()
        {
            var shrineBloodDC = new DirectorCard
            {
                spawnCard = Utils.Paths.InteractableSpawnCard.iscShrineBlood.Load<InteractableSpawnCard>(),
                selectionWeight = 3,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                preventOverhead = false,
                minimumStageCompletions = 0
            };

            var shrineBossDC = new DirectorCard
            {
                spawnCard = Utils.Paths.InteractableSpawnCard.iscShrineBoss.Load<InteractableSpawnCard>(),
                selectionWeight = 1,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                preventOverhead = false,
                minimumStageCompletions = 0
            };

            var shrineChanceDC = new DirectorCard
            {
                spawnCard = Utils.Paths.InteractableSpawnCard.iscShrineChance.Load<InteractableSpawnCard>(),
                selectionWeight = 4,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                preventOverhead = false,
                minimumStageCompletions = 0
            };

            var roost1 = Utils.Paths.DirectorCardCategorySelection.dccsBlackBeachInteractables.Load<DirectorCardCategorySelection>();
            Array.Resize(ref roost1.categories[2].cards, 3);
            roost1.categories[2].cards[0] = shrineBloodDC;
            roost1.categories[2].cards[1] = shrineBossDC;
            roost1.categories[2].cards[2] = shrineChanceDC;

            var roost2 = Utils.Paths.DirectorCardCategorySelection.dccsBlackBeachInteractablesDLC1.Load<DirectorCardCategorySelection>();
            Array.Resize(ref roost2.categories[2].cards, 3);
            roost2.categories[2].cards[0] = shrineBloodDC;
            roost2.categories[2].cards[1] = shrineBossDC;
            roost2.categories[2].cards[2] = shrineChanceDC;
        }
    }
}