using RoR2;
using UnityEngine.AddressableAssets;

namespace WellRoundedBalance.Interactable
{
    public class WhitePrinter : InteractableBase
    {
        public override string Name => "Interactables : White Printer";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            var whitePrinter = Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/Base/Interactables/Duplicator/iscDuplicator.asset").WaitForCompletion();
            whitePrinter.maxSpawnsPerStage = 2;
        }
    }
}