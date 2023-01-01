using RoR2;
using UnityEngine.AddressableAssets;

namespace WellRoundedBalance.Interactable
{
    public class YellowPrinter : InteractableBase
    {
        public override string Name => "Interactables :::: Yellow Printer";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            var yellowPrinter = Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/Base/Interactables/DuplicatorWild/iscDuplicatorWild.asset").WaitForCompletion();
            yellowPrinter.maxSpawnsPerStage = 1;
        }
    }
}