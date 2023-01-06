using UnityEngine;

namespace WellRoundedBalance.Interactables
{
    internal class AllShrines : InteractableBase
    {
        public override string Name => ":: Interactables ::: Shrines";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            var shrineBlood = Utils.Paths.InteractableSpawnCard.iscShrineBlood.Load<InteractableSpawnCard>();
            shrineBlood.maxSpawnsPerStage = 2;

            var shrineBlood2 = Utils.Paths.InteractableSpawnCard.iscShrineBloodSandy.Load<InteractableSpawnCard>();
            shrineBlood2.maxSpawnsPerStage = 2;

            var shrineBlood3 = Utils.Paths.InteractableSpawnCard.iscShrineBloodSnowy.Load<InteractableSpawnCard>();
            shrineBlood3.maxSpawnsPerStage = 2;

            var shrineCombat = Utils.Paths.InteractableSpawnCard.iscShrineCombat.Load<InteractableSpawnCard>();
            shrineCombat.maxSpawnsPerStage = 2;

            var shrineCombat2 = Utils.Paths.InteractableSpawnCard.iscShrineCombatSandy.Load<InteractableSpawnCard>();
            shrineCombat2.maxSpawnsPerStage = 2;

            var shrineCombat3 = Utils.Paths.InteractableSpawnCard.iscShrineCombatSnowy.Load<InteractableSpawnCard>();
            shrineCombat3.maxSpawnsPerStage = 2;

            var shrineRestack = Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/Base/ShrineRestack/iscShrineRestack.asset").WaitForCompletion();
            shrineRestack.maxSpawnsPerStage = 2;
            shrineRestack.directorCreditCost = 25;

            var shrineRestack2 = Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/Base/ShrineRestack/iscShrineRestackSandy.asset").WaitForCompletion();
            shrineRestack2.maxSpawnsPerStage = 2;
            shrineRestack2.directorCreditCost = 25;

            var shrineRestack3 = Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/Base/ShrineRestack/iscShrineRestackSnowy.asset").WaitForCompletion();
            shrineRestack3.maxSpawnsPerStage = 2;
            shrineRestack3.directorCreditCost = 25;

            var shrineRestackGO = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ShrineRestack/ShrineRestack.prefab").WaitForCompletion();
            var purchaseInteraction = shrineRestackGO.GetComponent<PurchaseInteraction>();
            purchaseInteraction.cost = 0;

            var shrineRestackGO2 = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ShrineRestack/ShrineRestackSandy Variant.prefab").WaitForCompletion();
            var purchaseInteraction2 = shrineRestackGO2.GetComponent<PurchaseInteraction>();
            purchaseInteraction2.cost = 0;

            var shrineRestackGO3 = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ShrineRestack/ShrineRestackSnowy Variant.prefab").WaitForCompletion();
            var purchaseInteraction3 = shrineRestackGO3.GetComponent<PurchaseInteraction>();
            purchaseInteraction3.cost = 0;

            var shrineWood = Utils.Paths.InteractableSpawnCard.iscShrineHealing.Load<InteractableSpawnCard>();
            shrineWood.maxSpawnsPerStage = 2;

            var shrineWoodGO = Utils.Paths.GameObject.ShrineHealing.Load<GameObject>();
            var purchaseInteraction4 = shrineWoodGO.GetComponent<PurchaseInteraction>();
            purchaseInteraction4.cost = 15;

            var behavior = shrineWoodGO.GetComponent<ShrineHealingBehavior>();
            behavior.baseRadius = 13f;
            behavior.radiusBonusPerPurchase = 13f;

            var shrineHealingWard = Utils.Paths.GameObject.ShrineHealingWard.Load<GameObject>().GetComponent<HealingWard>();
            shrineHealingWard.healFraction = 0.02f;

            var goldShrine = Utils.Paths.GameObject.ShrineGoldshoresAccess.Load<GameObject>();
            var goldShrinePurchaseInteraction = goldShrine.GetComponent<PurchaseInteraction>();
            goldShrinePurchaseInteraction.cost = 125;

            var goldShrineIsc = Utils.Paths.InteractableSpawnCard.iscShrineGoldshoresAccess.Load<InteractableSpawnCard>();
            goldShrineIsc.maxSpawnsPerStage = 1;

            On.RoR2.GlobalEventManager.OnInteractionBegin += GlobalEventManager_OnInteractionBegin;
        }

        private void GlobalEventManager_OnInteractionBegin(On.RoR2.GlobalEventManager.orig_OnInteractionBegin orig, GlobalEventManager self, Interactor interactor, IInteractable interactable, GameObject interactableObject)
        {
            if (interactableObject.name.Contains("ShrineRestack"))
            {
                var purchaseInteraction = interactableObject.GetComponent<PurchaseInteraction>();
                // purchaseInteraction
                // todo: change token to say +3 lunar coins and the display as well
                // also make it give +3 lunar coins
            }
            orig(self, interactor, interactable, interactableObject);
        }
    }
}