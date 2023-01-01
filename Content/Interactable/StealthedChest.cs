using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace WellRoundedBalance.Interactable
{
    public class StealthedChest : InteractableBase
    {
        public override string Name => "Interactables ::::: Stealthed Chest";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            var stealthedChest = Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/Base/Chest1StealthedVariant/iscChest1Stealthed.asset").WaitForCompletion();
            stealthedChest.maxSpawnsPerStage = 2;
            stealthedChest.directorCreditCost = 1;

            var stealthedChestGO = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Chest1StealthedVariant/Chest1StealthedVariant.prefab").WaitForCompletion();
            var chestBehavior = stealthedChestGO.GetComponent<ChestBehavior>();
            chestBehavior.tier1Chance = 0.65f;
            chestBehavior.tier2Chance = 0.35f;
        }
    }
}