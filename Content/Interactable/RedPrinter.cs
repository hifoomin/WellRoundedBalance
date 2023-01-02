using RoR2;
using UnityEngine.AddressableAssets;

namespace WellRoundedBalance.Interactable
{
    public class RedPrinter : InteractableBase
    {
        public override string Name => "Interactables ::: Red Printer";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            var redPrinter = Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/Base/DuplicatorMilitary/iscDuplicatorMilitary.asset").WaitForCompletion();
            redPrinter.maxSpawnsPerStage = 1;
        }
    }
}