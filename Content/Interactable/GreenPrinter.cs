using RoR2;
using UnityEngine.AddressableAssets;

namespace WellRoundedBalance.Interactable
{
    public class GreenPrinter : InteractableBase
    {
        public override string Name => "Interactables :: Green Printer";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            var greenPrinter = Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/Base/DuplicatorLarge/iscDuplicatorLarge.asset").WaitForCompletion();
            greenPrinter.directorCreditCost = 8; // down from 10
            greenPrinter.maxSpawnsPerStage = 2;
        }
    }
}