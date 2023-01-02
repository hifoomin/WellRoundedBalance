using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace WellRoundedBalance.Interactable
{
    public class ShrineOfOrder : InteractableBase
    {
        public override string Name => "Interactables :::::: Shrine of Order";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
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
            purchaseInteraction.cost = -3;

            var shrineRestackGO2 = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ShrineRestack/ShrineRestackSandy Variant.prefab").WaitForCompletion();
            var purchaseInteraction2 = shrineRestackGO2.GetComponent<PurchaseInteraction>();
            purchaseInteraction2.cost = -3;

            var shrineRestackGO3 = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ShrineRestack/ShrineRestackSnowy Variant.prefab").WaitForCompletion();
            var purchaseInteraction3 = shrineRestackGO3.GetComponent<PurchaseInteraction>();
            purchaseInteraction3.cost = -3;
        }
    }
}